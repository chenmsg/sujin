using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITOrm.Host.Models;
using ITOrm.Host.BLL;
using Newtonsoft.Json;
using ITOrm.Payment.Masget;
using ITOrm.Utility.Helper;
using Newtonsoft.Json.Linq;
using ITOrm.Utility.Const;
using ITOrm.Utility.Client;
using ITOrm.Payment.Teng;
namespace ITOrm.Api.Controllers
{
    public class PayController : Controller
    {
        public static PayCashierBLL payCashierDao = new PayCashierBLL();
        public static PayRecordBLL payRecordDao = new PayRecordBLL();
        public static UserBankCardBLL userBankCardDao = new UserBankCardBLL();
        public static UserEventRecordBLL userEventDao = new UserEventRecordBLL();
        // GET: Pay
        [HttpGet]
        public ActionResult Cashier(int payid)
        {




            var payCashier = payCashierDao.Single(payid);

            respBackPayModel model = JsonConvert.DeserializeObject<respBackPayModel>(payCashier.Value);
            UserBankCard ubk = userBankCardDao.Single(payCashier.UbkId);
            JObject data = new JObject();
            data["respBackPay"] = JObject.FromObject(model);
            data["ubk"] = JObject.FromObject(ubk);
            ResultModel result = new ResultModel(data);

            if (payCashier.State == 1)
            {
                result.backState = -200;
                result.message = "订单已支付完成";
            }
            else if (payCashier.State == -1)
            {
                result.backState = -200;
                result.message = "订单已过期";
            }

            return View(result);
        }

        [HttpPost]
        public ActionResult Cashier(int payid, string code)
        {
            var payCashier = payCashierDao.Single(payid);
            respBackPayModel model = JsonConvert.DeserializeObject<respBackPayModel>(payCashier.Value);
            UserBankCard ubk = userBankCardDao.Single(payCashier.UbkId);
            JObject data = new JObject();
            data["respBackPay"] = JObject.FromObject(model);
            data["ubk"] = JObject.FromObject(ubk);
            ResultModel result = new ResultModel(data);

            if (!ITOrm.Utility.StringHelper.TypeParse.IsNumeric(code))
            {
                result.backState = -100;
                result.message = "验证码必须为数字";
                return View(result);
            }
            if (payCashier.State == 1)
            {
                result.backState = -200;
                result.message = "订单已支付完成";
                return View(result);
            }
            else if (payCashier.State == -1)
            {
                result.backState = -200;
                result.message = "订单已过期";
                return View(result);
            }



            var payResult = MasgetDepository.PayConfirmpay(payid, code, (int)Logic.Platform.系统, (Logic.ChannelType)payCashier.ChannelType);
            userEventDao.UserEventInit((int)Logic.Platform.系统, payCashier.UserId, Ip.GetClientIp(), payResult.backState == 0 ? 1 : 0, "Pay", "Cashier", $"{{payid:{payid},code:{code}}}");
            if (payResult.backState == 0)
            {
                result.backState = -200;
                result.message = "支付成功";
                return new RedirectResult("/itapi/pay/success?backState=0&message=支付成功");
            }
            else
            {
                result.backState = -100;
                result.message = payResult.message;
                return View(result);
            }
        }



        [HttpGet]
        public ActionResult TengCashier(int payid)
        {

            var payCashier = payCashierDao.Single(payid);
            var pay = payRecordDao.Single(payCashier.PayRecordId);
            var ubk = userBankCardDao.Single(payCashier.UbkId);

            JObject data = new JObject();
            data["amount"] = pay.Amount.ToString("F2");
            data["ordernumber"] = payCashier.LogId;
            data["BankCode"] = ubk.BankCode;
            data["BankName"] = ubk.BankName;
            data["BankCard"] = ubk.BankCard;
            data["Mobile"] = ITOrm.Utility.StringHelper.Util.GetHiddenString(ubk.Mobile, 3, 4);
            ResultModel result = new ResultModel(data);

            if (payCashier.State == 1)
            {
                result.backState = -200;
                result.message = "订单已支付完成";
            }
            else if (payCashier.State == -1)
            {
                result.backState = -200;
                result.message = "订单已过期";
            }

            return View(result);
        }

        [HttpPost]
        public ActionResult TengCashier(int payid,string tengGuid, string code)
        {
            var payCashier = payCashierDao.Single(payid);
            var pay = payRecordDao.Single(payCashier.PayRecordId);
            var ubk = userBankCardDao.Single(payCashier.UbkId);

            JObject data = new JObject();
            data["amount"] = pay.Amount.ToString("F2");
            data["ordernumber"] = payCashier.LogId;
            data["BankCode"] = ubk.BankCode;
            data["BankName"] = ubk.BankName;
            data["BankCard"] = ubk.BankCard;
            data["Mobile"] = ITOrm.Utility.StringHelper.Util.GetHiddenString(ubk.Mobile, 3, 4);
            ResultModel result = new ResultModel(data);

            if (!ITOrm.Utility.StringHelper.TypeParse.IsNumeric(code))
            {
                result.backState = -100;
                result.message = "验证码必须为数字";
                return View(result);
            }
            if (tengGuid.Length != 36)
            {
                result.backState = -100;
                result.message = "未获取短信";
                return View(result);
            }
            if (payCashier.State == 1)
            {
                result.backState = -200;
                result.message = "订单已支付完成";
                return View(result);
            }
            else if (payCashier.State == -1)
            {
                result.backState = -200;
                result.message = "订单已过期";
                return View(result);
            }

            
            var validResult = TengDepository.ValidateMobileCode(payid, (int)Logic.Platform.系统, tengGuid, code);
            if (validResult.backState == 0)//短信验证成功
            {
                //respTengModel payResult = new respTengModel();
                //payResult.respCode = "00";
                //payResult.respMsg = "支付成功";
                 var payResult = TengDepository.DebitPayment(payid, (int)Logic.Platform.系统);
                //var payResult = JsonConvert.DeserializeObject<respTengModel>("{\"respCode\":\"E99999\",\"respMsg\":\"支付卡已超过有效期\",\"sign\":\"B231ED5797458C3D02374D14317E59EA\"}");
                userEventDao.UserEventInit((int)Logic.Platform.系统, payCashier.UserId, Ip.GetClientIp(), payResult.backState == 0 ? 1 : 0, "Pay", "TengCashier", $"{{payid:{payid},code:{code},tengGuid:{tengGuid}}}");
                if (payResult.backState == 0)//支付成功
                {
                    result.backState = 0;
                    result.message = "支付成功";
                    return new RedirectResult("/itapi/pay/success?backState=0&message=支付成功");
                }
                result.backState = -100;
                result.message = payResult.respMsg;
                return View(result);
            }
            else//短信验证失败
            {
                result.backState = -100;
                result.message = validResult.message;
                return View(result);
            }
       
          
          
        
        }

        public string TengSendMsgCode(int payid)
        {
            var result= TengDepository.SendMsgCode(payid, (int)Logic.Platform.系统, Guid.NewGuid().ToString());
            return JsonConvert.SerializeObject(result);
        }




        public string Success()
        {
            return "";
        }
    }
}