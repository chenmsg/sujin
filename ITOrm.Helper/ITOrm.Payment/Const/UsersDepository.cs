using ITOrm.Host.BLL;
using ITOrm.Host.Models;
using ITOrm.Payment.Yeepay;
using ITOrm.Utility.Const;
using ITOrm.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ITOrm.Utility.Log;

namespace ITOrm.Payment.Const
{
    //公共方法
    public class UsersDepository
    {
        public static UsersBLL userDao = new UsersBLL();
        public static YeepayUserBLL yeepayUserDao = new YeepayUserBLL();
        public static UserBankCardBLL userBankCardDao = new UserBankCardBLL();
        public static MasgetUserBLL masgetUserDao = new MasgetUserBLL();
        public static ShareProfitBLL shareProfitDao = new ShareProfitBLL();
        /// <summary>
        /// 升级Vip
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="VipType"></param>
        /// <param name="Platform"></param>
        /// <returns></returns>
        public static ResultModel UpgradeVip(int UserId,int VipType, int Platform)
        {
            var result = userDao.UpgradeVip(UserId, VipType, (int)Logic.Platform.系统);
            StringBuilder sb = new StringBuilder();
            if (result.backState == 0)
            {
               var result1= UpdateChannelVip(UserId, VipType, Platform);
                result.message = result1.message;
            }
            return result;


        }

        /// <summary>
        /// 清理缓存并修改第三方费率
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="VipType"></param>
        /// <param name="Platform"></param>
        /// <returns></returns>
        public static ResultModel UpdateChannelVip(int UserId, int VipType, int Platform)
        {
            ResultModel result = new ResultModel();
            StringBuilder sb = new StringBuilder();
            //清理缓存
            userDao.RemoveCache(UserId);
            sb.Append("系统费率修改成功；<br/>");
            decimal[] r = Constant.GetRate(0, (Logic.VipType)VipType);

            decimal rate1 = r[0];
            decimal rate3 = r[1];
            YeepayUser yUser = yeepayUserDao.Single("UserId=@UserId ", new { UserId });
            if (yUser != null)//存在易宝商户
            {
                var result1 = YeepayDepository.FeeSetApi(UserId, (int)Logic.Platform.系统, Enums.YeepayType.设置费率1, rate1.ToString("F4"));
                sb.Append("易宝交易费率(").Append(result1.rate).Append(")修改").Append(result1.backState == 0 ? "成功" : "失败").Append("；<br/>");
                var result3 = YeepayDepository.FeeSetApi(UserId, (int)Logic.Platform.系统, Enums.YeepayType.设置费率3, rate3.ToString("F0"));
                sb.Append("易宝结算费率(").Append(result3.rate).Append(")修改").Append(result3.backState == 0 ? "成功" : "失败").Append("；<br/>");
            }
            //查询是否有荣邦用户
            var masgetUserList = masgetUserDao.GetQuery("UserId=@UserId", new { UserId });
            if (masgetUserList != null && masgetUserList.Count > 0)
            {
                foreach (var item in masgetUserList)
                {
                    var result4 = ITOrm.Payment.Masget.MasgetDepository.SamenameUpdate(UserId, (int)Logic.Platform.系统, (Logic.ChannelType)item.TypeId, (Logic.VipType)VipType);
                    sb.Append((Logic.ChannelType)item.TypeId).Append("费率套餐修改").Append(result4.backState == 0 ? "成功" : "失败").Append("；<br/>");
                }
            }
            sb.Append("请仔细阅读以上通道是否成功，如出现失败，记得联系管理员；");
            result.message = sb.ToString();
            return result;
        }

        /// <summary>
        /// 交易成功时调用方法
        /// </summary>
        /// <param name="PayId"></param>
        /// <param name="UserId"></param>
        public static void NoticeSuccess(int PayId,int UserId)
        {
            try
            {
                Users user = userDao.Single(UserId);
                Logs.WriteLog($"user:{JsonConvert.SerializeObject(user)}", "d:\\Log\\", "NoticeSuccess");
                //分润
                var shareProfit = shareProfitDao.ShareProfit(PayId);
                Logs.WriteLog($"分润：PayId:{PayId},UserId:{UserId},shareProfit:{JsonConvert.SerializeObject(shareProfit)}", "d:\\Log\\", "NoticeSuccess");
                if (user.VipType == (int)Logic.VipType.普通用户)
                {
                    //自己检查是否刷卡达8万
                    //刷卡升级Vip检查
                    //var checkGiveVipResult = userDao.CheckGiveVip(UserId, (int)Logic.Platform.系统);
                    //Logs.WriteLog($"自我检查:PayId:{PayId},UserId:{UserId},checkGiveVipResult:{JsonConvert.SerializeObject(checkGiveVipResult)}", "d:\\Log\\", "NoticeSuccess");
                    //if (checkGiveVipResult.backState == 0)
                    //{
                    //    var uv = UpdateChannelVip(UserId, (int)Logic.VipType.VIP, (int)Logic.Platform.系统);
                    //    Logs.WriteLog($"自我第三方升级:PayId:{PayId},UserId:{UserId},UpdateChannelVip:{JsonConvert.SerializeObject(uv)}", "d:\\Log\\", "NoticeSuccess");
                    //}

                }
                if (user.BaseUserId > 0)
                {
                    Users BaseUser = userDao.Single(user.BaseUserId);
                    Logs.WriteLog($"BaseUser:{JsonConvert.SerializeObject(BaseUser)}", "d:\\Log\\", "NoticeSuccess");
                    if (BaseUser.VipType == (int)Logic.VipType.普通用户)
                    {
                        //邀请人需要检测 下线商户是否累计15万
                        //刷卡升级Vip检查
                        var checkGiveVipResult = userDao.CheckGiveVip(user.BaseUserId, (int)Logic.Platform.系统);
                        Logs.WriteLog($"邀请人检查:PayId:{PayId},user.BaseUserId:{user.BaseUserId},checkGiveVipResult:{JsonConvert.SerializeObject(checkGiveVipResult)}", "d:\\Log\\", "NoticeSuccess");
                        if (checkGiveVipResult.backState == 0)
                        {
                            var uv = UpdateChannelVip(user.BaseUserId, (int)Logic.VipType.VIP, (int)Logic.Platform.系统);
                            Logs.WriteLog($"邀请人第三方升级:PayId:{PayId},user.BaseUserId:{user.BaseUserId},UpdateChannelVip:{JsonConvert.SerializeObject(uv)}", "d:\\Log\\", "NoticeSuccess");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logs.WriteLog($"PayId:{PayId},UserId:{UserId},msg:{e.Message}", "d:\\Log\\", "NoticeSuccessError");
            }
            
            
        }
    }
}
