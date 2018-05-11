using ITOrm.Core.Helper;
using ITOrm.Host.BLL;
using ITOrm.Host.Models;
using ITOrm.Utility.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ITOrm.Utility.Const;
using ITOrm.Utility.Client;

namespace ITOrm.Payment.MiShua
{
    public static class MiShuaDepository
    {

        #region 公共参数
        public static string mchNo = "100445";
        public static string AESkey = "G3HM53HM53HM3H3G";
        public static string Md5Key = "G54U45";
        public static string noticeUrl = ConfigHelper.GetAppSettings("MishuaNoticeUrl");
        public static string MiShuaLogDic = "d:\\Log\\MiShua";//日志文件夹地址
        public static string MiShuaDomain = "http://pay.mishua.cn/zhonlinepay/service/down/trans/";
        public static YeepayLogBLL yeepayLogDao = new YeepayLogBLL();
        public static YeepayLogParasBLL yeepayLogParasDao = new YeepayLogParasBLL();
        public static YeepayUserBLL yeepayUserDao = new YeepayUserBLL();
        public static UsersBLL usersDao = new UsersBLL();
        public static UserBankCardBLL userBankCardDao = new UserBankCardBLL();
        public static PayRecordBLL payRecordDao = new PayRecordBLL();
        public static WithDrawBLL withDrawDao = new WithDrawBLL();
        public static MasgetUserBLL masgetUserDao = new MasgetUserBLL();
        public static BankTreatyApplyBLL bankTreatyApplyDao = new BankTreatyApplyBLL();
        public static PayCashierBLL payCashierDao = new PayCashierBLL();
        public static BankBLL bankDao = new BankBLL();
        #endregion

        #region 无卡支付接口
        public static respModel< respPayDzeroModel> PayDzero(int UbkId, int Platform, decimal Amount)
        {
            string LogDic = "无卡支付接口";
            Logic.ChannelType channel = Logic.ChannelType.米刷;
            var ubk = userBankCardDao.Single(UbkId);

            var withBank = userBankCardDao.Single(" TypeId=0 and UserId=@UserId ", new { ubk.UserId });
            var user = usersDao.Single(ubk.UserId);
            //获取请求流水号
            int keyId = payRecordDao.Init(UbkId, Amount, Platform, Ip.GetClientIp(), (int)channel);
            //int keyId = payRecordDao.Init(bta.UserId, Amount, Platform, Ip.GetClientIp(), bta.BankCard);
            Logs.WriteLog($"创建支付记录：UserId:{ubk.UserId},Platform:{Platform},keyId:{keyId},Amount={Amount}", MiShuaLogDic, LogDic);

            int requestId = yeepayLogDao.Init((int)MiShua.Enums.MiShuaType.无卡支付接口, ubk.UserId, Platform, keyId, (int)channel);
            Logs.WriteLog($"获取请求流水号：UserId:{ubk.UserId},Platform:{Platform},requestId:{requestId},keyId:{keyId}", MiShuaLogDic, LogDic);

            //reqModel model = new reqModel();
            //model.mchNo = mchNo;
            //model.


            reqPayDzeroModel model = new reqPayDzeroModel();
            model.versionNo = "1";
            model.mchNo = mchNo;
            model.price = Amount.ToString("F2");
            model.description = "SJ商品";
            model.orderDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            model.tradeNo = requestId.ToString();
            model.notifyUrl = noticeUrl + "MiShuaNotice";
            model.callbackUrl = ITOrm.Utility.Const.Constant.CurrentApiHost + "itapi/pay/success?backState=0&message=支付成功"; ;
            model.payCardNo = ubk.BankCard;
            model.userToken = ubk.Mobile;
            model.accTel = withBank.Mobile;
            model.accName = user.RealName;
            model.accIdCard = user.IdCard;
            model.bankName = withBank.BankName;

            var bank = bankDao.Single("BankName=@BankName", new { withBank.BankName });
            model.bankCode = bank.UnionCode;
            model.cardNo = withBank.BankCard;

            decimal[] r = Constant.GetRate(0, (Logic.VipType)user.VipType);
            model.downPayFee = (r[0] * 1000M).ToString("F1");
            model.downDrawFee = r[1].ToString("F2");



            return Post<respPayDzeroModel>(requestId, "payDzero", JObject.FromObject(model), LogDic);
        }


        #endregion

        #region 无卡查询接口
        public static respModel<respCheckDzeroModel> CheckDzero(int orderno, Logic.Platform Platform)
        {
            string LogDic = "无卡查询接口";
            Logic.ChannelType channel = Logic.ChannelType.米刷;

            var yeelog = yeepayLogDao.Single(orderno);
            int requestId = yeepayLogDao.Init((int)MiShua.Enums.MiShuaType.无卡查询接口, yeelog.UserId, (int)Platform, orderno, (int)channel);
            Logs.WriteLog($"获取请求流水号：orderno:{orderno},Platform:{Platform},requestId:{requestId}", MiShuaLogDic, LogDic);

            //reqModel model = new reqModel();
            //model.mchNo = mchNo;
            //model.


            reqCheckDzeroModel model = new reqCheckDzeroModel();
            model.versionNo = "1";
            model.mchNo = mchNo;
            model.tradeNo = orderno.ToString();
            model.transNo = "";

            return Post<respCheckDzeroModel>(requestId, "checkDzero", JObject.FromObject(model), LogDic);
        }

        #endregion


        #region 公共方法

        public static respModel<T> Post<T>(int requestId, string cmd,JObject model,string logPath)
        {
            bool flag = false;
            string result = string.Empty;

            string json = JsonConvert.SerializeObject(model);

            string AESData = ITOrm.Payment.MiShua.AES.Encrypt(json, AESkey, "0102030405060708");
            string sign = ITOrm.Utility.Encryption.SecurityHelper.GetMD5String(AESData + Md5Key).ToUpper();
            reqModel req = new reqModel();
            req.mchNo = mchNo;
            req.payload = AESData;
            req.sign = sign;


            //日志记录
            JObject data = new JObject();
            data["ExpressData"] = model;
            data["EncryptionData"] = JObject.FromObject( req);

            //请求前日志记录
            Logs.WriteLog("提交参数：" + data.ToString(), MiShuaLogDic, logPath);
            yeepayLogParasDao.Init(requestId, data.ToString(), 0);




            //执行请求
            int responseState = ITOrm.Utility.Client.HttpHelper.HttpPostJson(MiShuaDomain + cmd, data["EncryptionData"].ToString(), Encoding.UTF8, out result);
            if (responseState != 200)
            {
                result = $"{{\"mchNo\":\"{mchNo}\", \"state\":\"Failed\",\"message\":\"网络请求失败\",\"code\":\"-99\",\"payload\":\"网络请求失败\",\"sign\":\"\"}}";
            }

            //返回后日志记录
            Logs.WriteLog("返回参数：" + result, MiShuaLogDic, logPath);
            yeepayLogParasDao.Init(requestId, result, 1);

            //易宝日志状态更新
            respModel<T> resp = JsonConvert.DeserializeObject<respModel<T>>(result);
            yeepayLogParasDao.Init(requestId, JsonConvert.SerializeObject(resp), 1);

            flag = yeepayLogDao.UpdateState(requestId, resp.code, resp.message, resp.backState == 0 ? 1 : resp.backState);
            Logs.WriteLog($"易宝日志状态更新：requestId:{requestId},code:{resp.code},message:{ resp.message},State:{flag}", MiShuaLogDic, logPath);
            return JsonConvert.DeserializeObject<respModel<T>>(result);


        }

        #endregion
    }
}
