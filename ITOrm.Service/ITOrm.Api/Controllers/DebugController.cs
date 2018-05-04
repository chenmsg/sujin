using ITOrm.Api.Filters;
using ITOrm.Core.Utility.Helper;
using ITOrm.Utility.ITOrmApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using ITOrm.Utility.Const;
using ITOrm.Payment.Yeepay;
using Newtonsoft.Json;
using ITOrm.Utility.StringHelper;
using ITOrm.Host.BLL;
using ITOrm.Utility.Client;
using ITOrm.Utility.Helper;
using ITOrm.Payment.Masget;
using ITOrm.Utility.Encryption;
using ITOrm.Payment.Const;
using ITOrm.Utility.Cache;
using ITOrm.Host.Models;
using ITOrm.Utility.Log;

namespace ITOrm.Api.Controllers
{
    public class DebugController : Controller
    {
        public static ITOrm.Host.BLL.PayRecordBLL payRecordDao = new Host.BLL.PayRecordBLL();
        public static UserBankCardBLL userBankCardDao = new UserBankCardBLL();
        public static KeyValueBLL keyValueDao = new KeyValueBLL();
        public static UsersBLL usersDao = new UsersBLL();
        #region 有效
        public ActionResult tool()
        {
            decimal Amount = TQuery.GetDecimal("Amount", 10000);
            decimal Rate1 = TQuery.GetDecimal("Rate1", 0.0038M);
            decimal Rate3 = TQuery.GetDecimal("Rate3", 2M);
            decimal BasicRate1 = TQuery.GetDecimal("BasicRate1", 0.0038M);
            decimal BasicRate3 = TQuery.GetDecimal("BasicRate3", 2M);
            ToolPay model = new ToolPay(Amount == 0M ? 10000 : Amount, Rate1 == 0M ? 0.0043M : Rate1, Rate3 == 0 ? 2 : Rate3, BasicRate1 == 0M ? 0.0038M : BasicRate1, BasicRate3 == 0M ? 2M : BasicRate3);
            return View(model);
        }

        [HttpGet]
        public ActionResult Optimal()
        {
            ResultModelData<int> model = new ResultModelData<int>();
            return View(model);
        }

        [HttpPost]
        public ActionResult Optimal(string BankCard, decimal Amount, int PayType)
        {
            var model = userBankCardDao.Single("BankCard=@BankCard", new { BankCard });
            return View(SelectOptionChannel.Optimal(model.UserId, Amount, BankCard, PayType));
        }
        // GET: DeBug
        public string Version()
        {
            JObject dic = new JObject();
            //Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("Version", "1.0.0.1");
            dic.Add("VersionDate", "2016-7-7");
            return ApiReturnStr.getApiData(dic);
        }

        [Aysn(Setting = true)]
        public string tb()
        {
            return ApiReturnStr.getApiData();
        }
        public string yb()
        {
            return ApiReturnStr.getApiData();
        }
        #endregion


        public ActionResult ewm()
        {
            var b = QrCodeHelper.Instance.QrCodeCreate("http://www.baidu.com");
            return File(b, "image/jpeg");
        }

        public string sss()
        {
            string accountName = HttpUtility.UrlDecode(Request["accountName"]);
            string sss = Request["sss"];

            byte[] bytes = null;
            using (var binaryReader = new BinaryReader(Request.Files["bankCardPhoto"].InputStream))
            {
                bytes = binaryReader.ReadBytes(Request.Files["bankCardPhoto"].ContentLength);
            }
            string b = System.Text.Encoding.Default.GetString(bytes);
            return ApiReturnStr.getError(100, "开户成功");
        }
        public string aa()
        {
            string path = "D:\\test.jpg";
            string result = "";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("accountName", HttpUtility.UrlEncode("掉单"));
            nvc.Add("sss", "asdfasdf");
            string[] filePath = new string[] { "D:\\cx\\ITOrm\\ITOrm.Service\\ITOrm.Api\\upload/users/20180206/20180206110632986.jpg" };
            string[] fileKeyName = new string[] { "bankCardPhoto" };

            int s = ITOrm.Utility.Client.HttpHelper.HttpPostData("http://api.itorm.com/itapi/debug/sss", Encoding.UTF8, filePath, fileKeyName, nvc, out result);

            return result;
        }


        public string logs()
        {
            Utility.Log.Logs.WriteLog($"获取请求流水号：UserId:111", "d:\\Log\\Yeepay", "哈哈");
            Utility.Log.Logs.WriteLog($"获取请求流水号：UserId:111", "d:\\Log\\Yeepay", "哈哈", Utility.Log.Logs.LogType.Hour);
            return "";
        }

