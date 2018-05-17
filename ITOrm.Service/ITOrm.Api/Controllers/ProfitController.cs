using ITOrm.Core.Utility.Helper;
using ITOrm.Host.BLL;
using ITOrm.Host.Models;
using ITOrm.Utility.Cache;
using ITOrm.Utility.Client;
using ITOrm.Utility.Const;
using ITOrm.Utility.ITOrmApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITOrm.Api.Controllers
{
    public class ProfitController : Controller
    {
        public static UserShareBLL userShareDao = new UserShareBLL();
        public static UserEventRecordBLL userEventDao = new UserEventRecordBLL();
        public static AccountRecordBLL accountRecordDao = new AccountRecordBLL();
        public static UsersBLL usersDao = new UsersBLL();
        public static PayRecordBLL payRecordDao = new PayRecordBLL();

        #region 邀请二维码
        /// <summary>
        /// 邀请二维码
        /// </summary>
        [HttpGet]
        public ActionResult QRcode(int cid,int UserId)
        {
           
            string url = Constant.CurrentApiHost + "itapi/invite/reg?u=" + ITOrm.Utility.Encryption.AESEncrypter.AESEncrypt(UserId.ToString(), Constant.SystemAESKey);
            //url = "http://api.sujintech.com/itapi/invite/reg?u=" + ITOrm.Utility.Encryption.AESEncrypter.AESEncrypt(UserId.ToString(), Constant.SystemAESKey);;
            //用户事件
            userEventDao.UserEventInit(cid, UserId, Ip.GetClientIp(), 1, "Profit", "QRcode", $"{{UserId:{UserId}}}");

            var bytes = QrCodeHelper.Instance.QrCodeCreate(url);
            return File(bytes, @"image/jpeg");
        }
        #endregion

        #region 分享
        /// <summary>
        /// 分享
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="UserId"></param>
        /// <param name="Soure"></param>
        /// <returns></returns>
        public string Share(int cid, int UserId, int Soure)
        {
            string url = Constant.CurrentApiHost + "itapi/invite/reg?u=" + ITOrm.Utility.Encryption.AESEncrypter.AESEncrypt(UserId.ToString(), Constant.SystemAESKey);

            UserShare us = new UserShare();
            us.Title = "邀请好友";
            us.ShareUrl = url;
            us.ImageUrl = "";
            us.Context = "邀请好友";
            us.Platform = cid;
            us.Ip = Ip.GetClientIp();
            us.Soure = Soure;
            us.UserId = UserId;
            int num = userShareDao.Insert(us);
            JObject data = new JObject();
            data["Title"] = us.Title;
            data["ShareUrl"] = us.ShareUrl;
            data["ImageUrl"] = us.ImageUrl;
            data["Context"] = us.Context;
            //用户事件
            userEventDao.UserEventInit(cid, UserId, Ip.GetClientIp(),num>0? 1 : 0, "Profit", "Share", $"{{UserId:{UserId},Soure:{Soure},num:{num}}}");


            return ApiReturnStr.getApiData(data);

        }
        #endregion


        #region 邀请列表

        public string InviteList(int UserId, int pageIndex = 1, int pageSize = 10)
        {
            int totalCount = 0;
            var listUser = usersDao.GetPaged(pageSize, pageIndex, out totalCount, "BaseUserId=@UserId",new { UserId},"order by UserId desc");
            JArray list = new JArray();
            foreach (var item in listUser)
            {
                JObject data = new JObject();
                data["Mobile"] = item.Mobile;
                data["CTime"] = item.CTime.ToString("yyyy-MM-dd HH:mm:ss");
                data["IsRealState"] = item.IsRealState;
                data["IsRealStateTxt"] = item.IsRealState==0 ? "未认证" : "已认证";
                data["RealName"]= item.IsRealState == 0 ? "无名氏" : item.RealName;
   
                list.Add(data);
            }
            return ApiReturnStr.getApiDataListByPage(list, totalCount, pageIndex, pageSize);
        }

        #endregion


        #region 收益列表
        public string IncomeList(int UserId, int pageIndex = 1, int pageSize = 10)
        {
            int totalCount = 0;
            //int TypeId = (int)Logic.AccountType.刷卡分润; 开通会员分润
            var listAccountRecord = accountRecordDao.GetPaged(pageSize, pageIndex, out totalCount, "UserId=@UserId and TypeId in(100,101,102)", new { UserId }, "order by ID desc");
            JArray list = new JArray();
            foreach (var item in listAccountRecord)
            {
                JObject data = new JObject();
                data["InOrOut"] = item.InOrOut==1?"+":"-";
                data["Amount"] = item.Amount.ToString("F2");
                data["CTime"] = item.CTime.ToString("yyyy-MM-dd HH:mm:ss");
                data["TypeId"] = item.TypeId;
                data["Service"] = ((Logic.AccountType)item.TypeId).ToString();

                data["RealName"] = "";
                data["Mobile"] = "";
                if (item.TypeId == (int)Logic.AccountType.收款分润)
                {
                    var pay = payRecordDao.Single(item.KeyId);
                    var user = usersDao.Single(pay.UserId);
                    data["RealName"] = user.RealName;
                    data["Mobile"] = user.Mobile;
                }
                else
                {
                    var user = usersDao.Single(item.KeyId);
                    if (user != null)
                    {
                        data["RealName"] = user.RealName;
                        data["Mobile"] = user.Mobile;
                    }
                }
                list.Add(data);
            }
            return ApiReturnStr.getApiDataListByPage(list, totalCount, pageIndex, pageSize);
        }

        public string IncomeTotal(int UserId)
        {
            JObject total = MemcachHelper.Get<JObject>(Constant.income_total_key + UserId, DateTime.Now.AddMinutes(5), () =>
            {
                JObject data = new JObject();
                decimal TotalIncome = ITOrm.Utility.Helper.DapperHelper.ExecScalarSql<decimal>($" SELECT ISNULL(SUM(Amount),0) FROM dbo.AccountRecord WHERE UserId={UserId} AND TypeId IN(100,101,102) ");
                decimal LastMounthIncome = ITOrm.Utility.Helper.DapperHelper.ExecScalarSql<decimal>($" SELECT ISNULL(SUM(Amount),0) FROM dbo.AccountRecord WHERE UserId={UserId} AND TypeId IN(100,101,102) and DATEDIFF(MONTH,CTime,GETDATE())=1 ");
                decimal CurrentMounthIncome = ITOrm.Utility.Helper.DapperHelper.ExecScalarSql<decimal>($" SELECT ISNULL(SUM(Amount),0) FROM dbo.AccountRecord WHERE UserId={UserId} AND TypeId IN(100,101,102) and DATEDIFF(MONTH,CTime,GETDATE())=0 ");
                data["TotalIncome"] = TotalIncome.ToString("F2");
                data["LastMounthIncome"] = LastMounthIncome.ToString("F2");
                data["CurrentMounthIncome"] = CurrentMounthIncome.ToString("F2");
                return data;
            });

            return ApiReturnStr.getApiData(total);

        }
        #endregion
    }
}