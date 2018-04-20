using ITOrm.Host.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITOrm.Utility.Helper;
using ITOrm.Host.Models;
using Newtonsoft.Json.Linq;
using ITOrm.Payment.Yeepay;
using Newtonsoft.Json;
using ITOrm.Manage.Filters;
using ITOrm.Utility.Const;
using ITOrm.Utility.Cache;

namespace ITOrm.Manage.Controllers
{
    public class ChannelPayController : Controller
    {
        public KeyValueBLL keyValueDao = new KeyValueBLL();

        //支付通道
        // GET: ChannelPay
        public ActionResult Index(int pageIndex = 1)
        {
            int totalCount = 0;
            var list = keyValueDao.GetPaged(10, pageIndex, out totalCount, "  typeid=2 ", null, "order by Sort desc,CTime desc");
            return View(new ResultModel<KeyValue>(list, totalCount));
        }

        [HttpGet]
        public ActionResult Edit(int Id = 0)
        {
            KeyValue kv = new KeyValue();
            if (Id > 0)
            {
                kv = keyValueDao.Single(Id);
            }
            return View(kv);
        }
        [HttpPost]
        public ActionResult Edit(int Id = 0, int KeyId = 0, int Sort = 0, int State = 0, string Value2="0",string Remark="",string StartTime="",string EndTime="",decimal Rate1=0M,decimal Rate3=0M)
        {
            KeyValue kv = new KeyValue();

            if (Id > 0)
            {
                kv = keyValueDao.Single(Id);
                kv.UTime = DateTime.Now;
            }
            JObject data = new JObject();
            data["StartTime"] = StartTime;
            data["EndTime"] = EndTime;
            data["Rate1"] = Rate1.ToString("F4");
            data["Rate3"] = Rate3.ToString("F4");
            kv.TypeId = (int)Logic.KeyValueType.支付通道管理;
            kv.KeyId = KeyId;
            kv.Value = data.ToString();
            kv.Value2 = Value2;
            kv.Sort = Sort;
            kv.State = State;
            kv.Remark = Remark;
            bool flag = false;

            if (kv.ID > 0)
            {
                flag = keyValueDao.Update(kv);
            }
            else
            {
                flag = keyValueDao.Insert(kv) > 0;
            }
            int state = flag ? 0 : -100;
            string msg = flag ? "操作成功" : "操作失败";
            string url = "/channelpay/";
            MemcachHelper.Delete(Constant.list_keyvalue_key+ (int)Logic.KeyValueType.支付通道管理);//清理缓存
            return new RedirectResult($"/Prompt?state={state}&msg={msg}&url={url}");

        }


        //支付类型管理
        public ActionResult PayTypeList(int pageIndex = 1)
        {
            int totalCount = 0;
            var list = keyValueDao.GetPaged(10, pageIndex, out totalCount, "  typeid=3 ", null, "order by Sort desc,CTime desc");
            return View(new ResultModel<KeyValue>(list, totalCount));
        }
        [HttpGet]
        public ActionResult PayTypeEdit(int Id = 0)
        {
            KeyValue kv = new KeyValue();
            if (Id > 0)
            {
                kv = keyValueDao.Single(Id);
            }
            return View(kv);
        }
        [HttpPost]
        public ActionResult PayTypeEdit(int Id = 0, int Sort = 0,int PayType=0, string PayName = "", string Quota = "", string Fee = "", string WithDraw = "", string Time = "", string Remark = "")
        {
            KeyValue kv = new KeyValue();

            if (Id > 0)
            {
                kv = keyValueDao.Single(Id);
                kv.UTime = DateTime.Now;
            }
            kv.TypeId = (int)Logic.KeyValueType.支付类型管理;
            JObject data = new JObject();
            data["PayType"] = PayType;
            data["PayName"] = PayName;
            data["Quota"] = Quota;
            data["Fee"] = Fee;
            data["WithDraw"] = WithDraw;
            data["Time"] = Time;
            data["Remark"] = Remark;
            kv.Sort = Sort;
            kv.Value = data.ToString();
            bool flag = false;

            if (kv.ID > 0)
            {
                flag = keyValueDao.Update(kv);
            }
            else
            {
                flag = keyValueDao.Insert(kv) > 0;
            }
            int state = flag ? 0 : -100;
            string msg = flag ? "操作成功" : "操作失败";
            string url = "/channelpay/PayTypeList";
            MemcachHelper.Delete(Constant.list_keyvalue_key + kv.TypeId);
            return new RedirectResult($"/Prompt?state={state}&msg={msg}&url={url}");

        }

    }
}