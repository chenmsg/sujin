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

namespace ITOrm.Api.Controllers
{
    public class ServerController : Controller
    {
        public static YeepayLogBLL yeepayLogDao = new YeepayLogBLL();
        public static YeepayLogParasBLL yeepayLogParasDao = new YeepayLogParasBLL();
        public static YeepayUserBLL yeepayUserDao = new YeepayUserBLL();
        public static UsersBLL usersDao = new UsersBLL();
        public static UserBankCardBLL userBankCardDao = new UserBankCardBLL();
        public static PayRecordBLL payRecordDao = new PayRecordBLL();
        public static WithDrawBLL withDrawDao = new WithDrawBLL();
        // GET: Server
        public string Register()
        {
            return "{ \"code\": \"0000\", \"message\": \"开户成功\",\"rate\":\"0.02\",\"customerNumber\":\"123456\",\"productType\":\"1\"}";
        }

        public string auditMerchant()
        {
            return "{ \"code\": \"0000\", \"message\": \"开户成功\",\"rate\":\"0.02\",\"customerNumber\":\"123456\",\"productType\":\"1\"}";
        }

        public string FeeSetApi()
        {
            return "{ \"code\": \"0000\", \"message\": \"开户成功\",\"rate\":\"0.002\",\"customerNumber\":\"123456\",\"productType\":\"2\"}";
        }

        public string receiveApi()
        {
            string url = ITOrm.Utility.Encryption.EncryptionHelper.AESEncrypt("https://www.baidu.com", ITOrm.Payment.Yeepay.YeepayDepository.YeepayHmacKey.Substring(0, 16));
            string requestId = TQuery.GetInput("requestId");
            return "{ \"code\": \"0000\", \"message\": \"收款发起成功\",\"mainCustomerNumber\":\"1111\",\"customerNumber\":\"123456\",\"url\":\"" + url + "\",\"requestId\":\"" + requestId + "\"}";
        }


        //api.itorm.com/itapi/server/receiveApiNotice?requestId=100000029&amount=100
        public string receiveApiNotice()
        {
            string result = "fail";
            bool flag = false;
            string requestId = TQuery.GetInput("requestId");
            noticeReceiveApiModel model = new noticeReceiveApiModel();
            model.code = "0000";
            model.message = "支付成功";
            model.amount = TQuery.GetDecimal("amount", 0m).ToString("F2");
            model.fee = (TQuery.GetDecimal("amount", 0m) * 0.002m).ToString("F2");
            model.externalld = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            model.payerPhone = "15110167786";
            model.payerName = "陈鑫";
            model.payTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            model.requestId = requestId;
            model.src = "B";
            model.status = "SUCCESS";
            model.lastNo = "0239";
            model.bankCode = "icbc";
            model.busiType = "COMMON";
            model.customerNumber = "123456";

            //返回后日志记录
            Logs.WriteLog("回调参数：" + JsonConvert.SerializeObject(model), "d:\\Log\\Yeepay", "ReceiveApi");
            yeepayLogParasDao.Init(Convert.ToInt32(requestId), JsonConvert.SerializeObject(model), 2);

            //更新
            yeepayLogDao.UpdateState(Convert.ToInt32(requestId), model.code, model.message, (model.backState == 0 && model.status == "SUCCESS") ? 10 : -1);

            if (model.backState == 0 && model.status == "SUCCESS")//成功
            {
                var yeepayLog = yeepayLogDao.Single(Convert.ToInt32(requestId));
                var payRecord = payRecordDao.Single(yeepayLog.KeyId);
                if (payRecord.State == 1)
                {
                    result = "SUCCESS";
                    return result;
                }
                //修改订单信息
                payRecord.PayTime = Convert.ToDateTime(model.payTime);
                payRecord.State = 1;
                payRecord.PayerPhone = model.payerPhone;
                payRecord.PayerName = model.payerName;
                payRecord.LastNo = model.lastNo;
                payRecord.Message = model.message;
                payRecord.BankCode = model.bankCode;
                payRecord.Fee = Convert.ToDecimal(model.fee);
                payRecord.Src = model.src;
                flag = payRecordDao.Update(payRecord);
                //添加支付银行卡
                int ubkCnt = userBankCardDao.Count(" UserId=@UserId and TypeId=0 and State=1 and  BankCard=@BankCard ", new { payRecord.UserId, payRecord.BankCard });
                if (ubkCnt == 0)
                {
                    UserBankCard ubk = new UserBankCard();
                    ubk.BankCard = payRecord.BankCard;
                    ubk.TypeId = 1;
                    ubk.State = 1;
                    ubk.UserId = payRecord.UserId;
                    ubk.Mobile = model.payerPhone;
                    ubk.IP = ITOrm.Utility.Client.Ip.GetClientIp();
                    ubk.Platform = 1;
                    ubk.BankCode = model.bankCode;
                    userBankCardDao.Insert(ubk);
                }

                if (flag) result = "SUCCESS";
            }
            return result;
        }

