using ITOrm.Host.BLL;
using ITOrm.Utility.StringHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using ITOrm.Utility.Log;
using ITOrm.Host.Models;
using ITOrm.Payment.Yeepay;
using System.Text;
using Newtonsoft.Json.Linq;
using ITOrm.Utility.Encryption;
using ITOrm.Payment.Masget;
using ITOrm.Payment.Teng;
using ITOrm.Utility.Const;
using ITOrm.Payment.MiShua;
using System.IO;

namespace ITOrm.Notice.Controllers
{
    public class NoticeController : Controller
    {
        public static YeepayLogBLL yeepayLogDao = new YeepayLogBLL();
        public static YeepayLogParasBLL yeepayLogParasDao = new YeepayLogParasBLL();
        public static YeepayUserBLL yeepayUserDao = new YeepayUserBLL();
        public static UsersBLL usersDao = new UsersBLL();
        public static UserBankCardBLL userBankCardDao = new UserBankCardBLL();
        public static PayRecordBLL payRecordDao = new PayRecordBLL();
        public static WithDrawBLL withDrawDao = new WithDrawBLL();
        public static object lockWithDrawApi = new object();
        public static object lockReceiveApi = new object();
        public static object lockbackpayNotice = new object();
        public static object lockNoticePayTeng = new object();
        public static object lockNoticeWithTeng = new object();
        public static object lockMiShuaNotice = new object();
        // 易宝结算回调
        public string withDrawApi()
        {
            string[] paramtersKey = System.Web.HttpContext.Current.Request.Form.AllKeys;
            var sortedParamtersKey = from s in paramtersKey
                                     orderby s ascending
                                     select s;
            StringBuilder str = new StringBuilder();
            str.Append("{");
            foreach (string key in sortedParamtersKey)
            {
                str.AppendFormat("\"{0}\":\"{1}\",", key, System.Web.HttpContext.Current.Request.Form[key].Trim());
            }
            if (str.Length > 0)
            {
                str.Remove(str.Length - 1, 1);//移除最后一个逗号
            }
            str.Append("}");

            //返回后日志记录
            Logs.WriteLog("页面首次记录：" + str.ToString(), "d:\\Log\\Yeepay", "WithDrawApiNotice");


            noticeWithDrawApiModel model = new noticeWithDrawApiModel();
            model.mainCustomerNumber = TQuery.GetString("mainCustomerNumber");
            model.externalNo = TQuery.GetString("externalNo");
            model.customerNumber = TQuery.GetString("customerNumber");
            model.serialNo = TQuery.GetString("serialNo");
            model.transferStatus = TQuery.GetString("transferStatus");
            model.requestTime = TQuery.GetString("requestTime");
            model.handleTime = TQuery.GetString("handleTime");
            model.transferWay = TQuery.GetString("transferWay");
            model.receiver = TQuery.GetString("receiver");
            model.receiverBankCardNo = TQuery.GetString("receiverBankCardNo");
            model.receiverBank = TQuery.GetString("receiverBank");
            model.amount = TQuery.GetString("amount");
            model.fee = TQuery.GetString("fee");
            model.basicFee = TQuery.GetString("basicFee");
            model.exTargetFee = TQuery.GetString("exTargetFee");
            model.actualAmount = TQuery.GetString("actualAmount");
            model.failReason = TQuery.GetString("failReason");
            model.hmac = TQuery.GetString("hmac");
            model.code = TQuery.GetString("code");
            model.message = TQuery.GetString("message");
            //string json = "{\"actualAmount\":\"98.62\",\"amount\":\"99.62\",\"basicFee\":\"1.0\",\"customerNumber\":\"10019642647\",\"exTargetFee\":\"0.0\",\"externalNo\":\"100000178\",\"failReason\":\"\",\"fee\":\"0\",\"handleTime\":\"2018-03-07 17:18:39\",\"hmac\":\"8200ea9ccdf38d43a0e0ba9606bb504a\",\"mainCustomerNumber\":\"10018708270\",\"receiver\":\"渠*树\",\"receiverBank\":\"中国银行\",\"receiverBankCardNo\":\"621790*********6547\",\"requestTime\":\"2018-03-07 17:18:38\",\"serialNo\":\"SKBRJT325245e2492a4fdfad9066b89a0ef4a9\",\"transferStatus\":\"SUCCESSED\",\"transferWay\":\"1\"}";
            //string json = "{\"actualAmount\":\"175.23\",\"amount\":\"177.23\",\"basicFee\":\"2.0\",\"customerNumber\":\"10020136223\",\"exTargetFee\":\"0.0\",\"externalNo\":\"100000245\",\"failReason\":\"\",\"fee\":\"0\",\"handleTime\":\"2018-03-13 15:01:57\",\"hmac\":\"bbed9dfa537fef9658d27f1e444c210c\",\"mainCustomerNumber\":\"10018708270\",\"receiver\":\"王*凯\",\"receiverBank\":\"中国银行\",\"receiverBankCardNo\":\"621790*********9332\",\"requestTime\":\"2018-03-13 15:01:54\",\"serialNo\":\"SKBRJTa4ce8f5595c54e47a12a939f1202a680\",\"transferStatus\":\"SUCCESSED\",\"transferWay\":\"1\"}";
            //string json = "{\"actualAmount\":\"103.6\",\"amount\":\"104.6\",\"basicFee\":\"1.0\",\"customerNumber\":\"10020136223\",\"exTargetFee\":\"0.0\",\"externalNo\":\"100000233\",\"failReason\":\"\",\"fee\":\"0\",\"handleTime\":\"2018-03-12 22:32:14\",\"hmac\":\"b5a06307c9e40dffe7b697ea20045406\",\"mainCustomerNumber\":\"10018708270\",\"receiver\":\"王*凯\",\"receiverBank\":\"中国银行\",\"receiverBankCardNo\":\"621790*********9332\",\"requestTime\":\"2018-03-12 22:32:14\",\"serialNo\":\"SKBRJT7262630296714a8191468c61d4235399\",\"transferStatus\":\"SUCCESSED\",\"transferWay\":\"1\"}";
            //model = JsonConvert.DeserializeObject<noticeWithDrawApiModel>(json);

            lock (lockWithDrawApi)
            {
                bool flag = false;
                string result = "fail";
                //返回后日志记录
                Logs.WriteLog("回调参数：" + JsonConvert.SerializeObject(model), "d:\\Log\\Yeepay", "WithDrawApiNotice");

                //签名验证
                StringBuilder sb = new StringBuilder();
                sb.Append(model.mainCustomerNumber);
                sb.Append(model.customerNumber);
                sb.Append(model.externalNo);
                sb.Append(model.serialNo);
                sb.Append(model.transferStatus);
                sb.Append(model.requestTime);
                sb.Append(model.handleTime);
                sb.Append(model.transferWay);
                sb.Append(model.receiver);
                sb.Append(model.receiverBankCardNo);
                sb.Append(model.receiverBank);
                sb.Append(model.amount);
                sb.Append(model.fee);
                sb.Append(model.basicFee);
                sb.Append(model.exTargetFee);
                sb.Append(model.actualAmount);
                sb.Append(model.failReason);

                string sign = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(ITOrm.Payment.Yeepay.YeepayDepository.YeepayHmacKey, sb.ToString());
                if (model.hmac != sign)
                {
                    Logs.WriteLog($"签名比对失败：mac:{model.hmac},sign:{sign}", "d:\\Log\\Yeepay", "WithDrawApiNotice");
                    return result;
                }

                yeepayLogParasDao.Init(Convert.ToInt32(model.externalNo), JsonConvert.SerializeObject(model), 2);

                int state = -1;
                switch (model.transferStatus)
                {
                    case "SUCCESSED":
                        state = 10;
                        break;
                    case "RECEIVED":
                        state = 1;
                        break;
                    case "PROCESSING":
                        state = 2;
                        break;
                    case "FAILED":
                        state = -1;
                        break;
                    case "REFUNED":
                        state = -2;
                        break;
                    case "CANCELLED":
                        state = -3;
                        break;
                    default:
                        state = -4;
                        break;
                }
                //更新
                yeepayLogDao.UpdateState(Convert.ToInt32(model.externalNo), model.transferStatus == "SUCCESSED" ? "0000" : "9999", model.failReason, state);
                var yeepayLog = yeepayLogDao.Single(Convert.ToInt32(model.externalNo));
                var draw = withDrawDao.Single(yeepayLog.KeyId);
                var pay = payRecordDao.Single(draw.PayId);
                if (draw.State == 10)
                {
                    result = "SUCCESS";
                    Logs.WriteLog($"重复处理 :{result},draw.State ==10", "d:\\Log\\Yeepay", "WithDrawApiNotice");
                    return result;
                }


                //修改订单信息
                draw.UTime = DateTime.Now;
                draw.State = state;
                draw.Message = model.failReason;
                pay.DrawState = state;
                if (state == 10)
                {
                    draw.HandleTime = Convert.ToDateTime(model.handleTime);
                    draw.Receiver = model.receiver;
                    draw.ReceiverBankCardNo = model.receiverBankCardNo;
                    draw.ReceiverBank = model.receiverBank;
                    draw.Fee = Convert.ToDecimal(model.fee);
                    draw.BasicFee = Convert.ToDecimal(model.basicFee);
                    draw.ExTargetFee = Convert.ToDecimal(model.exTargetFee);
                    draw.ActualAmount = Convert.ToDecimal(model.actualAmount);
                    pay.HandleTime = draw.HandleTime;
                    //pay.DrawBankCard = draw.ReceiverBankCardNo;

                }
                flag = withDrawDao.Update(draw);
                Logs.WriteLog($"结算订单修改：flag:{flag},transferStatus:{model.transferStatus},state:{state}", "d:\\Log\\Yeepay", "WithDrawApiNotice");
                flag = payRecordDao.Update(pay);
                Logs.WriteLog($"支付订单修改：flag:{flag},transferStatus:{model.transferStatus},state:{state}", "d:\\Log\\Yeepay", "WithDrawApiNotice");

                if (flag) result = "SUCCESS";
                Logs.WriteLog($"返回结果:{result}", "d:\\Log\\Yeepay", "WithDrawApiNotice");
                return result;
            }
        }

