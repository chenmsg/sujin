﻿using ITOrm.Host.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITOrm.Utility.Helper;
using ITOrm.Host.Models;
using Newtonsoft.Json.Linq;
using ITOrm.Payment.Yeepay;
using Newtonsoft.Json;
using ITOrm.Manage.Filters;
using ITOrm.Utility.Const;
using System.Text;
using ITOrm.Payment.Const;
using ITOrm.Payment.Masget;
using ITOrm.Utility.Log;
using ITOrm.Utility.Cache;

namespace ITOrm.Manage.Controllers
{
    public class UsersController : Controller
    {
        public static UsersBLL userDao = new UsersBLL();
        public static YeepayUserBLL yeepayUserDao = new YeepayUserBLL();
        public static UserBankCardBLL userBankCardDao = new UserBankCardBLL();
        public static MasgetUserBLL masgetUserDao = new MasgetUserBLL();
        public static AccountBLL accountDao = new AccountBLL();
        public static AccountRecordBLL accountRecordDao = new AccountRecordBLL();
        public static AccountQueueBLL accountQueueDao = new AccountQueueBLL();
        public static BankTreatyApplyBLL bankTreatyApplyDao = new BankTreatyApplyBLL();
        public static DebarBankChannelBLL debarBankChannelDao = new DebarBankChannelBLL();
        string url = "/users/"; 
        string msg = "";
        // GET: Users
        [AdminFilter]
        public ActionResult Index(int pageIndex=1,int Type = -1,string KeyValue="", int IsRealState=-1,string StartTime="",string EndTime="",int VipType=-1)
        {
            KeyValue = KeyValue.Trim();
            #region where 条件
            StringBuilder where = new StringBuilder();
            where.Append("1=1");
            switch (Type)
            {
                case 0://用户ID
                    where.AppendFormat(" and UserId={0}", KeyValue);
                    break;
                case 1://手机号
                    where.AppendFormat(" and Mobile='{0}' ", KeyValue);
                    break;
                case 2://姓名
                    where.AppendFormat(" and RealName like '%{0}%'", KeyValue);
                    break;
                case 3://身份证
                    where.AppendFormat(" and IdCard='{0}'", KeyValue);
                    break;
                default:
                    break;
            }
            if (IsRealState != -1)
            {
                where.AppendFormat(" and IsRealState={0}",IsRealState);
            }
            if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
            {
                where.AppendFormat(" and CTime BETWEEN '{0}' AND '{1}'", StartTime,EndTime);
            }
            if (VipType != -1)
            {
                where.AppendFormat(" and VipType={0}", VipType);
            }
            #endregion

            int totalCount = 0;
            var listUsers = userDao.GetPaged(10, pageIndex, out totalCount, where.ToString());
            JArray list = new JArray();
            if (listUsers != null)
            {
                list=JArray.FromObject(listUsers); }
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    int UserId = item["UserId"].ToInt();
                    item["BankCard"] = "--";
                    item["BankName"] = "--";
                    var IsReal = item["IsRealState"].ToInt();
                    if (IsReal == 1)
                    {
                        var ukb = userBankCardDao.Single("UserId=@UserId and TypeId=0",new { UserId});
                        item["BankCard"] =ukb==null?"": ukb.BankCard;
                        item["BankName"] = ukb == null ? "" : ukb.BankName;
                    }

                    item["BaseRealName"] = "--";
                    var BaseUserId =item["BaseUserId"].ToInt();
                    if (BaseUserId != 0)
                    {
                        Users user = userDao.Single(BaseUserId);
                        item["BaseRealName"] = user.RealName;
                    }

                    item["Total"] = 0;
                    item["Available"] = 0;
                    item["Frozen"] = 0;
                    var account = accountDao.Single($"UserId={UserId}");
                    if (account!=null&&account.ID > 0)
                    {
                        item["Total"] = account.Total;
                        item["Available"] = account.Available;
                        item["Frozen"] = account.Frozen;
                    }
                }
            }
            return View(new ResultModel(list, totalCount));
        }

        public ActionResult Info(int id)
        {
            Users model = userDao.Single(id);
            return View(model);
        }

        public ActionResult FeeSetApi(int UserId,int VipType)
        {
            Logs.WriteLog($"Action:User,Cmd:FeeSetApi,UserId:{UserId},VipType：{VipType},ip:{ITOrm.Utility.Client.Ip.GetClientIp()}", "d:\\Log\\ITOrm", "FeeSetApi");
            var result= UsersDepository.UpgradeVip(UserId, VipType, (int)Logic.Platform.系统);
            return new RedirectResult($"/Prompt?state={result.backState}&msg={result.message}&url={url}");

        }

        public ActionResult AuditMerchant(int UserId)
        {
            YeepayUser yUser = yeepayUserDao.Single("UserId=@UserId", new { UserId });
            if (yUser == null)
            {
                return new RedirectResult($"/Prompt?state={-100}&msg=未开通商户&url={url}");
      
            }
            if (yUser.IsAudit == 1)
            {
                return new RedirectResult($"/Prompt?state={-100}&msg=已审核&url={url}");
            }

            var result=YeepayDepository.AuditMerchant(UserId, 1,ITOrm.Payment.Yeepay.Enums.AuditMerchant.SUCCESS, "审核成功");
            if (result.backState == 0)
            {
                return new RedirectResult($"/Prompt?state=0&msg={result.message}&url={url}");
            }
            return new RedirectResult($"/Prompt?state={-100}&msg={result.message}&url={url}"); ;
        }

        [HttpGet]
        public ActionResult UpdateBankCard(int Id)
        {
            UserBankCard kv = new  UserBankCard();
            if (Id > 0)
            {
                kv = userBankCardDao.Single(Id);
            }
            return View(kv);
        }

        [HttpPost]
        public ActionResult UpdateBankCard(string Mobile, string BankCard, string BankCode, string CVN2, string ExpiresYear, string ExpiresMouth, int ID)
        {
            int backState = -100;
            string msg = string.Empty;
            UserBankCard bank = new UserBankCard();
            string ubcurl = "";
            if (ID > 0)
            {
                bank = userBankCardDao.Single(ID);
                ubcurl = "/users/info/" + bank.UserId;
                if (bank.TypeId == 0 && bank.BankCard!=BankCard && bank.BankCode!=BankCode)
                {
                    return new RedirectResult($"/Prompt?state=-100&msg=结算卡不支持修改卡号和所属银行&url={url}");
                }
                bank.Mobile = Mobile;
                bank.BankCard = BankCard;
                bank.BankCode = BankCode;
                bank.CVN2 = CVN2;
                bank.ExpiresYear = ExpiresYear;
                bank.ExpiresMouth = ExpiresMouth;

                var list = bankTreatyApplyDao.GetQuery(" State=2 And UbkID=@ID", new { ID });
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        MasgetDepository.TreatyModify(ID, CVN2, ExpiresYear, ExpiresMouth, (int)Logic.Platform.系统, (Logic.ChannelType)item.ChannelType);
                    }
                }

                var result= userBankCardDao.Update(bank);
                backState = result ? 0 : -100;
                msg= result ? "修改成功" :"修改失败";
            }
  

            return new RedirectResult($"/Prompt?state={backState}&msg={msg}&url={ubcurl}");
        }


        public string GetDrawRecord(int UserId)
        {
            ResultModel result = new ResultModel();
           
            var listAccountRecord = accountRecordDao.GetQuery(5, "UserId=@UserId and TypeId=301", new { UserId }, " order by id desc");
            JArray list = new JArray();
            if (listAccountRecord != null && listAccountRecord.Count > 0)
            {
                foreach (var item in listAccountRecord)
                {
                    JObject data = new JObject();
                    data["ID"] = item.ID;
                    data["Amount"] = item.Amount.ToString("F2");
                    data["Available"] = item.Available.ToString("F2");
                    data["CTime"] = item.CTime.ToString("yyyy-MM-dd HH:mm:ss");
                    list.Add(data);
                }
                result.list = list;
            }
            return JsonConvert.SerializeObject(result);
        }


        public string Draw(int UserId,decimal Amount=0M)
        {

            ResultModel result = new ResultModel();
            if ( Amount==0M || UserId==0)
            {
                result.backState = -100;
                result.message = "提现金额不能为0!";
                return JsonConvert.SerializeObject(result);
            }

           
            Account account = accountDao.Single("UserId=@UserId",new { UserId});
            if (account.Available < Amount)
            {
                result.backState = -100;
                result.message = "可用余额不足!";
                return JsonConvert.SerializeObject(result);
            }
            AccountQueue acc = new AccountQueue();
            acc.Amount = Amount;
            acc.TypeId = (int)Logic.AccountType.提现到账;
            acc.InOrOut = -1;
            acc.UserId = UserId;
            acc.Platform = (int)Logic.Platform.系统;
            acc.Remark = "人工操作提现";
            int num=accountQueueDao.Insert(acc);
            if (num > 0)
            {
                result.backState = 0;
                result.message = "操作成功!";
                return JsonConvert.SerializeObject(result);
            }
            result.backState = -100;
            result.message = "操作失败!";
            return JsonConvert.SerializeObject(result);
        }


        public ActionResult DebarBank(int BankId,int ChannelId,int UserId)
        {
            int backState = -100;
            string msg = string.Empty;
       
            string ubcurl = "/users/info/" + UserId;
            if (BankId > 0 && ChannelId>0 && UserId>0)
            {
                DebarBankChannel dbc = new DebarBankChannel();
                dbc.BankID = BankId;
                dbc.ChannelID = ChannelId;
                dbc.UserId = UserId;
                var result = debarBankChannelDao.Insert(dbc);
                backState = result >0? 0 : -100;
                msg = result >0? "操作成功" : "操作失败";
                MemcachHelper.Delete(Constant.debarbankchannel_key);//清理缓存
            }
            return new RedirectResult($"/Prompt?state={backState}&msg={msg}&url={ubcurl}");
        }


        public string CustomerPictureUpdate(int UserId, int IdCardPhoto, int IdCardBackPhoto, int BankCardPhoto)
        {
            var result = YeepayDepository.customerPictureUpdate(UserId, (int)Logic.Platform.系统, IdCardPhoto, IdCardBackPhoto, BankCardPhoto);
            return JsonConvert.SerializeObject(result);
        }
    }
    
}