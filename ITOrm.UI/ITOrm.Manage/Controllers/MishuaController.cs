using ITOrm.Payment.MiShua;
using ITOrm.Utility.StringHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITOrm.Manage.Controllers
{
    public class MishuaController : Controller
    {
        // GET: Mishua
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CheckDzero()
        {
            string tradeNo = TQuery.GetString("tradeNo");
            respModel <respCheckDzeroModel> result = new respModel<respCheckDzeroModel>();

            if (string.IsNullOrEmpty(tradeNo))
            {
                result.backState = -100;
                result.message = "";
            }
            else
            {
                result = MiShuaDepository.CheckDzero(Convert.ToInt32(tradeNo), Utility.Const.Logic.Platform.系统);
            }

            return View(result);
        }
    }
}