        //易宝收款回调
        public string receiveApi()
        {

            string[] paramtersKey = System.Web.HttpContext.Current.Request.Form.AllKeys;
            var sortedParamtersKey = from s in paramtersKey
                                     orderby s ascending
                                     select s;
            StringBuilder str = new StringBuilder();
            str.Append("{");
            foreach (string key in sortedParamtersKey)
            {
                str.AppendFormat("\"{0}\":\"{1}\",", key, System.Web.HttpContext.Current.Request.Form[key].Trim());
            }
            if (str.Length > 0)
            {
                str.Remove(str.Length - 1, 1);//移除最后一个逗号
            }
            str.Append("}");

            //返回后日志记录
            Logs.WriteLog("页面首次记录：" + str.ToString(), "d:\\Log\\Yeepay", "ReceiveApiNotice");
            noticeReceiveApiModel model = new noticeReceiveApiModel();
            model.code = TQuery.GetString("code");
            model.message = TQuery.GetString("message");
            model.amount = TQuery.GetString("amount");
            model.fee = TQuery.GetString("fee");
            model.externalld = TQuery.GetString("externalld");
            model.payerPhone = TQuery.GetString("payerPhone");
            model.payerName = TQuery.GetString("payerName");
            model.payTime = TQuery.GetString("payTime");
            model.requestId = TQuery.GetString("requestId");
            model.src = TQuery.GetString("src");
            model.status = TQuery.GetString("status");
            model.lastNo = TQuery.GetString("lastNo");
            model.bankCode = TQuery.GetString("bankCode");
            model.busiType = TQuery.GetString("busiType");
            model.customerNumber = TQuery.GetString("customerNumber");
            model.createTime = TQuery.GetString("createTime");
            model.hmac = TQuery.GetString("hmac");
            //string json = "{\"requestId\":\"100000091\",\"customerNumber\":\"10019321635\",\"externalld\":\"661461948596449280\",\"createTime\":\"2018-02-24 15:49:19\",\"payTime\":\"2018-02-24 16:17:55\",\"amount\":\"100\",\"fee\":\"0.43\",\"status\":\"SUCCESS\",\"busiType\":\"COMMON\",\"bankCode\":\"CCB\",\"payerName\":\"刘*敏\",\"payerPhone\":\"185****6235\",\"lastNo\":\"625362******0286\",\"src\":\"B\",\"mainCustomerNumber\":null,\"hmac\":\"54a4145327a2db38e2ab7f0146eb2cea\",\"code\":\"0000\",\"message\":\"成功\",\"backState\":0}";
            //model = JsonConvert.DeserializeObject<noticeReceiveApiModel>(json);
            lock (lockReceiveApi)
            {
                string result = "fail";
                bool flag = false;
                //返回后日志记录
                Logs.WriteLog("回调参数：" + JsonConvert.SerializeObject(model), "d:\\Log\\Yeepay", "ReceiveApiNotice");
                yeepayLogParasDao.Init(Convert.ToInt32(model.requestId), JsonConvert.SerializeObject(model), 2);


                //签名验证
                StringBuilder sb = new StringBuilder();
                sb.Append(model.code);
                sb.Append(model.message);
                sb.Append(model.requestId);
                sb.Append(model.customerNumber);
                sb.Append(model.externalld);
                sb.Append(model.createTime);
                sb.Append(model.payTime);
                sb.Append(model.amount);
                sb.Append(model.fee);
                sb.Append(model.status);
                sb.Append(model.busiType);
                sb.Append(model.bankCode);
                sb.Append(model.payerName);
                sb.Append(model.payerPhone);
                sb.Append(model.lastNo);
                sb.Append(model.src);

                string sign = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(ITOrm.Payment.Yeepay.YeepayDepository.YeepayHmacKey, sb.ToString());
                if (model.hmac != sign)
                {
                    Logs.WriteLog($"签名比对失败：mac:{model.hmac},sign:{sign}", "d:\\Log\\Yeepay", "ReceiveApiNotice");
                    return result;
                }
                //更新
                yeepayLogDao.UpdateState(Convert.ToInt32(model.requestId), model.code, model.message, (model.backState == 0 && model.status == "SUCCESS") ? 10 : -1);

                if (model.backState == 0 && model.status == "SUCCESS")//成功
                {
                    var yeepayLog = yeepayLogDao.Single(Convert.ToInt32(model.requestId));
                    var payRecord = payRecordDao.Single(yeepayLog.KeyId);
                    if (payRecord.State == 10)
                    {
                        result = "SUCCESS";
                        Logs.WriteLog($"重复处理：{result},payRecord.State == 10", "d:\\Log\\Yeepay", "ReceiveApiNotice");
                        return result;
                    }
                    //修改订单信息
                    payRecord.PayTime = Convert.ToDateTime(model.payTime);
                    payRecord.State = 10;
                    //payRecord.PayerPhone = model.payerPhone;
                    //payRecord.PayerName = model.payerName;
                    payRecord.LastNo = model.lastNo;
                    payRecord.Message = model.message;
                    payRecord.BankCode = model.bankCode;
                    payRecord.Fee = Convert.ToDecimal(model.fee);
                    payRecord.Src = model.src;
                    flag = payRecordDao.Update(payRecord);
                    Logs.WriteLog($"修改订单信息：flag={flag}", "d:\\Log\\Yeepay", "ReceiveApiNotice");
                    ////添加支付银行卡
                    //int ubkCnt = userBankCardDao.Count(" UserId=@UserId and TypeId=1 and State=1 and  BankCard=@BankCard ", new { payRecord.UserId, payRecord.BankCard });
                    //Logs.WriteLog($"支付银行卡个数：ubkCnt：{ubkCnt}", "d:\\Log\\Yeepay", "ReceiveApiNotice");
                    //if (ubkCnt == 0)
                    //{
                    //    UserBankCard ubk = new UserBankCard();
                    //    ubk.BankCard = payRecord.BankCard;
                    //    ubk.TypeId = 1;
                    //    ubk.State = 1;
                    //    ubk.UserId = payRecord.UserId;
                    //    ubk.Mobile = model.payerPhone;
                    //    ubk.IP = ITOrm.Utility.Client.Ip.GetClientIp();
                    //    ubk.Platform = 1;
                    //    ubk.BankCode = model.bankCode;
                    //    int ubkId= userBankCardDao.Insert(ubk);
                    //    Logs.WriteLog($"支付银行卡添加Id：ubkId：{ubkId}", "d:\\Log\\Yeepay", "ReceiveApiNotice");
                    //}
                    if (flag) result = "SUCCESS";
                }
                Logs.WriteLog($"返回结果:{result}", "d:\\Log\\Yeepay", "ReceiveApiNotice");
                return result;
            }
        }


