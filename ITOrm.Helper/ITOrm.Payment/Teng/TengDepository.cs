using ITOrm.Core.Helper;
using ITOrm.Utility.Const;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ITOrm.Host.BLL;
using ITOrm.Host.Models;
using Newtonsoft.Json;
using ITOrm.Utility.Log;
using ITOrm.Utility.Client;
using Newtonsoft.Json.Linq;
using ITOrm.Utility.Helper;
using ITOrm.Utility.Message;
using ITOrm.Utility.Cache;

namespace ITOrm.Payment.Teng
{
    public class TengDepository
    {
        public static string TengLogDic = "d:\\Log\\Teng";//日志文件夹地址
        public static string TengDomain = "http://47.95.41.53/";  // "http://59.110.53.175:9080/tts/";
        public static string TengNoticeUrl = ConfigHelper.GetAppSettings("TengNoticeUrl");// "http://testnotice.sujintech.com/notice/";
        public static string reqMd5Key = "AA695359FB5649F6838254362AC725A1";
        public static string respMd5Key = "3C11FA0DED674C5F9D288660F486915E";

        public static YeepayLogBLL yeepayLogDao = new YeepayLogBLL();
        public static YeepayLogParasBLL yeepayLogParasDao = new YeepayLogParasBLL();
        public static YeepayUserBLL yeepayUserDao = new YeepayUserBLL();
        public static UsersBLL usersDao = new UsersBLL();
        public static UserBankCardBLL userBankCardDao = new UserBankCardBLL();
        public static PayRecordBLL payRecordDao = new PayRecordBLL();
        public static WithDrawBLL withDrawDao = new WithDrawBLL();
        public static UserImageBLL userImageDao = new UserImageBLL();
        public static BankBLL bankDao = new BankBLL();
        public static PayCashierBLL payCashierDao = new PayCashierBLL();
        public static SendMsgBLL sendMsgDao = new SendMsgBLL();
        public static TimedTaskBLL timedTaskDao = new TimedTaskBLL();
        public static KeyValueBLL keyValueDao = new KeyValueBLL();
        #region 生成收银台URL

