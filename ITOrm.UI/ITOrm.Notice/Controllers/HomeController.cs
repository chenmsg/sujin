using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ITOrm.Host.BLL;
using ITOrm.Host.Models;

namespace ITOrm.Notice.Controllers
{
    public class HomeController : Controller
    {
        public static ITOrm.Host.BLL.BankQuotaBLL bankQuota = new Host.BLL.BankQuotaBLL();
        // GET: Home
        public string Index()
        {
            JObject data = new JObject();
            data["Name"] = "苏津科技";
            data["ServerTime"] = DateTime.Now.ToString();
            
            var model = bankQuota.Single(1);
            data["bank"] =JObject.FromObject(model);
            return data.ToString();
        }

        public string a()
        {
            WithDrawBLL wi = new WithDrawBLL();
            WithDraw model = wi.Single(100004);
            model.UTime = DateTime.Now;
            bool flag = wi.Update(model);
            return flag.ToString();
        }
    }

    
}