        //荣邦科技  支付回调
        public string backpayNotice()
        {
            string[] paramtersKey = System.Web.HttpContext.Current.Request.Form.AllKeys;
            var sortedParamtersKey = from s in paramtersKey
                                     orderby s ascending
                                     select s;
            StringBuilder str = new StringBuilder();
            str.Append("{");
            foreach (string key in sortedParamtersKey)
            {
                str.AppendFormat("\"{0}\":\"{1}\",", key, System.Web.HttpContext.Current.Request.Form[key].Trim());
            }
            if (str.Length > 0)
            {
                str.Remove(str.Length - 1, 1);//移除最后一个逗号
            }
            str.Append("}");

            //返回后日志记录
            Logs.WriteLog("页面首次记录：" + str.ToString(), "d:\\Log\\Masget", "backpayNotice");


            bool flag = false;


            //string json= "{\"Appid\":\"402862423\",\"Data\":\"5JuUVmDLj4r9d-7FhjaEn1LtxEUAdBv3Aco2lYdyOhzPEWt-ZFCPe8vHFDaPvrc8AG4s0cH1lJ393A1aSWCRTTGEp5rDl0aYCp2ijBlN_jyAp8HwEDdYpGyKE5PkO8XGLmjbPPyj3zLXVNuHvgHuhTRiQ5pNgNTj6ZG2vSqsmednikP5MOEo4HBQX_mTMpzi_P_fnTFHfK4MlVmPamHqxdG1uOeu2vLPJjVB01crohdtiEBU-ZE6WHsRwmgxVPrcUdWCSzsipukK4Ebgfzz34F8d4ZBcOnR0aNz7_ZB-xDLK66yNBnhI7V6fY-Pg_oaqK13Cc9tJYssbzqPX4S5Ek29KX4lous7BCyjvYYJmp_3MTbDU0Sq1t_A8IMUeNI8K4iHyubHYnS2-vclmwIY0_YnYqOSuZASS12juzDR_nDyFzf_k5iWbMQ_E6vEoGTz0Vmq-r93rDZkpcXeS2TRYjnkZNXSRvihdqL3ZA4BtCt9uRDQ02E3hemJDetnCGAYRfm8d_yoHQhc_GNcIXA_MQt-lzVT2Cp1Qj2kPDBxjZyVm7SlL_ticf44MvitGpxxueDOywcJkV0Az0iVyWsjuAXDJovBaoSCA-AZAi35jYcV3_O0hv1Qne9juWhDm7JKh_OGEP2qVf8MfgNDNB8yOXK258UMoEfQEfzk5uQT3txjmq75BqpwEXrpA44AjEBTpxSvbIYID-crcBrYbhHpzP3X1zMJXh3TeAerTC-QigIx4u6FdDMXyzFD9PpyFft2kixCnf1P0xL1azAYRlD6H7UmgJC-wfZR-eFR3_hrbC8RymD8AVdwiJGzSikPERAIXcQbF760M4qIfATg71WC6lirRuL-wRrFwCtbrcurzHxp95kUMSlF1UH0XI5p7S6GZ\",\"Method\":\"paymentreport\",\"Sign\":\"b6609c9bf0b74b23249a733c9dd2524e\"}";
            //JObject data = JObject.Parse(json);
            //string Secretkey = "dxn47a9egzljq3pw";
            //string AesD = ITOrm.Payment.Masget.AES.Decrypt(data["Data"].ToString(), Secretkey, Secretkey);
            //string lastSign =$"{data["Data"].ToString()}{Secretkey}";
            //string sign = SecurityHelper.GetMD5String(lastSign);

            lock (lockbackpayNotice)
            {
                noticeMasgetModel<noticePayConfirmpayModel> notice = new noticeMasgetModel<noticePayConfirmpayModel>();
                notice.Appid = TQuery.GetString("Appid");
                notice.Data = TQuery.GetString("Data");
                notice.Sign = TQuery.GetString("Sign");
                notice.Method = TQuery.GetString("Method");

                //notice = JsonConvert.DeserializeObject<noticeMasgetModel<noticePayConfirmpayModel>>(json);
                JObject respNotice = new JObject();
                respNotice["response"] = "00";
                respNotice["message"] = "成功";

                Logs.WriteLog($"参数序列化记录：{JsonConvert.SerializeObject(notice)}", "d:\\Log\\Masget", "backpayNotice");

                if (notice.IsSign)
                {
                    if (notice.dataExpress != null && notice.dataExpress.respcode == "2" && notice.Method == "paymentreport")
                    {


                        //支付成功
                        int requestId = Convert.ToInt32(notice.dataExpress.ordernumber);
                        var yeepayLog = yeepayLogDao.Single(requestId);

                        if (yeepayLog.State == 10)
                        {
                            Logs.WriteLog($"重复处理：requestId:{requestId},payRecord.State == 10,返回数据：{respNotice.ToString()}", "d:\\Log\\Masget", "backpayNotice");
                            return respNotice.ToString();
                        }
                        yeepayLogParasDao.Init(requestId, JsonConvert.SerializeObject(notice), 2);
                        Logs.WriteLog($"记录日志：requestId:{requestId},notice:{JsonConvert.SerializeObject(notice)}", "d:\\Log\\Masget", "backpayNotice");

                        //更新
                        yeepayLogDao.UpdateState(requestId, notice.dataExpress.respcode, notice.dataExpress.respmsg, (notice.dataExpress.respcode == "2") ? 10 : -1);
                        Logs.WriteLog($"更新日志：requestId:{requestId},respmsg:{notice.dataExpress.respmsg}", "d:\\Log\\Masget", "backpayNotice");


                        int payRecordId = yeepayLog.KeyId;
                        var payRecord = payRecordDao.Single(payRecordId);
                        payRecord.State = 10;
                        payRecord.DrawState = 10;
                        payRecord.Message = "支付成功";
                        payRecord.HandleTime = Convert.ToDateTime(notice.dataExpress.businesstime);
                        flag = payRecordDao.Update(payRecord);
                        //更新支付记录
                        Logs.WriteLog($"更新支付记录：requestId:{requestId},payRecordId:{payRecordId},flag:{flag}", "d:\\Log\\Masget", "backpayNotice");
                    }
                    else
                    {
                        Logs.WriteLog($"其他错误：appid:{notice.Appid},Method:{notice.Method}", "d:\\Log\\Masget", "backpayNotice");
                        respNotice["response"] = "99";
                        respNotice["message"] = "其他错误";
                    }
                }
                else//签名失败
                {
                    Logs.WriteLog($"签名比对失败：appid:{notice.Appid},Method:{notice.Method},sign:{notice.Sign},sysSign:{notice.sysSign}", "d:\\Log\\Masget", "backpayNotice");
                    respNotice["response"] = "99";
                    respNotice["message"] = "签名比对失败";
                }
                Logs.WriteLog($"返回数据：{respNotice.ToString()}", "d:\\Log\\Masget", "backpayNotice");
                return respNotice.ToString();
            }

        }