        public string AuditMerchant()
        {
            var result = YeepayDepository.AuditMerchant(100000, 0, Payment.Yeepay.Enums.AuditMerchant.SUCCESS);
            return JsonConvert.SerializeObject(result);
        }

        public string FeeSetApi()
        {
            var result = YeepayDepository.FeeSetApi(100000, 0, Payment.Yeepay.Enums.YeepayType.设置费率1, "0.03");
            return JsonConvert.SerializeObject(result);
        }


        public string receiveApi()
        {
            var result = YeepayDepository.ReceiveApi(100058, 100, 0, "6259588903947587");
            return JsonConvert.SerializeObject(result);
        }

        public string WithDrawApi()
        {
            var result = YeepayDepository.WithDrawApi(100003, 0);
            return JsonConvert.SerializeObject(result);
        }

        public string sendmsg()
        {
            var resultMsg = Utility.Message.SystemSendMsg.Send(Logic.EnumSendMsg.忘记密码短信, "15110167786");
            return JsonConvert.SerializeObject(resultMsg);
        }

        public string TradeReviceQuery()
        {
            var result = YeepayDepository.TradeReviceQuery("", 0);
            return JsonConvert.SerializeObject(result);
        }


        public string transferQuery()
        {
            var result = YeepayDepository.TransferQuery(100000, "", 1, 6);
            return JsonConvert.SerializeObject(result);
        }


        public string customerInforQuery()
        {
            var result = YeepayDepository.CustomerInforQuery(100000, 0);
            return JsonConvert.SerializeObject(result);
        }

        public string CustomerBalanceQuery()
        {
            var result = YeepayDepository.CustomerBalanceQuery(100000, 0);
            return JsonConvert.SerializeObject(result);
        }





        public string asdfs()
        {
            UserEventRecordBLL userEventDao = new UserEventRecordBLL();
            bool num = userEventDao.RealNameAuthentication(0, 100000, Ip.GetClientIp(), "511112199101312410", "陈鑫", 0);

            return num.ToString();
        }

        public string rate1()
        {
            var model = YeepayDepository.FeeSetApi(100000, 1, Payment.Yeepay.Enums.YeepayType.设置费率1, "0.0043");
            return JsonConvert.SerializeObject(model);
        }



        public string rate4()
        {
            var model = YeepayDepository.FeeSetApi(100000, 1, Payment.Yeepay.Enums.YeepayType.设置费率4, "0");
            return JsonConvert.SerializeObject(model);
        }
        public string rate5()
        {
            var model = YeepayDepository.FeeSetApi(100000, 1, Payment.Yeepay.Enums.YeepayType.设置费率5, "0");
            return JsonConvert.SerializeObject(model);
        }

        public string isaudit()
        {
            var model = YeepayDepository.AuditMerchant(100000, 1, Payment.Yeepay.Enums.AuditMerchant.SUCCESS, "审核通过");

            return JsonConvert.SerializeObject(model);
        }

        public string liuhuimin()
        {
            int UserId = 100056;
            YeepayDepository.FeeSetApi(UserId, 1, Payment.Yeepay.Enums.YeepayType.设置费率1, "0.0043");
            YeepayDepository.FeeSetApi(UserId, 1, Payment.Yeepay.Enums.YeepayType.设置费率4, "0");
            YeepayDepository.FeeSetApi(UserId, 1, Payment.Yeepay.Enums.YeepayType.设置费率5, "0");
            var model = YeepayDepository.AuditMerchant(UserId, 1, Payment.Yeepay.Enums.AuditMerchant.SUCCESS, "审核通过");
            return JsonConvert.SerializeObject(model);
        }


        public string jiesuan()
        {
            var model = YeepayDepository.WithDrawApi(100009, 1);
            return JsonConvert.SerializeObject(model);
        }


        public string ValidateBank()
        {
            var s = BankCardBindHelper.ValidateBank("招商银行1", "370286001882411");
            return s.ToString();
        }

        public string ab()
        {
            int a = 0;
            int b = 0;
            return (a / b).ToString();
        }

        public string setfee(int UserId)
        {
            var result1 = YeepayDepository.FeeSetApi(UserId, 1, Payment.Yeepay.Enums.YeepayType.设置费率1, "0.0043");
            var result2 = YeepayDepository.FeeSetApi(UserId, 1, Payment.Yeepay.Enums.YeepayType.设置费率3, "2");
            return JsonConvert.SerializeObject(result1) + JsonConvert.SerializeObject(result2);
        }

