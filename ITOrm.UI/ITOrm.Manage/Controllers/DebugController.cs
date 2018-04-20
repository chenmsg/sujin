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
        public ActionResult DebugApi(string lcturl, string username, string password, string strMd5Key, string Target, string Param,string version)
        {
            var model = ApiRequest.RequestApiTest(lcturl, username, password, strMd5Key, Target, Param,version, "POST", "");
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
            var keyName= TQuery.GetString("keyName");
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
    }
}