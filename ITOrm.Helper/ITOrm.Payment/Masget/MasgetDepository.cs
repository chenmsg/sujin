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

namespace ITOrm.Payment.Masget
{
    public class MasgetDepository
    {

        public static string MasgetDomain = ConfigHelper.GetAppSettings("MasgetDomain");
        public static string MasgetNoticeUrl = ConfigHelper.GetAppSettings("MasgetNoticeUrl");
        public static string[] MasgetSecretKey= new string[] { "1ZON8DNJAtzG6fgF", "e1m0vkM2bxljrULc", "RSAHIJ29xUto1lHI" };//积分密钥
        public static string[] MasgetSession = new string[] { "gjkevhr1gfczuk6drhl7cmd4ipvg2dtj", "if56kewtmmiim5cfhq6mempppjeta98p", "dytdkzabxqvyigmqd6u0acrgv13sj738" };//session
        public static string[] MasgetAppid = new string[] { "402857315", "402857333", "403433027" };//Appid
       
        public static string MasgetLogDic = "d:\\Log\\Masget";//日志文件夹地址
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


        #region 快速进件（相当于开户） masget.webapi.com.subcompany.add
        public static respMasgetModel<respSubcompanyAddModel> SubcompanyAdd(int UserId, int Platform, Logic.ChannelType chanel)
        {
            string LogDic = "快速进件";

            UserBankCard ubk = userBankCardDao.Single("UserId=@UserId and State=1 and TypeId=0  ", new {UserId });
            Users user = usersDao.Single(UserId);
            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Masget.Enums.MasgetType.快速进件, UserId, Platform,0,(int)chanel);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId},chanel:{chanel}", MasgetLogDic, LogDic);
            string wu ="";
            switch (chanel)
            {
                case Logic.ChannelType.荣邦科技积分:
                    break;
                case Logic.ChannelType.荣邦科技无积分:
                    wu = "(无积分)";
                    break;
                case Logic.ChannelType.荣邦3:
                    wu = "(积分3)";
                    break;
                default:
                    break;
            }
            reqSubcompanyAddModel model = new reqSubcompanyAddModel();
            model.companyname = $"{user.RealName}({user.UserId}){wu}";
            model.companycode = user.UserId.ToString();
            model.accountname = user.RealName;
            model.bankaccount = ubk.BankCard;
            //model.accounttype = "1";
            //model.bankcardtype = "1";
            model.mobilephone = ubk.Mobile;
            model.idcardno = user.IdCard;
            model.address = "无";
            model.loginname = ubk.Mobile;


            
            Logic.VipType vip = (Logic.VipType)user.VipType;
            var optionFee= SelectOptionFee(chanel, vip);
            model.ratecode = optionFee.ratecode;