        public string SubcompanyAdd()
        {
            var result = MasgetDepository.SubcompanyAdd(100058, 1, Logic.ChannelType.荣邦科技积分);
            return JsonConvert.SerializeObject(result);
        }

        public string SamenameOpen()
        {
            var result = MasgetDepository.SamenameOpen(100058, 1, Logic.ChannelType.荣邦科技积分);
            return JsonConvert.SerializeObject(result);
        }


        /// <summary>
        /// 生成图形验证码
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetImgCode(int width = 73, int height = 28, string guid = "")
        {

            int fontsize = 20;
            string code = string.Empty;
            byte[] bytes = ValidateCode.CreateValidateGraphic(out code, 4, width, height, fontsize);
            return File(bytes, @"image/jpeg");
        }

        public string ssssdfasdf()
        {
            Logic.ChannelType chanel = Logic.ChannelType.荣邦科技积分;
            int a = (int)chanel;
            return a.ToString();
        }

        public string TreatyApply(int id)
        {
            var result = MasgetDepository.TreatyApply(id, 1, Logic.ChannelType.荣邦科技积分);
            return JsonConvert.SerializeObject(result);
        }

        public string TreatyConfirm(int id = 0, string code = "")
        {
            int ubkId = 100009;
            var result = MasgetDepository.TreatyConfirm(id, code, 1, Logic.ChannelType.荣邦科技积分);
            return JsonConvert.SerializeObject(result);
        }

        public string BackPay(int ubkid)
        {
            var result = MasgetDepository.BackPay(ubkid, 500, 1, Logic.ChannelType.荣邦科技积分);
            return JsonConvert.SerializeObject(result);
        }

        public string PayConfirmpay(int payId = 0, string code = "")
        {

            var result = MasgetDepository.PayConfirmpay(payId, code, 1, Logic.ChannelType.荣邦科技积分);
            return JsonConvert.SerializeObject(result);

        }

        public string SamenameUpdate(int UserId)
        {
            var result = MasgetDepository.SamenameUpdate(UserId, 1, Logic.ChannelType.荣邦科技积分, Logic.VipType.顶级用户);
            return JsonConvert.SerializeObject(result);
        }

        public string aaaaaa()
        {
            var result = YeepayDepository.CustomerInforUpdate(100098, 1, "6214920201239124", "光大银行");
            return JsonConvert.SerializeObject(result);
        }
        public string ssss()
        {
            int[] aa = new int[] {100058
,100060
,100063
,100067
,100080 };
            foreach (var item in aa)
            {
                MasgetDepository.SamenameUpdate(item, 1, Logic.ChannelType.荣邦科技积分, Logic.VipType.顶级用户);
            }
            return "";
        }




        public string debitWithdraw()
        {
            var result = ITOrm.Payment.Teng.TengDepository.DebitWithdraw(100001261, 1);
            return JsonConvert.SerializeObject(result);
        }

        public string PayDebitQuery()
        {
            var result = ITOrm.Payment.Teng.TengDepository.PayDebitQuery(100001261, 1);
            return JsonConvert.SerializeObject(result);
        }

        public string DebitWithdrawQuery()
        {
            var result = ITOrm.Payment.Teng.TengDepository.DebitWithdrawQuery(100001261, 1);
            return JsonConvert.SerializeObject(result);
        }


        public string CreatePayCashier()
        {
            var result = ITOrm.Payment.Teng.TengDepository.CreatePayCashier(100058, 1, 100, 100016);
            return JsonConvert.SerializeObject(result);
        }


        public static TimedTaskBLL timedTaskDao = new TimedTaskBLL();

        public string asaa()
        {
            var stime = "08:00:00";
            DateTime execTime = Convert.ToDateTime(DateTime.Now.ToString($"yyyy-MM-dd {stime}")).AddDays(1);
            int keyId = 10009;//腾付通通道记录ID
            JObject value = new JObject();
            value["keyId"] = keyId;
            timedTaskDao.Init(Logic.TimedTaskType.通道开启, execTime, value.ToString());
            return "";
        }

        public string aaadsfadsf()
        {
            var list = usersDao.GetQuery(" VipType=0 ");
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    var result1= YeepayDepository.FeeSetApi(item.UserId, 1, Payment.Yeepay.Enums.YeepayType.设置费率1, "0.0041");
                   var result2 = MasgetDepository.SamenameUpdate(item.UserId, 1, Logic.ChannelType.荣邦科技积分, Logic.VipType.顶级用户);
                }
            }
            return "";
        }
    }
}