        //腾付通  支付回调
        public string NoticePayTeng()
        {
            string[] paramtersKey = System.Web.HttpContext.Current.Request.Form.AllKeys;
            var sortedParamtersKey = from s in paramtersKey
                                     orderby s ascending
                                     select s;
            StringBuilder str = new StringBuilder();
            str.Append("{");
            foreach (string key in sortedParamtersKey)
            {
                str.AppendFormat("\"{0}\":\"{1}\",", key, System.Web.HttpContext.Current.Request.Form[key].Trim());
            }
            if (str.Length > 0)
            {
                str.Remove(str.Length - 1, 1);//移除最后一个逗号
            }
            str.Append("}");

            //返回后日志记录
            Logs.WriteLog("页面首次记录：" + str.ToString(), "d:\\Log\\Teng", "NoticePayTeng");

            lock (lockNoticePayTeng)
            {
                string result = "fail";
                respTengModel model = new respTengModel();
                model.version = TQuery.GetString("version");
                model.agentId = TQuery.GetString("agentId");
                model.merId = TQuery.GetString("merId");
                model.orderId = TQuery.GetString("orderId");
                model.respCode = TQuery.GetString("respCode");
                model.respMsg = TQuery.GetString("respMsg");
                model.sign = TQuery.GetString("sign");
                bool flag = false;
                //返回后日志记录
                Logs.WriteLog("回调参数：" + JsonConvert.SerializeObject(model), "d:\\Log\\Teng", "NoticePayTeng");
                yeepayLogParasDao.Init(Convert.ToInt32(model.orderId), JsonConvert.SerializeObject(model), 2);


                if (TengDepository.isSign( model))
                {
                    if (model.backState == 0)//处理成功
                    {
             
                        result = "SUCCESS";
                        var yeepayLog = yeepayLogDao.Single(Convert.ToInt32(model.orderId));
                        var pay = payRecordDao.Single(yeepayLog.KeyId);
                        if (pay.State == 5)
                        {
                            Logs.WriteLog($"重复处理：orderId:{model.orderId},返回结果：{result}", "d:\\Log\\Teng", "NoticePayTeng");
                            return result;
                        }
                        flag = payRecordDao.UpdateState(yeepayLog.KeyId, 5, "支付成功，等待回调");
                        Logs.WriteLog($"修改订单状态：支付成功，等待回调,orderId:{model.orderId},flag:{flag}", "d:\\Log\\Teng", "NoticePayTeng");

                        yeepayLogDao.UpdateState(Convert.ToInt32(model.orderId), model.respCode, model.respMsg, 5);

                        //发起代付申请
                        var ret = ITOrm.Payment.Teng.TengDepository.DebitWithdraw(yeepayLog.ID, (int)Logic.Platform.系统);
                        Logs.WriteLog($"代付申请：json:{JsonConvert.SerializeObject(ret)}", "d:\\Log\\Teng", "NoticePayTeng");
                    }
                }
                else
                {
                    Logs.WriteLog($"签名比对失败：sign:{model.sign},orderId:{model.orderId}", "d:\\Log\\Teng", "NoticePayTeng");
                }
                Logs.WriteLog($"返回结果：{result}", "d:\\Log\\Teng", "NoticePayTeng");
                return result;
            }

        }

