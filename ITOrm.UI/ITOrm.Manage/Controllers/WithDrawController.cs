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

namespace ITOrm.Manage.Controllers
{
    public class WithDrawController : Controller
    {
        public static WithDrawBLL withDrawDao = new WithDrawBLL();
        // GET: WithDraw
        [AdminFilter]
        public ActionResult Index(int? pageIndex)
        {
            pageIndex = pageIndex ?? 1;
            int totalCount = 0;
            var listUsers = withDrawDao.GetPaged(10, pageIndex.Value, out totalCount, "1=1");
            JArray list = new JArray();
            if (listUsers != null)
            {
                list = JArray.FromObject(listUsers);
            }
            //if (list.Count > 0)
            //{
            //    foreach (var item in list)
            //    {
            //        item["BaseRealName"] = "--";
            //        var BaseUserId = item["BaseUserId"].ToInt();
            //        if (BaseUserId != 0)
            //        {
            //            Users user = userDao.Single(BaseUserId);
            //            item["BaseRealName"] = user.RealName;
            //        }
            //    }
            //}
            return View(new ResultModel(list, totalCount));
        }
    }
}