        public string withDrawApi()
        {
            string externalNo = TQuery.GetInput("externalNo");
            string amount = TQuery.GetInput("amount");
            return "{ \"code\": \"0000\", \"message\": \"收款发起成功\",\"serialNo\":\"1111\",\"customerNumber\":\"123456\",\"amount\":\"" + amount + "\",\"externalNo\":\"" + externalNo + "\",\"mainCustomerNumber\":\"1111\",\"transferWay\":\"1\",\"hmac\":\"21212\"}";
        }



        public string TradeReviceQuery()
        {
            respTradeReviceQueryModel resp = new respTradeReviceQueryModel();
            resp.code = "0000";
            resp.message = "查询成功";
            List<tradeReceivesModel> list = new List<tradeReceivesModel>();
            for (int i = 0; i < 10; i++)
            {
                tradeReceivesModel model = new tradeReceivesModel();
                model.amount = "500";
                model.payerName = "陈鑫";
                list.Add(model);
            }
            resp.tradeReceives = list;
            return JsonConvert.SerializeObject(resp);
        }

        public string transferQuery()
        {
            respTransferQueryModel resp = new respTransferQueryModel();
            resp.code = "0000";
            resp.message = "查询成功";
            List<transferRequstsModel> list = new List<transferRequstsModel>();
            for (int i = 0; i < 10; i++)
            {
                transferRequstsModel model = new transferRequstsModel();
                model.amount = "500";
                model.actualAmount = "480";
                list.Add(model);
            }
            resp.transferRequests = list;
            return JsonConvert.SerializeObject(resp);
        }

        public string customerBalanceQuery()
        {
            respCustomerBalanceQueryModel model = new respCustomerBalanceQueryModel();
            model.balanceType = "1";
            model.balance = "8800";
            model.customerNumber = "123456";
            model.code = "0000";
            model.message = "请求成功";
            return JsonConvert.SerializeObject(model);
        }
        public string withDrawApiNotice()
        {
            bool flag = false;
            string result = "fail";
            noticeWithDrawApiModel model = new noticeWithDrawApiModel();
            model.externalNo = "100000030";
            model.serialNo = "2342423423";
            model.transferStatus = "SUCCESSED";
            model.requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            model.handleTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            model.transferWay = "1";
            model.receiver = "陈鑫";
            model.receiverBankCardNo = "6222020200083410239";
            model.receiverBank = "工商银行";
            model.amount = "100";
            model.fee = "2";
            model.basicFee = "3";
            model.exTargetFee = "4";
            model.actualAmount = "91";
            model.failReason = "成功";


            //返回后日志记录
            Logs.WriteLog("回调参数：" + JsonConvert.SerializeObject(model), "d:\\Log\\Yeepay", "WithDrawApi");
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
            if (draw.State == 10)
            {
                result = "SUCCESS";
                return result;
            }


            //修改订单信息
            draw.UTime = DateTime.Now;
            draw.State = state;
            draw.Message = model.failReason;
            if (state == 10)
            {
                draw.Receiver = model.receiver;
                draw.ReceiverBankCardNo = model.receiverBankCardNo;
                draw.ReceiverBank = model.receiverBank;
                draw.Fee = Convert.ToDecimal(model.fee);
                draw.BasicFee = Convert.ToDecimal(model.basicFee);
                draw.ExTargetFee = Convert.ToDecimal(model.exTargetFee);
                draw.ActualAmount = Convert.ToDecimal(model.actualAmount);
                flag = withDrawDao.Update(draw);
            }
            //添加支付银行卡
            int ubkCnt = withDrawDao.Count(" UserId=@UserId and TypeId=0 and State=1 and  BankCard=@BankCard ", new { draw.UserId, draw.ReceiverBankCardNo });
            if (ubkCnt == 0)
            {
                UserBankCard ubk = new UserBankCard();
                ubk.BankCard = model.receiverBankCardNo;
                ubk.TypeId = 0;
                ubk.State = 1;
                ubk.UserId = draw.UserId;
                //ubk.Mobile = model.payerPhone;
                ubk.IP = ITOrm.Utility.Client.Ip.GetClientIp();
                ubk.Platform = 1;
                //ubk.BankCode = model.bankCode;
                userBankCardDao.Insert(ubk);
            }

            if (flag) result = "SUCCESS";

            return result;
        }


        public string customerInforQuery()
        {
            respCustomerInforQueryModel model = new respCustomerInforQueryModel();
            model.code = "0000";
            return JsonConvert.SerializeObject(model);
        }
    }
}