        //腾付通  代付回调
        public string NoticeWithTeng()
        {
            string[] paramtersKey = System.Web.HttpContext.Current.Request.Form.AllKeys;
            var sortedParamtersKey = from s in paramtersKey
                                     orderby s ascending
                                     select s;
            StringBuilder str = new StringBuilder();
            str.Append("{");
            foreach (string key in sortedParamtersKey)
            {
                str.AppendFormat("\"{0}\":\"{1}\",", key, System.Web.HttpContext.Current.Request.Form[key].Trim());
            }
            if (str.Length > 0)
            {
                str.Remove(str.Length - 1, 1);//移除最后一个逗号
            }
            str.Append("}");

            //返回后日志记录
            Logs.WriteLog("页面首次记录：" + str.ToString(), "d:\\Log\\Teng", "NoticeWithTeng");

            lock (lockNoticeWithTeng)
            {
                string result = "fail";
                respTengModel model = new respTengModel();
                model.version = TQuery.GetString("version");
                model.agentId = TQuery.GetString("agentId");
                model.merId = TQuery.GetString("merId");
                model.orderId = TQuery.GetString("orderId");
                model.respCode = TQuery.GetString("respCode");
                model.respMsg = TQuery.GetString("respMsg");
                model.sign = TQuery.GetString("sign");

                //string json = "{\"agentId\":\"A1000000009\",\"merId\":\"1000000010\",\"orderId\":\"100001261\",\"respCode\":\"00\",\"respMsg\":\"成功\",\"sign\":\"C3735B3E932CA3335ED7258E9DB21524\",\"version\":\"1.0.0\"}";
                //model = JsonConvert.DeserializeObject<respTengModel>(json);
                bool flag = false;
                //返回后日志记录
                Logs.WriteLog("回调参数：" + JsonConvert.SerializeObject(model), "d:\\Log\\Teng", "NoticeWithTeng");
                yeepayLogParasDao.Init(Convert.ToInt32(model.orderId), JsonConvert.SerializeObject(model), 2);

                if (TengDepository.isSign( model))
                {
                    if (model.backState == 0)//处理成功
                    {
                        result = "SUCCESS";
                        var yeepayLog = yeepayLogDao.Single(Convert.ToInt32(model.orderId));
                        var pay = payRecordDao.Single(yeepayLog.KeyId);
                        if (pay.State == 10)
                        {
                            Logs.WriteLog($"重复处理：State=10，orderId:{model.orderId},返回结果:{result}", "d:\\Log\\Teng", "NoticeWithTeng");
                            return result;
                        }
                        pay.State = 10;
                        pay.DrawState = 10;
                        pay.UTime = DateTime.Now;
                        pay.HandleTime = DateTime.Now;
                        flag = payRecordDao.Update(pay);
                        Logs.WriteLog($"修改支付订单结果：flag={flag}", "d:\\Log\\Teng", "NoticeWithTeng");
                        yeepayLogDao.UpdateState(Convert.ToInt32(model.orderId), model.respCode, model.respMsg, 10);
                    }
                }
                else
                {
                    Logs.WriteLog($"签名比对失败：sign:{model.sign}", "d:\\Log\\Teng", "NoticeWithTeng");
                }
                Logs.WriteLog($"返回结果：{result}", "d:\\Log\\Teng", "NoticeWithTeng");
                return result;
            }

        }


