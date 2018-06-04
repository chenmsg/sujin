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
        public ActionResult QRcode(int cid, int UserId)
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
            userEventDao.UserEventInit(cid, UserId, Ip.GetClientIp(), num > 0 ? 1 : 0, "Profit", "Share", $"{{UserId:{UserId},Soure:{Soure},num:{num}}}");


            return ApiReturnStr.getApiData(data);

        }
        #endregion

        #region 邀请规则

        public string InviteRule(int UserId)
        {
            JArray list = new JArray();

            string str = "1.SVIP每推荐一名用户开通SVIP可获得50元现金奖励；2.VIP每推荐一名用户开通SVIP可获得40元现金奖励；3.SVIP用户邀请好友享受高达0.1%的分佣；4.普通用户邀请的所有好友刷卡金额累加达到15万，将送您永久VIP！";

            string[] strs = str.Split('；');
            foreach (var item in strs)
            {
                JObject obj = new JObject();
                obj["line"] = item;
                list.Add(obj);
            }

            JArray list2 = new JArray();
            str = "1.普通用户邀请的所有好友刷卡金额累加达到15万，邀请人将获得永久VIP会员；";
            str += "2.用户刷卡达到8万元时，将获得永久VIP会员；";
            str += "3.普通用户邀请的好友为普通用户：-所有好友刷卡金额累加达到15万，邀请人将获得永久VIP会员；";
            str += "4.普通用户邀请好友开通VIP会员：-被邀请人开通VIP活动期间分润15，原分润为10；";
            str += "5.普通用户邀请好友开通SVIP会员：-被邀请人开通SVIP活动期间分润30，原分润为19；";
            str += "6.VIP会员邀请的好友为普通用户：-刷卡分润a元（a =（普通会员利率 - VIP费率）×刷卡金额；";
            str += "7.VIP会员邀请好友开通VIP会员：-被邀请人开通VIP分润活动期间20，原分润为15；";
            str += "8.VIP会员邀请好友开通SVIP会员： -被邀请人开通SVIP活动期间分润40，原分润为29；";
            str += "9.SVIP会员邀请好友为普通用户：-刷卡分润b元（b =（普通会员利率 - SVIP利率）×刷卡金额)；";
            str += "10.SVIP会员邀请好友开通VIP会员：-刷卡分润c元（c =（VIP - SVIP利率）×刷卡金额）-被邀请人开通VIP活动期间分润25，原分润为20；";
            str += "11.SVIP会员邀请好友开通SVIP会员：   -被邀请人开通SVIP活动期间分润50，原分润为39；"; 
            string[] strs2 = str.Split('；');
            foreach (var item in strs2)
            {
                JObject obj = new JObject();
                obj["line"] = item;
                if(!string.IsNullOrEmpty(item))
                list2.Add(obj);
            }

            JObject data = new JObject();
            data["simple"] = list;
            data["complex"] = list2;
            return ApiReturnStr.getApiData(data);
        }
        #endregion


        #region 邀请列表

        public string InviteList(int UserId, int pageIndex = 1, int pageSize = 10)
        {
            int totalCount = 0;
            var listUser = usersDao.GetPaged(pageSize, pageIndex, out totalCount, "BaseUserId=@UserId", new { UserId }, "order by UserId desc");
            JArray list = new JArray();
            if (listUser != null && listUser.Count > 0)
            {
                foreach (var item in listUser)
                {
                    JObject data = new JObject();
                    data["Mobile"] = item.Mobile;
                    data["CTime"] = item.CTime.ToString("yyyy-MM-dd HH:mm:ss");
                    data["IsRealState"] = item.IsRealState;
                    data["IsRealStateTxt"] = item.IsRealState == 0 ? "未认证" : "已认证";
                    data["RealName"] = item.IsRealState == 0 ? "无名氏" : item.RealName;

                    list.Add(data);
                }
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
            if (listAccountRecord != null && listAccountRecord.Count > 0)
            {
                foreach (var item in listAccountRecord)
                {
                    JObject data = new JObject();
                    data["InOrOut"] = item.InOrOut == 1 ? "+" : "-";
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

            total["LastMounthIncomeTip"] = "上月收益会扣除2元的手续费在每月的第一个工作日打款到您的结算卡中。";
            return ApiReturnStr.getApiData(total);

        }
        #endregion

        #region 升级VIP规则
        public string OpenVipRule(int UserId)
        {
            JObject data = new JObject();
            JArray list = new JArray();
            JObject vip1 = new JObject();
            vip1["Title"] = "SVIP会员";
            vip1["CurrentPrice"] = "99";
            vip1["OriginalPrice"] = "原价￥199";
            vip1["QRcode"] = Constant.StaticHost+ "upload/QRcode/129.png";
            list.Add(vip1);
            JObject vip2 = new JObject();
            vip2["Title"] = "VIP会员";
            vip2["CurrentPrice"] = "49";
            vip2["OriginalPrice"] = "原价￥99";
            vip2["QRcode"] = Constant.StaticHost + "upload/QRcode/129.png";
            list.Add(vip2);

            JObject vip3 = new JObject();
            vip3["Title"] = "升级SVIP";
            vip3["CurrentPrice"] = "69";
            vip3["OriginalPrice"] = "原价￥119";
            vip3["QRcode"] = Constant.StaticHost + "upload/QRcode/129.png";
            list.Add(vip3);

            data["listTitle"] = "微信扫码支付";
            data["list"] = list;
       


            JArray list2 = new JArray();
            string  str = "1.超低费率；2.邀请好友超高返佣；3.单单分润";
            string str2 = "SVIP有积分0.43%，无积分0.39% VIP有积分0.48%，无积分0.44%；好友开通SVIP一次性最高返现50元；好友每次刷卡，即可享受高达0.1%的利润返现";
            string[] strs2 = str.Split('；');
            string[] strs3 = str2.Split('；');
            for (int i = 0; i < strs2.Length; i++)
            {
                JObject obj = new JObject();
                if (!string.IsNullOrEmpty(strs2[i]))  obj["Title"] = strs2[i];
                if (!string.IsNullOrEmpty(strs3[i]))  obj["Context"] = strs3[i];
                list2.Add(obj);
            }
            data["list2Title"] = "VIP尊享权益";
            data["list2"] = list2;
            data["DingjiDaili"] = "客户经理：18610122058\n客户经理：18506120807\n微信客服：SJpay-op";


            data["Tips"] = "选择您需要开通会员对应的二维码并使用微信支付，备在注速金派已注册的账号，工作人员确认收款后立刻给您开通会员。您还可以将二维码保存在手机里面，方便支付。如有疑问请关注我们的微信公众号SJ派，联系在线客服";
            return ApiReturnStr.getApiData(data);
        }
        #endregion
    }
}