            model.bank = "";
            model.bankcode = "";
            //model.accountrule = "1";
            string json = Order<reqSubcompanyAddModel>(model);
            var resp = PostUrl<respSubcompanyAddModel>(requestId, "masget.webapi.com.subcompany.add", json, LogDic,chanel);
            if (resp.backState == 0)
            {
                //创建用户
                var mid= masgetUserDao.Init(UserId, resp.data.appid, resp.data.session, resp.data.secretkey,resp.data.companyid,Platform,(int)chanel, optionFee.Rate1, optionFee.Rate3);
                Logs.WriteLog($"创建荣邦渠道用户：UserId:{UserId},Platform:{Platform},requestId:{requestId},MasgetUserId:{mid},Rate1{optionFee.Rate1},Rate3:{optionFee.Rate3}", MasgetLogDic, LogDic);
                if (chanel == Logic.ChannelType.荣邦3)
                {
                    masgetUserDao.UpdateState(UserId, (int)chanel, 1);
                    Logs.WriteLog($"默认设置已入驻：UserId:{UserId},chanel:{Platform},MasgetUserId:{mid}", MasgetLogDic, LogDic);
                }
                
            }
            return resp;
        }

        #endregion

        #region 子商户秘钥下载  masget.webapi.com.subcompany.get
        public static respMasgetModel<respSubcompanyGetModel> SubcompanyGet(int UserId, int Platform, Logic.ChannelType chanel)
        {
            string LogDic = "子商户秘钥下载";
            //MasgetUser mUser = masgetUserDao.Single(" UserId=@UserId", new { UserId });
            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Masget.Enums.MasgetType.子商户秘钥下载, UserId, Platform, 0, (int)chanel);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId}", MasgetLogDic, LogDic);

            reqSubcompanyGetModel model = new reqSubcompanyGetModel();
            model.companycode = UserId.ToString();
            model.mobilephone = "";
            //model.accountrule = "1";
            string json = Order<reqSubcompanyGetModel>(model);
            var resp = PostUrl<respSubcompanyGetModel>(requestId, "masget.webapi.com.subcompany.get", json, LogDic, chanel);
            if (resp.backState == 0)
            {
                //更新用户
                //flag = masgetUserDao.UpdateState(UserId, 1);
                Logs.WriteLog($"子商户秘钥下载：UserId:{UserId},Platform:{Platform},requestId:{requestId},flag:{flag}", MasgetLogDic, LogDic);
            }
            return resp;
        }
        #endregion

        #region 商户通道入驻接口 masget.pay.compay.router.samename.open
        public static respMasgetModel<object> SamenameOpen(int UserId, int Platform,Logic.ChannelType chanel)
        {
            string LogDic = "商户通道入驻";
            int TypeId = (int)chanel;
            MasgetUser mUser = masgetUserDao.Single(" UserId=@UserId and TypeId=@TypeId", new { UserId ,TypeId});
            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Masget.Enums.MasgetType.商户通道入驻接口, UserId, Platform,0, (int)chanel);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId}", MasgetLogDic, LogDic);

            reqSamenameOpenModel model = new reqSamenameOpenModel();
            model.companyid = mUser.CompanyId;
            //model.accountrule = "1";
            string json = Order<reqSamenameOpenModel>(model);
            var resp = PostUrl<object>(requestId, "masget.pay.compay.router.samename.open", json, LogDic,chanel);
            if (resp.backState == 0)
            {
                //更新用户
                flag= masgetUserDao.UpdateState(UserId,TypeId, 1);
                Logs.WriteLog($"入驻状态更新：UserId:{UserId},Platform:{Platform},requestId:{requestId},flag:{flag}", MasgetLogDic, LogDic);
            }
            return resp;
        }
        #endregion

        #region 申请开通快捷协议 masget.pay.collect.router.treaty.apply
        public static respMasgetModel<respTreatyApplyModel> TreatyApply(int UbkId, int Platform, Logic.ChannelType chanel)
        {
            string LogDic = "申请开通快捷协议";

            UserBankCard ubk = userBankCardDao.Single(UbkId);
            int TypeId = (int)chanel;
            MasgetUser mUser = masgetUserDao.Single(" UserId=@UserId and TypeId=@TypeId", new { ubk.UserId, TypeId });
            if (mUser == null)
            {
                Logs.WriteLog($"未查到用户：UserId:{ubk.UserId},Platform:{Platform},ubkId:{UbkId}", MasgetLogDic, LogDic);
                return new respMasgetModel<respTreatyApplyModel>() { ret=-9999,message="用户未进件"};
            }
            Users user = usersDao.Single(ubk.UserId);
            bool flag = false;

            //创建快捷协议记录
            var btaId= bankTreatyApplyDao.Init(ubk.UserId, UbkId,ubk.BankCard,ubk.Mobile, Platform, (int)chanel);
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Masget.Enums.MasgetType.申请开通快捷协议, ubk.UserId, Platform, btaId, (int)chanel);
            Logs.WriteLog($"获取请求流水号：UserId:{ubk.UserId},Platform:{Platform},requestId:{requestId},ubkId:{UbkId},btaId:{btaId}", MasgetLogDic, LogDic);

            reqTreatyApplyModel model = new reqTreatyApplyModel();
            model.accountname = user.RealName;
            model.accounttype = "1";
            model.bankaccount = ubk.BankCard;
            model.certificatetype = "1";
            model.certificateno = user.IdCard;
            model.mobilephone = ubk.Mobile;
            model.collecttype = "1";
            model.startdate = "";
            model.enddate = "";
            model.bankcode = "";
            model.bank = "";
            model.cvv2 = ubk.CVN2.ToString();
            model.expirationdate = $"{ubk.ExpiresMouth}{ubk.ExpiresYear}";
            model.fronturl = "";
            model.backurl = "";
            //model.accountrule = "1";
            string json = Order<reqTreatyApplyModel>(model);
            var resp = PostUrl<respTreatyApplyModel>(requestId, "masget.pay.collect.router.treaty.apply", json, LogDic, chanel, mUser.ID);
            if (resp.backState == 0)
            {
                //更新快捷协议
                flag = bankTreatyApplyDao.UpdateTreatycode(btaId,resp.data.treatycode,resp.data.smsseq,1);// 1等待确认
                Logs.WriteLog($"申请开通快捷协议：UserId:{ubk.UserId},Platform:{Platform},requestId:{requestId},flag:{flag}", MasgetLogDic, LogDic);
            }
            return resp;
        }
        #endregion

        #region 确认开通快捷协议 masget.pay.collect.router.treaty.confirm
        public static respMasgetModel<respTreatyConfirmModel> TreatyConfirm(int UbkId,string Authcode, int Platform, Logic.ChannelType chanel)
        {
            string LogDic = "确认开通快捷协议";

            int TypeId = (int)chanel;
            BankTreatyApply bta = bankTreatyApplyDao.Single(" UbkId=@UbkId and ChannelType=@TypeId", new { UbkId, TypeId });
            if (bta == null)
            {
                Logs.WriteLog($"未查到协议记录：UserId:{bta.UserId},Platform:{Platform},BtaId:{bta.ID}", MasgetLogDic, LogDic);
                return new respMasgetModel<respTreatyConfirmModel>() { ret = -9999, message = "未查到协议记录" };
            }
            if (bta.State == 2)
            {
                Logs.WriteLog($"快捷协议已开通：UserId:{bta.UserId},Platform:{Platform},BtaId:{bta.ID}", MasgetLogDic, LogDic);
                return new respMasgetModel<respTreatyConfirmModel>() { ret = -9999, message = "快捷协议已开通" };
            }
            MasgetUser mUser = masgetUserDao.Single(" UserId=@UserId and TypeId=@TypeId", new { bta.UserId, TypeId });
            if (mUser == null)
            {
                Logs.WriteLog($"未查到用户：UserId:{bta.UserId},Platform:{Platform},BtaId:{bta.ID}", MasgetLogDic, LogDic);
                return new respMasgetModel<respTreatyConfirmModel>() { ret = -9999, message = "用户未进件" };
            }
            Users user = usersDao.Single(bta.UserId);
            bool flag = false;

            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Masget.Enums.MasgetType.确认开通快捷协议, bta.UserId, Platform, bta.ID, (int)chanel);
            Logs.WriteLog($"获取请求流水号：UserId:{bta.UserId},Platform:{Platform},requestId:{requestId},BtaId:{bta.ID}", MasgetLogDic, LogDic);

            reqTreatyConfirmModel model = new reqTreatyConfirmModel();
            model.authcode = Authcode;
            model.smsseq = bta.Smsseq;
            model.treatycode = bta.Treatycode;

            //model.accountrule = "1";
            string json = Order<reqTreatyConfirmModel>(model);
            var resp = PostUrl<respTreatyConfirmModel>(requestId, "masget.pay.collect.router.treaty.confirm", json, LogDic, chanel, mUser.ID);
            if (resp.backState == 0)
            {
                //更新快捷协议
                flag = bankTreatyApplyDao.UpdateTreatycode(bta.ID, resp.data.treatycode, bta.Smsseq, 2);// 1开通成功
                Logs.WriteLog($"确认开通快捷协议：UserId:{bta.UserId},Platform:{Platform},requestId:{requestId},btaId:{bta.ID},flag:{flag}", MasgetLogDic, LogDic);
                //更新银行卡对应通道可用
                UserBankCard ubk = userBankCardDao.Single(UbkId);
                ubk.State = 1;
                ubk.UTime = DateTime.Now;
                ubk.RelationId += TypeId + ",";
                flag= userBankCardDao.Update(ubk);
                Logs.WriteLog($"更新银行卡对应通道可用：UserId:{bta.UserId},Platform:{Platform},requestId:{requestId},UbkId:{UbkId},flag:{flag}", MasgetLogDic, LogDic);
            }
            return resp;
        }
        #endregion

        #region 修改协议信息 masget.pay.collect.router.treaty.modify
        public static respMasgetModel<respTreatyModifyModel> TreatyModify(int UbkId, string cvv2,string expirationYear,string expirationMonuth, int Platform, Logic.ChannelType chanel)
        {
            string LogDic = "修改协议信息";

            int TypeId = (int)chanel;
            BankTreatyApply bta = bankTreatyApplyDao.Single(" UbkId=@UbkId and ChannelType=@TypeId", new { UbkId, TypeId });
            if (bta == null)
            {
                Logs.WriteLog($"未查到协议记录：UserId:{bta.UserId},Platform:{Platform},BtaId:{bta.ID}", MasgetLogDic, LogDic);
                return new respMasgetModel<respTreatyModifyModel>() { ret = -9999, message = "未查到协议记录" };
            }
            if (bta.State != 2)
            {
                Logs.WriteLog($"快捷协议未开通：UserId:{bta.UserId},Platform:{Platform},BtaId:{bta.ID}", MasgetLogDic, LogDic);
                return new respMasgetModel<respTreatyModifyModel>() { ret = -9999, message = "快捷协议未开通" };
            }
            MasgetUser mUser = masgetUserDao.Single(" UserId=@UserId and TypeId=@TypeId", new { bta.UserId, TypeId });
            if (mUser == null)
            {
                Logs.WriteLog($"未查到用户：UserId:{bta.UserId},Platform:{Platform},BtaId:{bta.ID}", MasgetLogDic, LogDic);
                return new respMasgetModel<respTreatyModifyModel>() { ret = -9999, message = "用户未进件" };
            }
            Users user = usersDao.Single(bta.UserId);
            bool flag = false;

            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Masget.Enums.MasgetType.修改协议信息, bta.UserId, Platform, bta.ID, (int)chanel);
            Logs.WriteLog($"获取请求流水号：UserId:{bta.UserId},Platform:{Platform},requestId:{requestId},BtaId:{bta.ID}", MasgetLogDic, LogDic);

            reqTreatyModifyModel model = new reqTreatyModifyModel();
            model.cvv2 = cvv2;
            model.expirationdate = expirationMonuth + expirationYear;
            model.treatycode = bta.Treatycode;

            //model.accountrule = "1";
            string json = Order<reqTreatyModifyModel>(model);
            var resp = PostUrl<respTreatyModifyModel>(requestId, "masget.pay.collect.router.treaty.modify", json, LogDic, chanel, mUser.ID);
            if (resp.backState == 0)
            {
                //更新银行卡对应通道可用
                //UserBankCard ubk = userBankCardDao.Single(UbkId);
                //ubk.UTime = DateTime.Now;
                //ubk.CVN2 = cvv2;
                //ubk.ExpiresYear = expirationYear;
                //ubk.ExpiresMouth = expirationMonuth;
                //flag = userBankCardDao.Update(ubk);
                //Logs.WriteLog($"更新银行卡信息：UserId:{bta.UserId},Platform:{Platform},requestId:{requestId},UbkId:{UbkId},flag:{flag}", MasgetLogDic, LogDic);
            }
            return resp;
        }
        #endregion

        #region 查询快捷协议 masget.pay.compay.router.samename.update 
        public static respMasgetModel<respTreatyQueryModel> TreatyQuery( int Platform, Logic.ChannelType chanel,string bankaccount)
        {
            respMasgetModel<respTreatyQueryModel> tip = new respMasgetModel<respTreatyQueryModel>();
            string LogDic = "查询快捷协议";
            UserBankCard ubk = userBankCardDao.Single("BankCard=@bankaccount",new { bankaccount });

            if (ubk == null)
            {
                tip.ret = -100;
                tip.message = "未查到卡记录";
                return tip;
            }
            MasgetUser mUser = masgetUserDao.Single(" UserId=@UserId", new { ubk.UserId });
            if (mUser == null)
            {
                tip.ret = -100;
                tip.message = "用户未开户";
                return tip;
            }
            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Masget.Enums.MasgetType.查询快捷协议, ubk.UserId, Platform, 0, (int)chanel);
            Logs.WriteLog($"获取请求流水号：UserId:{ ubk.UserId},Platform:{Platform},requestId:{requestId}", MasgetLogDic, LogDic);
            reqMasgetModel req = new reqMasgetModel();
            req.chanel = chanel;
            reqTreatyQueryModel model = new reqTreatyQueryModel();
            model.pcompanyid = req.appid;
            model.companyid = mUser.CompanyId;
            model.bankaccount = bankaccount;
            model.treatycode = "";
            //model.accountrule = "1";
            string json = Order<reqTreatyQueryModel>(model);
            var resp = PostUrl<respTreatyQueryModel>(requestId, "masget.pay.compay.router.samename.update", json, LogDic, chanel);
            if (resp.backState == 0)
            {
                
            }
            return resp;
        }
        #endregion

        #region 订单支付(后台) masget.pay.compay.router.back.pay
        public static respMasgetModel<respBackPayModel> BackPay(int UbkId,decimal Amount, int Platform, Logic.ChannelType chanel)
        {
            string LogDic = "订单支付";

            int TypeId = (int)chanel;
            BankTreatyApply bta = bankTreatyApplyDao.Single(" UbkId=@UbkId and ChannelType=@TypeId", new { UbkId, TypeId });

            MasgetUser mUser = masgetUserDao.Single(" UserId=@UserId and TypeId=@TypeId", new { bta.UserId, TypeId });
            if (mUser == null)
            {
                Logs.WriteLog($"未查到用户：UserId:{bta.UserId},Platform:{Platform},BtaId:{bta.ID}", MasgetLogDic, LogDic);
                return new respMasgetModel<respBackPayModel>() { ret = -9999, message = "用户未进件" };
            }
            Users user = usersDao.Single(bta.UserId);
            bool flag = false;

            //获取请求流水号
            int keyId = payRecordDao.Init(UbkId, Amount, Platform, Ip.GetClientIp(), (int)chanel);
            //int keyId = payRecordDao.Init(bta.UserId, Amount, Platform, Ip.GetClientIp(), bta.BankCard);
            Logs.WriteLog($"创建支付记录：UserId:{bta.UserId},Platform:{Platform},keyId:{keyId},Amount={Amount}", MasgetLogDic, LogDic);
            
            int requestId = yeepayLogDao.Init((int)Masget.Enums.MasgetType.订单支付, bta.UserId, Platform, keyId, (int)chanel);
            Logs.WriteLog($"获取请求流水号：UserId:{bta.UserId},Platform:{Platform},requestId:{requestId},keyId:{keyId}", MasgetLogDic, LogDic);

            reqBackPayModel model = new reqBackPayModel();
            model.ordernumber = requestId.ToString();
            model.body = "SJ商品购买";
            model.amount = (Amount*100).ToString("F0");
            model.businesstype = "1001";
            model.paymenttypeid = "25";
            model.backurl = MasgetNoticeUrl+ "backpayNotice";
            var ext =new PayExtraParamsModel();
            ext.password = "";
            ext.authcode = "";
            ext.treatycode = bta.Treatycode;
            model.payextraparams = Order<PayExtraParamsModel>(ext);
            //model.accountrule = "1";
            string json = Order<reqBackPayModel>(model);
            var resp = PostUrl<respBackPayModel>(requestId, "masget.pay.compay.router.back.pay", json, LogDic, chanel, mUser.ID);

            if (resp.backState == 0)
            {
                //添加收银记录表
                PayCashier payCash = new PayCashier();
                payCash.ChannelType = (int)chanel;
                payCash.Value = JsonConvert.SerializeObject(resp.data);
                payCash.LogId = requestId;
                payCash.State = 0;
                payCash.UserId = bta.UserId;
                payCash.PayRecordId = keyId;
                payCash.UbkId = UbkId;
                int payCId = payCashierDao.Insert(payCash);
                Logs.WriteLog($"添加收银记录表：UserId:{bta.UserId},Platform:{Platform},requestId:{requestId},payCId:{payCId}", MasgetLogDic, LogDic);
                resp.url =$"{Constant.CurrentApiHost}itapi/pay/cashier?payid={payCId}";
            }

            return resp;
        }
        #endregion

        #region 确认支付 masget.pay.compay.router.confirmpay 
        public static respMasgetModel<respPayConfirmpayModel> PayConfirmpay(int PayId,string code ,int Platform, Logic.ChannelType chanel)
        {
            string LogDic = "确认支付";

            int TypeId = (int)chanel;
            PayCashier pay = payCashierDao.Single(PayId);
            respBackPayModel respBP = JsonConvert.DeserializeObject<respBackPayModel>(pay.Value);


            MasgetUser mUser = masgetUserDao.Single(" UserId=@UserId and TypeId=@TypeId", new { pay.UserId, TypeId });
            if (mUser == null)
            {
                Logs.WriteLog($"未查到用户：UserId:{pay.UserId},Platform:{Platform},PayId:{pay.ID}", MasgetLogDic, LogDic);
                return new respMasgetModel<respPayConfirmpayModel>() { ret = -9999, message = "用户未进件" };
            }
            Users user = usersDao.Single(pay.UserId);
            bool flag = false;

            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Masget.Enums.MasgetType.确认支付, pay.UserId, Platform, pay.PayRecordId, (int)chanel);
            Logs.WriteLog($"获取请求流水号：UserId:{pay.UserId},Platform:{Platform},requestId:{requestId},keyId:{pay.PayRecordId}", MasgetLogDic, LogDic);

            reqPayConfirmpayModel model = new reqPayConfirmpayModel();
            model.authcode = code;
            model.ordercode = respBP.ordercode;
            //model.accountrule = "1";
            string json = Order<reqPayConfirmpayModel>(model);
            var resp = PostUrl<respPayConfirmpayModel>(requestId, "masget.pay.compay.router.confirmpay", json, LogDic, chanel, mUser.ID);

            if (resp.backState == 0)
            {
                //成功修改支付表状态
                pay.State = 1;
                pay.UTime = DateTime.Now;
                flag= payCashierDao.Update(pay);
                Logs.WriteLog($"收银表状态修改：UserId:{pay.UserId},Platform:{Platform},requestId:{requestId},PayCashierId:{PayId},flag={flag}", MasgetLogDic, LogDic);
                var payRecord = payRecordDao.Single(pay.PayRecordId);
                payRecord.State = 5;//等待回调
                payRecord.UTime = DateTime.Now;
                flag= payRecordDao.Update(payRecord);
                Logs.WriteLog($"收款记录修改：UserId:{pay.UserId},Platform:{Platform},requestId:{requestId},PayRecordId:{pay.PayRecordId},flag={flag}", MasgetLogDic, LogDic);
            }

            return resp;
        }
        #endregion

        #region  查询交易订单 masget.pay.compay.router.paymentjournal.get
        public static respMasgetModel<respPaymentjournalGetModel> PaymentjournalGet(int rid, int Platform, Logic.ChannelType chanel)
        {
            string LogDic = "查询交易订单";
            var yeepaylog = yeepayLogDao.Single(rid);
            int TypeId = (int)chanel;
            MasgetUser mUser = masgetUserDao.Single(" UserId=@UserId and typeId=@TypeId", new { yeepaylog.UserId , TypeId });
            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Masget.Enums.MasgetType.查询交易订单, yeepaylog.UserId, Platform, 0, (int)chanel);
            Logs.WriteLog($"获取请求流水号：UserId:{yeepaylog.UserId},Platform:{Platform},requestId:{requestId}", MasgetLogDic, LogDic);

            reqPaymentjournalGetModel model = new reqPaymentjournalGetModel();
            model.companyid = mUser.CompanyId;
            model.ordernumber = rid.ToString();
            //model.accountrule = "1";
            string json = Order<reqPaymentjournalGetModel>(model);
            var resp = PostUrl<respPaymentjournalGetModel>(requestId, "masget.pay.compay.router.paymentjournal.get", json, LogDic, chanel);
            if (resp.backState == 0)
            {

            }
            return resp;
        }
        #endregion

        #region 修改同名进出商户费率 masget.pay.compay.router.samename.update
        public static respMasgetModel<object> SamenameUpdate(int UserId, int Platform, Logic.ChannelType chanel,Logic.VipType vip)
        {
            string LogDic = "修改同名进出商户费率";
            int TypeId = (int)chanel;
            MasgetUser mUser = masgetUserDao.Single(" UserId=@UserId and TypeId=@TypeId", new { UserId, TypeId });
            if (mUser == null)
            {
                return new respMasgetModel<object>() { ret = -100, message = "用户不存在" };
            }
            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Masget.Enums.MasgetType.修改同名进出商户费率, UserId, Platform, 0, (int)chanel);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId}", MasgetLogDic, LogDic);

            var optionFee = SelectOptionFee(chanel, vip);
            reqSamenameUpdateModel model = new reqSamenameUpdateModel();
            model.companyid = mUser.CompanyId;
            model.ratecode = optionFee.ratecode;
            model.paymenttypeid = "25";
            model.subpaymenttypeid = "25";
            //model.accountrule = "1";
            string json = Order<reqSamenameUpdateModel>(model);
            var resp = PostUrl<object>(requestId, "masget.pay.compay.router.samename.update", json, LogDic, chanel);
            if (resp.backState == 0)
            {

                //更新用户
                mUser.Rate1 = optionFee.Rate1;
                mUser.Rate3 = optionFee.Rate3;
                mUser.UTime = DateTime.Now;
                flag = masgetUserDao.Update(mUser);
                Logs.WriteLog($"修改费率：UserId:{UserId},Platform:{Platform},requestId:{requestId},flag:{flag},Rate1{ optionFee.Rate1}, Rate3:{ optionFee.Rate1}", MasgetLogDic, LogDic);
            }
            return resp;
        }
        #endregion

        #region Base
        /// <summary>
        /// 公共方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestId"></param>
        /// <param name="action"></param>
        /// <param name="json"></param>
        /// <param name="logPath"></param>
        /// <returns></returns>
        public static respMasgetModel<T> PostUrl<T>(int requestId, string action, string json, string logPath, Logic.ChannelType chanel,int MasgetId=0)
        {

            try
            {
                bool flag = false;
                string result = string.Empty;
                reqMasgetModel req = new reqMasgetModel();
                req.chanel = chanel;
                req.method = action;
                req.target_appid = "";
                req.dataExpress = json;
                req.MasgetID = MasgetId;
                req.data = AES.Encrypt(json, req.secretkey, req.secretkey);
                //请求前日志记录
                Logs.WriteLog("提交参数：" + JsonConvert.SerializeObject(req), MasgetLogDic, logPath);
                yeepayLogParasDao.Init(requestId, JsonConvert.SerializeObject(req), 0);

                //拼接所有传递参数
                //公共字段
                StringBuilder strPostPara = new StringBuilder();
                PropertyInfo[] pis = typeof(reqMasgetModel).GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    object keyName = pi.GetValue(req, null);
                    if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()) && keyName.ToString() != "dataExpress"&&keyName.ToString()!= "MasgetID"&&keyName.ToString()!= "mUser")
                    {
                        strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(req, null));
                    }
                }


                //执行请求
                int responseState = ITOrm.Utility.Client.HttpHelper.HttpPost(MasgetDomain, strPostPara.ToString(), Encoding.UTF8, out result);
                if (responseState != 200)
                {
                    result=$"{{ \"ret\":\"{responseState}\", \"message\":\"{result}\",\"data\":null  }}";
                }
                //返回后日志记录
                Logs.WriteLog("返回参数：" + result, MasgetLogDic, logPath);
                yeepayLogParasDao.Init(requestId, result, 1);

                ////易宝日志状态更新
                respMasgetModel<T> resp = JsonConvert.DeserializeObject<respMasgetModel<T>>(result);
                flag = yeepayLogDao.UpdateState(requestId, resp.backState.ToString(), resp.message, resp.backState == 0 ? 1 : resp.backState);
                Logs.WriteLog($"易宝日志状态更新：requestId:{requestId},ret:{resp.ret},message:{ resp.message},State:{flag}", MasgetLogDic, logPath);
                return JsonConvert.DeserializeObject<respMasgetModel<T>>(result);
            }
            catch (Exception e)
            {
                return JsonConvert.DeserializeObject<respMasgetModel<T>>($"{{ \"ret\":\"-1000\", \"message\":\"{e.Message}\"  }}");
            }

        }


        /// <summary>
        /// 按a-z排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string Order<T>(T model)
        {
            SortedDictionary<string, string> dicArrayPre = new SortedDictionary<string, string>();
            PropertyInfo[] pis = typeof(T).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    dicArrayPre.Add(pi.Name, pi.GetValue(model, null).ToString());
                    //strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            var diclist = ITOrm.Utility.Encryption.EncryptionHelper.FilterPara(dicArrayPre);
            JObject data = new JObject();
            foreach (var item in diclist)
            {
                data[item.Key] = item.Value;
            }
            return data.ToString();
        }

        //顶级 vip  SVip  普通 
        static string[] feecode1 = new string[] { "178906", "232797", "630793", "090463", "897622" };//荣邦积分
        static string[] feecode2 = new string[] { "627206", "627206", "238716", "842660", "886778" };//荣邦(无积分)  boos没仔细分好
        static string[] feecode3 = new string[] { "406591",  "502114", "521467", "556151", "704252" };//荣邦3
        public static OptionFee SelectOptionFee(Logic.ChannelType chanel, Logic.VipType vip)
        {
            OptionFee model = new OptionFee();



            decimal[] r = Constant.GetRate(chanel == Logic.ChannelType.荣邦科技无积分 ? 1 : 0, vip);
            decimal Rate1 = r[0];
            decimal Rate3 = r[1];
            switch (chanel)
            {
                case Logic.ChannelType.荣邦科技积分:
                    model.ratecode = feecode1[(int)vip];
                    break;
                case Logic.ChannelType.荣邦科技无积分:
                    model.ratecode = feecode2[(int)vip];
                    break;
                case Logic.ChannelType.荣邦3:
                    model.ratecode = feecode3[(int)vip];
                    break;
                default:
                    break;
            }
            model.Rate1 = Rate1;
            model.Rate3 = Rate3;
            return model;
        }


        #endregion


    }
}