        public static ResultModelData<JObject> CreatePayCashier(int UserId,int Platform,decimal Amount,int UbkId)
        {
            string LogDic = "生成收银台";
            bool flag = false;
            //获得支付ID
            int keyId = payRecordDao.Init(UbkId, Amount, Platform, Ip.GetClientIp(), (int)Logic.ChannelType.腾付通);
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Teng.Enums.TengType.生成收银台, UserId, Platform, keyId, (int)Logic.ChannelType.腾付通);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId}", TengLogDic, LogDic);
            //生成收银台
            var payCId = payCashierDao.Init((int)Logic.ChannelType.腾付通, requestId, UbkId, UserId, keyId);
            ResultModelData<JObject> resp = new ResultModelData<JObject>();
            resp.backState = 0;
            resp.message = "请求成功";
            JObject data = new JObject();
            data["url"] = $"{Constant.CurrentApiHost}itapi/pay/tengcashier?payid={payCId}";
            resp.Data = data;
            if (payCId > 0)
            {
                #region 日志处理
                //返回后日志记录
                yeepayLogParasDao.Init(requestId, JsonConvert.SerializeObject(resp), 1);
                //易宝日志状态更新
                flag = yeepayLogDao.UpdateState(requestId, resp.backState.ToString(), resp.message, resp.backState == 0 ? 1 : resp.backState);
                #endregion
            }
            return resp;
        }

        #endregion

        #region 发送收银短信

        public static ResultModelData<JObject> SendMsgCode(int payCId,int Platform,string tengGuid)
        {
            ResultModelData<JObject> ret = new ResultModelData<JObject>();
            ret.backState = -100;
            ret.message = "发送失败";


            string LogDic = "发送收银短信";
            PayCashier payc = payCashierDao.Single(payCId);
            PayRecord pay = payRecordDao.Single(payc.PayRecordId);
            UserBankCard ubk = userBankCardDao.Single(payc.UbkId);
            int typeId = (int)Teng.Enums.TengType.发送收银短信;
            int cnt = yeepayLogDao.Count(" UserId=@UserId and KeyId=@PayRecordId and ChannelType=@ChannelType and typeId=@typeId ", new {payc.UserId,payc.PayRecordId, payc.ChannelType, typeId });
            if (cnt > 4)
            {
                ret.message = "该订单短信发送次数超限";
                return ret;
            }
            //获取请求流水号
            int requestId = yeepayLogDao.Init(typeId, payc.UserId, Platform, payc.PayRecordId, (int)Logic.ChannelType.腾付通);
            Logs.WriteLog($"获取请求流水号：payCId:{payCId},UserId:{payc.UserId},Platform:{Platform},tengGuid:{tengGuid},requestId:{requestId}", TengLogDic, LogDic);
            bool flag = false;

            //发送短信
            var resultMsg = SystemSendMsg.Send(Logic.EnumSendMsg.腾付通收银短信, ubk.Mobile,pay.Amount);
            SendMsg model = new SendMsg();
            model.UserId = payc.UserId;
            model.TypeId = (int)Logic.EnumSendMsg.腾付通收银短信;
            model.Context = resultMsg.content;
            model.CTime = DateTime.Now;
            model.IP = ITOrm.Utility.Client.Ip.GetClientIp();
            model.Merchant = resultMsg.Merchant;
            model.Mobile = resultMsg.Mobile;
            model.Platform = Platform;
            model.Service = "teng";
            model.RelationId = resultMsg.relationId;
            model.State = resultMsg.backState ? 2 : 1;
            model.UTime = DateTime.Now;
            int result = sendMsgDao.Insert(model);

            if (resultMsg.backState && result > 0)
            {
                string key = Constant.teng_mobile_code + tengGuid;
                var cacheData = new JObject();
                cacheData["mobile"] = resultMsg.Mobile;
                cacheData["code"] = resultMsg.code;
                MemcachHelper.Set(key, cacheData.ToString(), ITOrm.Utility.Const.Constant.mobile_code_expires);
                var data = new JObject();
                data["tengGuid"] = tengGuid;
                if (Constant.IsDebug)
                {
                    data["code"] = resultMsg.code;
                }
                ret.backState = 0;
                ret.message = "发送成功";
                ret.Data = data;
            }
            #region 日志处理
            //返回后日志记录
            yeepayLogParasDao.Init(requestId, JsonConvert.SerializeObject(ret), 1);
            //易宝日志状态更新
            flag = yeepayLogDao.UpdateState(requestId, ret.backState.ToString(), ret.message, ret.backState == 0 ? 1 : ret.backState);
            #endregion
            return ret;
        }

        #endregion

        #region 验证收银短信

        public static ResultModelData<JObject> ValidateMobileCode(int payCId, int Platform, string tengGuid,string Code)
        {
            bool flag = false;
            ResultModelData<JObject> ret = new ResultModelData<JObject>();
            ret.backState = 0;
            ret.message = "验证成功";
            string LogDic = "验证收银短信";
            PayCashier payc = payCashierDao.Single(payCId);
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Teng.Enums.TengType.验证收银短信, payc.UserId, Platform, payc.PayRecordId, (int)Logic.ChannelType.腾付通);
            Logs.WriteLog($"获取请求流水号：UserId:{payc.UserId},Platform:{Platform},requestId:{requestId}", TengLogDic, LogDic);

            string mobileKey = ITOrm.Utility.Const.Constant.teng_mobile_code + tengGuid;

            if (!ITOrm.Utility.Cache.MemcachHelper.Exists(mobileKey))
            {
                ret.backState = -100;
                ret.message = "验证码已过期";
            }
            JObject cacheMobileCode = JObject.Parse(ITOrm.Utility.Cache.MemcachHelper.Get(mobileKey).ToString());
            if (Code.Trim() != cacheMobileCode["code"].ToString())
            {
                ret.backState = -100;
                ret.message = "手机验证码错误";
            }


            #region 日志处理
            //返回后日志记录
            yeepayLogParasDao.Init(requestId, JsonConvert.SerializeObject(ret), 1);
            //易宝日志状态更新
            flag = yeepayLogDao.UpdateState(requestId, ret.backState.ToString(), ret.message, ret.backState == 0 ? 1 : ret.backState);
            #endregion


            return ret;
        }

        #endregion

        #region 支付接口
        public static respTengModel DebitPayment(int payCId,int Platform)
        {
            string LogDic = "支付接口";
            bool flag = false;

            PayCashier payCashier = payCashierDao.Single(payCId);
            PayRecord pay = payRecordDao.Single(payCashier.PayRecordId);

            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Teng.Enums.TengType.支付接口,pay.UserId, Platform,pay.ID, (int)Logic.ChannelType.腾付通);
            Logs.WriteLog($"获取请求流水号：UserId:{pay.UserId},Platform:{Platform},requestId:{requestId},payCId:{payCId}", TengLogDic, LogDic);
            var user = usersDao.Single(payCashier.UserId);
            var withBank = userBankCardDao.Single(" TypeId=0 and UserId=@UserId ", new { pay.UserId });
            var payBank = userBankCardDao.Single(payCashier.UbkId);
            reqTengDebitPaymentModel model = new reqTengDebitPaymentModel();
            model.orderId = requestId.ToString();
            model.name = user.RealName;
            model.idCardNo = user.IdCard;
            model.phone = pay.PayerPhone;
            model.txnAmt = (pay.Amount * 100M).ToString("F0");
            model.settleAmt =(pay.ActualAmount*100M).ToString("F0");//清算金额
            model.notifyUrl = TengNoticeUrl + "NoticePayTeng";
            model.bankCardNo = pay.BankCard;
            model.lastThreeNo = payBank.CVN2;
            model.cardExpYear = payBank.ExpiresYear;
            model.cardExpMonth = payBank.ExpiresMouth;
            model.withdrawUrl= TengNoticeUrl + "NoticeWithTeng";
            model.settleCardNo = pay.DrawBankCard;
            model.settlePhone= withBank.Mobile;

            //model.accountrule = "1";
            JObject json = Order<reqTengDebitPaymentModel>(model);
            #region des签名对象

            JObject data = new JObject();
            JObject reqData = JObject.Parse(json.ToString());
            data["ExpressData"] = json;
            data["EncryptionData"] = reqData;
            reqData["name"] = ITOrm.Utility.Encryption.EncryptionHelper.DesEncrypt(reqMd5Key, reqData["name"].ToString());
            reqData["idCardNo"] = ITOrm.Utility.Encryption.EncryptionHelper.DesEncrypt(reqMd5Key, reqData["idCardNo"].ToString());
            reqData["phone"] = ITOrm.Utility.Encryption.EncryptionHelper.DesEncrypt(reqMd5Key, reqData["phone"].ToString());
            reqData["bankCardNo"] = ITOrm.Utility.Encryption.EncryptionHelper.DesEncrypt(reqMd5Key, reqData["bankCardNo"].ToString());
            reqData["lastThreeNo"] = ITOrm.Utility.Encryption.EncryptionHelper.DesEncrypt(reqMd5Key, reqData["lastThreeNo"].ToString());
            reqData["cardExpYear"] = ITOrm.Utility.Encryption.EncryptionHelper.DesEncrypt(reqMd5Key, reqData["cardExpYear"].ToString());
            reqData["cardExpMonth"] = ITOrm.Utility.Encryption.EncryptionHelper.DesEncrypt(reqMd5Key, reqData["cardExpMonth"].ToString());
            reqData["settleCardNo"] = ITOrm.Utility.Encryption.EncryptionHelper.DesEncrypt(reqMd5Key, reqData["settleCardNo"].ToString());
            reqData["settlePhone"] = ITOrm.Utility.Encryption.EncryptionHelper.DesEncrypt(reqMd5Key, reqData["settlePhone"].ToString());
            #endregion
            var resp = PostUrl<respTengModel>(requestId, "debitPayment.do", data, LogDic);
            if (resp.backState == 0)
            {
                //更新收款记录
                flag = payRecordDao.UpdateState(payCashier.PayRecordId, 1, resp.respMsg);
                Logs.WriteLog($"更新收款记录：UserId:{payCashier.UserId},Platform:{Platform},requestId:{requestId},PayRecordId:{payCashier.PayRecordId},flag:{flag}", TengLogDic, LogDic);
                //更新收银台
                payCashier.State = 2;
                payCashier.UTime = DateTime.Now;
                flag = payCashierDao.Update(payCashier);
                Logs.WriteLog($"更新收银台：UserId:{payCashier.UserId},Platform:{Platform},requestId:{requestId},PayCId:{payCashier.ID},flag:{flag}", TengLogDic, LogDic);
            }
            else if (resp.respCode == "YH99")
            {
                int keyId = 10009;//腾付通通道记录ID
                //关闭通道
                KeyValue kv = keyValueDao.Single(keyId);
                kv.State = -1;
                kv.UTime = DateTime.Now;
                kv.Remark = $"{DateTime.Now}自动关闭";
                keyValueDao.Update(kv);
                MemcachHelper.Delete(Constant.list_keyvalue_key + (int)Logic.KeyValueType.支付通道管理);//清理缓存

                //添加定时任务记录
                var stime = JObject.Parse(kv.Value)["StartTime"];
                DateTime execTime = Convert.ToDateTime(DateTime.Now.ToString($"yyyy-MM-dd {stime}")).AddDays(1);

                JObject value = new JObject();
                value["keyId"] = keyId;
                timedTaskDao.Init(Logic.TimedTaskType.通道开启, execTime, value.ToString());
            }

            if (resp.backState != 0)
            {
                //更新收款记录
                flag = payRecordDao.UpdateState(payCashier.PayRecordId, -1, resp.respMsg);
                //更新收银台
                payCashier.State = -1;
                payCashier.UTime = DateTime.Now;
                flag = payCashierDao.Update(payCashier);
            }
            return resp;
        }
        #endregion

        #region 代付申请

        public static respTengModel DebitWithdraw(int RequestId,int Platform)
        {
            string LogDic = "代付申请";
            bool flag = false;

            var yeepayLog = yeepayLogDao.Single(RequestId);

            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Teng.Enums.TengType.代付申请, yeepayLog.UserId, Platform, yeepayLog.KeyId, (int)Logic.ChannelType.腾付通);
            Logs.WriteLog($"获取请求流水号：UserId:{yeepayLog.UserId},Platform:{Platform},requestId:{requestId}", TengLogDic, LogDic);
            var user = usersDao.Single(yeepayLog.UserId);
            
            reqTengModel model = new reqTengModel();
            model.orderId = RequestId.ToString();
            JObject json = Order<reqTengModel>(model);
            #region des签名对象

            JObject data = new JObject();
            JObject reqData = JObject.Parse(json.ToString());
            data["ExpressData"] = json;
            data["EncryptionData"] = reqData;
            #endregion
            var resp = PostUrl<respTengModel>(requestId, "debitWithdraw.do", data, LogDic);
            if (resp.backState == 0)
            {
                var pay = payRecordDao.Single(yeepayLog.KeyId);
                pay.DrawState = 1;
                pay.UTime = DateTime.Now;
                flag = payRecordDao.Update(pay);
                Logs.WriteLog($"代付申请：UserId:{yeepayLog.UserId},Platform:{Platform},requestId:{requestId},flag:{flag}", TengLogDic, LogDic);
            }
            return resp;
        }

        #endregion

        #region 支付结果查询

        public static respPayDebitQueryModel PayDebitQuery(int RequestId, int Platform)
        {
            string LogDic = "支付结果查询";
            bool flag = false;

            var yeepayLog = yeepayLogDao.Single(RequestId);

            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Teng.Enums.TengType.支付结果查询, yeepayLog.UserId, Platform, 0, (int)Logic.ChannelType.腾付通);
            Logs.WriteLog($"获取请求流水号：UserId:{yeepayLog.UserId},Platform:{Platform},requestId:{requestId}", TengLogDic, LogDic);
            var user = usersDao.Single(yeepayLog.UserId);

            reqTengModel model = new reqTengModel();
            model.orderId = RequestId.ToString();
            JObject json = Order<reqTengModel>(model);
            #region des签名对象

            JObject data = new JObject();
            JObject reqData = JObject.Parse(json.ToString());
            data["ExpressData"] = json;
            data["EncryptionData"] = reqData;
            #endregion
            var resp = PostUrl<respPayDebitQueryModel>(requestId, "payDebitQuery.do", data, LogDic);
            if (resp.backState == 0)
            {
               
            }
            return resp;
        }

        #endregion

        #region 代付结果查询

        public static respDebitWithdrawQueryModel DebitWithdrawQuery(int RequestId, int Platform)
        {
            string LogDic = "代付结果查询";
            bool flag = false;

            var yeepayLog = yeepayLogDao.Single(RequestId);

            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Teng.Enums.TengType.代付结果查询, yeepayLog.UserId, Platform, 0, (int)Logic.ChannelType.腾付通);
            Logs.WriteLog($"获取请求流水号：UserId:{yeepayLog.UserId},Platform:{Platform},requestId:{requestId}", TengLogDic, LogDic);
            var user = usersDao.Single(yeepayLog.UserId);

            reqTengModel model = new reqTengModel();
            model.orderId = RequestId.ToString();
            JObject json = Order<reqTengModel>(model);
            #region des签名对象

            JObject data = new JObject();
            JObject reqData = JObject.Parse(json.ToString());
            data["ExpressData"] = json;
            data["EncryptionData"] = reqData;
            #endregion
            var resp = PostUrl<respDebitWithdrawQueryModel>(requestId, "debitWithdrawQuery.do", data, LogDic);
            if (resp.backState == 0)
            {

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
        public static T PostUrl<T>(int requestId, string action, JObject data, string logPath)
        {

            try
            {
                bool flag = false;
                string result = string.Empty;

                //拼接签名
                StringBuilder sb = new StringBuilder();
                var reqData =JObject.FromObject( data["EncryptionData"]);
                foreach (var item in reqData)
                {
                    sb.AppendFormat("{0}={1}&",item.Key,item.Value);
                }
                string str = sb.ToString().Trim('&');

                string sign = ITOrm.Utility.Encryption.SecurityHelper.GetMD5String($"{str}&key={reqMd5Key}").ToUpper();
                str = $"{str}&sign={sign}";
                data["EncryptionData"]["sign"] = sign;
                //请求前日志记录
                Logs.WriteLog("提交参数：" + data.ToString(), TengLogDic, logPath);
                yeepayLogParasDao.Init(requestId, data.ToString(), 0);

                
                //执行请求
                int responseState = ITOrm.Utility.Client.HttpHelper.HttpPost(TengDomain+action, str, Encoding.UTF8, out result);
                if (responseState != 200)
                {
                    result = $"{{\"respCode\":\"{responseState}\", \"respMsg\":\"{result}\"}}";
                }
                //返回后日志记录
                Logs.WriteLog("返回参数：" + result, TengLogDic, logPath);
                yeepayLogParasDao.Init(requestId, result, 1);
                //易宝日志状态更新
                respTengModel resp = JsonConvert.DeserializeObject<respTengModel>(result);
                flag = yeepayLogDao.UpdateState(requestId, resp.respCode, resp.respMsg, resp.backState == 0 ? 1 : resp.backState);
                Logs.WriteLog($"易宝日志状态更新：requestId:{requestId},respCode:{resp.respCode},respMsg:{ resp.respMsg},State:{flag}", TengDomain, logPath);
                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception e)
            {
                return JsonConvert.DeserializeObject<T>($"{{\"respCode\":\"-1000\", \"respMsg\":\"{e.Message}\"}}");
            }

        }


        /// <summary>
        /// 按a-z排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static JObject Order<T>(T model)
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
            return data;
        }


        public static bool isSign(respTengModel model)
        {
            var data = TengDepository.Order<respTengModel>(model);
            //拼接签名
            StringBuilder sb = new StringBuilder();
            foreach (var item in data)
            {
                if (item.Key != "sign"&&item.Key!= "backState")
                {
                    sb.AppendFormat("{0}={1}&", item.Key, item.Value);
                }
            }

            string str = sb.ToString().Trim('&');
            return ITOrm.Utility.Encryption.SecurityHelper.GetMD5String($"{str}&key={TengDepository.respMd5Key}").ToUpper() == model.sign;
        }


        #endregion

    }

    
}
