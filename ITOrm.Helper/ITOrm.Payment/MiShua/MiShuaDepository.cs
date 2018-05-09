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
        public static string mchNo = "100000";
        public static string AESkey = "1234567812345678";
        public static string Md5Key = "1234";
        public static string noticeUrl = "http://notice.sujintech.com";
        public static string MiShuaLogDic = "d:\\Log\\MiShua";//日志文件夹地址
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

        public static respPayDzeroModel PayDzero(int UbkId, int Platform,decimal Amount)
        {
            string LogDic = "订单支付";
            Logic.ChannelType channel = Logic.ChannelType.米刷;
            var ubk= userBankCardDao.Single(UbkId);

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
            model.orderDate = DateTime.Now.ToString("yyyyMMddHHmmss") ;
            model.tradeNo = requestId.ToString();
            model.notifyUrl = noticeUrl;
            model.callbackUrl = ITOrm.Utility.Const.Constant.CurrentApiHost + ""; ;
            model.payCardNo = ubk.BankCard;
            model.userToken = ubk.Mobile;
            model.accTel = withBank.Mobile;
            model.accName = user.RealName;
            model.accIdCard = user.IdCard;
            model.bankName = withBank.BankName;

            var bank=bankDao.Single("BankName=BankName",new { withBank.BankName});
            model.bankCode = bank.UnionCode;
            model.cardNo = withBank.BankCard;

            decimal[] r = Constant.GetRate(0, (Logic.VipType)user.VipType);
            model.downPayFee = r[0].ToString("F4");
            model.downDrawFee = r[1].ToString("F2");


            return null;
        }

    }
}