        //米刷回调
        public string MiShuaNotice()
        {
            HttpRequestBase request =HttpContext.Request;
            Stream stream = request.InputStream;
            string json = string.Empty;
            string responseJson = string.Empty;
            if (stream.Length != 0)
            {
                StreamReader streamReader = new StreamReader(stream);
                json = streamReader.ReadToEnd();
            
            }

            //返回后日志记录
            Logs.WriteLog("页面首次记录：" + json.ToString(), "d:\\Log\\MiShua", "MiShuaNotice");

            lock (lockMiShuaNotice)
            {
                string result = "fail";

                //json= "{\"payload\":\"mdvFvn5Aa9C + HONo6E9Sp7JmUGFaC5DTrwfmiB3d + sNpXUptNtMnKwy6tB2creQR2NxhGynCoTaQ85FbgDXlkDXRx8n0cPBaijKpWu2sTKjAm1kRUO / j7Qoh5TH0LR7UGVVJRPvnT5HvCVU9KizqfObhyaccjJnlHC9bq + mTdwaJ0gdJzACZhycS18vYDadrFyWyCaoPoShRzB3w3yGxIK6NhAY18iN9qjjWSsJWcfppzVCcu + yMiMnB8MZ8J2EGVohl6QlE4NrEQknEbWwcgwVV8xa07pBW2Z / QESzA6 / 7jO4e3ZSZcZ3SIartYZsrE7kG3vvWAYr2zFK745OUWeXSdNP0a5MpWP3K39RWWMYLfM7FKr9qAbuEl2vbGHjgC5zve5M3oiCPNuG / 5k5iDAzdgyElrP5PD / bF + E97hihNSK0zmhzZOZcXERZEzq6svBA7iSWKF1H2pCxLwtVNeacHHJIU0QR0VpTgyyEO2KV7JTDVBtVnLyZJhfQNY38r0CKE9Ysy22iruIq + pmBheGFi7tCvvaw + fJfIAykbUiW3lNHWlFImekq3h1KivAHJGhVFQfL2XjT + 7zxw2a9QhsOgwhaA5TvdtgtqLY + bkrvoG9UDo1A / IlRDV23SG5N50q0 / fvoL4QlZkseew5geu0OGvPWysurll87JqCGaQO3sewDEE9RIXyCuPPxCQTJ6qad636PyVANEYoK5PXVm116E03GTWp7 / +zGZXI + L5lgpJdFFuFfqbZ745PvKQ0GIrNxsx5ZaviB7yuos5IvVbG3F944pn / N31Jz5nBISQkx0zmQqJ8lZblue9XWPGC14T / 2 / 8E1y5lvuAc5EICRa9EfEYvOeH1NER3VnxexCeq2g\u003d\",\"state\":\"Successful\",\"sign\":\"3C1FF7EFED5D760F3CE4672DD7A4811B\",\"mchNo\":\"100445\",\"code\":0}";
                respModel<noticeMiShuaModel> model = new respModel<noticeMiShuaModel>();


                model = JsonConvert.DeserializeObject<respModel<noticeMiShuaModel>>(json);


                bool flag = false;
                //返回后日志记录
                Logs.WriteLog("回调参数：" + JsonConvert.SerializeObject(model), "d:\\Log\\MiShua", "MiShuaNotice");
                yeepayLogParasDao.Init(Convert.ToInt32(model.Data.tradeNo), JsonConvert.SerializeObject(model), 2);

                result = "SUCCESS";
                if (model.Data.status == "00" && model.Data.qfStatus == "SUCCESS")//处理成功
                {

                    var yeepayLog = yeepayLogDao.Single(Convert.ToInt32(model.Data.tradeNo));
                    var pay = payRecordDao.Single(yeepayLog.KeyId);
                    if (pay.DrawState == 10)
                    {
                        Logs.WriteLog($"重复处理：State=10，orderId:{model.Data.tradeNo},返回结果:{result}", "d:\\Log\\MiShua", "MiShuaNotice");
                        return result;
                    }
                    pay.State = 10;
                    pay.DrawState = 10;
                    pay.UTime = DateTime.Now;
                    pay.HandleTime = DateTime.Now;
                    flag = payRecordDao.Update(pay);
                    Logs.WriteLog($"代付成功：flag={flag}", "d:\\Log\\MiShua", "MiShuaNotice");
                    yeepayLogDao.UpdateState(Convert.ToInt32(model.Data.tradeNo), model.Data.status, model.Data.statusDesc, 10);
                }
                else if (model.Data.status == "00"&& model.Data.qfStatus != "SUCCESS")
                {
                    var yeepayLog = yeepayLogDao.Single(Convert.ToInt32(model.Data.tradeNo));
                    var pay = payRecordDao.Single(yeepayLog.KeyId);
                    if (pay.State == 5)
                    {
                        Logs.WriteLog($"重复处理：State=5，orderId:{model.Data.tradeNo},返回结果:{result}", "d:\\Log\\MiShua", "MiShuaNotice");
                        return result;
                    }
                    pay.State = 5;
                    pay.DrawState = 0;
                    pay.UTime = DateTime.Now;
                    pay.HandleTime = DateTime.Now;
                    flag = payRecordDao.Update(pay);
                    Logs.WriteLog($"支付成功：flag={flag}", "d:\\Log\\MiShua", "MiShuaNotice");
                    yeepayLogDao.UpdateState(Convert.ToInt32(model.Data.tradeNo), model.Data.status, model.Data.statusDesc, 5);
                }
                Logs.WriteLog($"返回结果：{result}", "d:\\Log\\MiShua", "MiShuaNotice");
                return result;
            }
        }
    }
}