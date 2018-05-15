using ITOrm.Host.BLL;
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
namespace ITOrm.Manage.Controllers
{
    public class UsersController : Controller
    {
        public static UsersBLL userDao = new UsersBLL();
        public static YeepayUserBLL yeepayUserDao = new YeepayUserBLL();
        public static UserBankCardBLL userBankCardDao = new UserBankCardBLL();
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

            Users user = userDao.Single(UserId);
            //if (user.VipType == VipType)
            //{
            //    return new RedirectResult($"/Prompt?state={-100}&msg=当前用户类型相同，不可设置&url={url}");
            //}
            Logic.VipType vip = (Logic.VipType)VipType;
            YeepayUser yUser = yeepayUserDao.Single("UserId=@UserId",new { UserId});
            if (yUser == null)
            {
                return new RedirectResult($"/Prompt?state={-100}&msg=未开通商户&url={url}");
            }
            if (yUser.IsAudit != 1)
            {
                return new RedirectResult($"/Prompt?state={-100}&msg=请先审核&url={url}");
            }
            if((DateTime.Now -yUser.UTime).Seconds<5)
            if (yUser.RateState1 == 1)
            {
                    return new RedirectResult($"/Prompt?state={-100}&msg=审核时间和设置费率时间需间隔5秒&url={url}");
            }

            
            decimal[] r = Constant.GetRate(0, vip);

            decimal rate1 = r[0];
            decimal rate3 = r[1];


            var result1=  YeepayDepository.FeeSetApi(UserId, 1,  Enums.YeepayType.设置费率1,rate1.ToString("F4"));
            var result3 = YeepayDepository.FeeSetApi(UserId, 1, Enums.YeepayType.设置费率3, rate3.ToString("F0"));
            if (result1.backState == 0 && result3.backState == 0)
            {
                user.VipType = VipType;
                user.UTime = DateTime.Now;
                userDao.Update(user);
                return new RedirectResult($"/Prompt?state=0&msg=设置成功&url={url}");
            }
            return new RedirectResult($"/Prompt?state={-100}&msg=设置失败,{result1.message},{result3.message}&url={url}");
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

            var result=YeepayDepository.AuditMerchant(UserId, 1, Enums.AuditMerchant.SUCCESS, "审核成功");
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
                bank.Mobile = Mobile;
                bank.BankCard = BankCard;
                bank.BankCode = BankCode;
                bank.CVN2 = CVN2;
                bank.ExpiresYear = ExpiresYear;
                bank.ExpiresMouth = ExpiresMouth;

                if (bank.TypeId == 0)
                {
                    return new RedirectResult($"/Prompt?state=-100&msg=结算卡暂不支持修改&url={url}");
                }
                var result= userBankCardDao.Update(bank);
                backState = result ? 0 : -100;
                msg= result ? "修改成功" :"修改失败";
            }
  

            return new RedirectResult($"/Prompt?state={backState}&msg={msg}&url={ubcurl}");
        }
    }
}