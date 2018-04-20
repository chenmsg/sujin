using ITOrm.Utility.StringHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITOrm.Payment.Teng;
using ITOrm.Utility.Const;

namespace ITOrm.Manage.Controllers
{
    public class TengController : Controller
    {
        // GET: Teng
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DebitWithdrawQuery()
        {
            int OrderId = TQuery.GetInt("OrderId");

            respDebitWithdrawQueryModel result = new respDebitWithdrawQueryModel();
            if (OrderId == 0)
            {

                result.backState = -100;
                result.respMsg = "";
            }
            else
            {
                result = TengDepository.DebitWithdrawQuery(OrderId, (int)Logic.Platform.系统);
            }
            return View(result);
        }
        public ActionResult PayDebitQuery()
        {
            int OrderId = TQuery.GetInt("OrderId");

            respPayDebitQueryModel result = new respPayDebitQueryModel();
            if (OrderId == 0)
            {

                result.backState = -100;
                result.respMsg = "";
            }
            else
            {
                result = TengDepository.PayDebitQuery(OrderId, (int)Logic.Platform.系统);
            }
            return View(result);
        }
    }
}