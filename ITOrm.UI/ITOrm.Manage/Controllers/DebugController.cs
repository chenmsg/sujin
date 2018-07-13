using ITOrm.Utility.Cache;
using ITOrm.Utility.Helper;
using ITOrm.Utility.ITOrmApi;
using ITOrm.Utility.StringHelper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using ITOrm.Manage.Filters;
using ITOrm.Host.Models;
using ITOrm.Host.BLL;
using ITOrm.Utility.Const;
using ITOrm.Payment.Const;

namespace ITOrm.Manage.Controllers
{
    public class DebugController : Controller
    {
        // GET: Debug
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult DebugApi()
        {

            return View();

        }

        [HttpPost]
        public ActionResult DebugApi(string lcturl, string username, string password, string strMd5Key, string Target, string Param, string version)
        {
            var model = ApiRequest.RequestApiTest(lcturl, username, password, strMd5Key, Target, Param, version, "POST", "");
            return View(model);
        }

        [HttpPost]
        public ActionResult UrlEncode(string text)
        {
            string context = System.Web.HttpUtility.UrlEncode(text);
            return Content(context);
        }
        [AdminFilter]
        public ActionResult Memcached()
        {
            var keyName = TQuery.GetString("keyName");
            var cmd = TQuery.GetString("cmd");

            ResultModel result = new ResultModel();
            result.data = new JObject();
            result.data["keyName"] = keyName;
            result.data["keyValue"] = "";
            if (cmd == "查询")
            {
                if (!string.IsNullOrEmpty(keyName))
                {
                    if (!MemcachHelper.Exists(keyName))
                    {
                        result.backState = -100;
                        result.message = "键值不存在";
                    }
                    else
                    {

                        result.data["keyValue"] = JsonConvert.SerializeObject(MemcachHelper.Get(keyName));
                        result.data["keyName"] = keyName;
                        result.message = "查询成功";
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(keyName))
                {
                    if (!MemcachHelper.Exists(keyName))
                    {
                        result.backState = -100;
                        result.message = "键值不存在";
                    }
                    else
                    {
                        result.backState = -100;
                        MemcachHelper.Delete(keyName);
                        result.message = "删除成功";
                    }
                }
            }

            return View(result);
        }


        public static UsersBLL userDao = new UsersBLL();

        public static AccountBLL accountDao = new AccountBLL();
        public static PayRecordBLL payRecordDao = new PayRecordBLL();
        public ActionResult CreateUser(string RealName, string Mobile, int BaseUserId, int VipType)
        {

            if (string.IsNullOrEmpty(RealName) || string.IsNullOrEmpty(Mobile) || BaseUserId == 0 )
            {
                return new RedirectResult($"/Prompt?state=-100&msg=参数错误&url=/debug/");
            }
            var model = new Users();
            model.BaseUserId = BaseUserId;
            model.CTime = DateTime.Now;
            model.Email = "";
            model.IdCard = "xxxxxxxxxxxxxxxxxxxx";
            model.IP = ITOrm.Utility.Client.Ip.GetClientIp();
            model.IsRealState = 1;
            model.Mobile = Mobile;
            model.Password = "0b4e7a0e5fe84ad35fb5f95b9ceeac79";
            model.PlatForm = 1;
            model.RealName = "";
            model.Soure = "";
            model.State = 0;
            model.RealName = RealName;
            model.UserName = Mobile;
            model.UTime = DateTime.Now;
            model.RealTime = DateTime.Now;
            model.VipType = VipType;
            var result = userDao.Insert(model);
            var account = new Account();
            account.UserId = result;
            account.CTime = DateTime.Now;
            account.UTime = DateTime.Now;
            account.Frozen = 0m;
            account.Available = 0m;
            account.Total = 0m;
            var resultAccount = accountDao.Insert(account);
            int backState = result > 0 ? 0 : -100;
            string message = result > 0 ? "操作成功" : "操作失败";
            return new RedirectResult($"/Prompt?state={backState}&msg={message}&url=/debug/");
        }


        public ActionResult Pay(int UserId,decimal Amount=0)
        {

            if (UserId == 0 || Amount < 500M)
            {
                return new RedirectResult($"/Prompt?state=-100&msg=参数错误&url=/debug/");
            }

            var user = userDao.Single(UserId);
        

            Logic.VipType vip = (Logic.VipType)user.VipType;
            decimal[] r = Constant.GetRate(0, vip);
            ToolPay tp = new ToolPay(Amount, r[0], r[1], 0.0038M, 1M);
            PayRecord model = new PayRecord();
            model.UserId = UserId;
            model.Amount = Amount;
            model.Platform = 1;
            model.Ip = "1.1.1.1";
            model.BankCard = "622202XXXXXXXXXXXXXX";
            model.Fee = tp.PayFee;
            model.Rate = tp.Rate1;
            model.Fee3 = tp.Rate3;
            model.State = 10;
            model.DrawState = 10;
            model.WithDrawAmount = model.Amount - model.Fee;//结算金额
            model.ActualAmount = tp.ActualAmount;
            model.DrawBankCard = "622222XXXXXX";
            model.BankCode = "ICBC";
            model.PayerName = user.RealName;
            model.PayerPhone = user.Mobile;
            model.ChannelType = 4;
            model.Income = tp.Income;
            model.DrawIncome = tp.Rate3 - 1;//结算收益

            var result= payRecordDao.Insert(model);

            int backState = result > 0 ? 0 : -100;
            string message = result > 0 ? "操作成功" : "操作失败";
            //交易成功回调
            UsersDepository.NoticeSuccess(result, UserId);

            return new RedirectResult($"/Prompt?state={backState}&msg={message}&url=/debug/");
        }



    }
}