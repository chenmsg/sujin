using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITOrm.Payment.Masget;
using ITOrm.Utility.Helper;
using ITOrm.Utility.StringHelper;
using ITOrm.Utility.Const;
using ITOrm.Manage.Filters;

namespace ITOrm.Manage.Controllers
{
    public class MasgetController : Controller
    {
        // GET: Masget
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SubcompanyGet()
        {
            respMasgetModel<respSubcompanyGetModel> result = new respMasgetModel<respSubcompanyGetModel>();
            result.ret = -100;
            
            int UserId = TQuery.GetInt("UserId");
            int ChannelType = TQuery.GetInt("ChannelType");
            if (UserId == 0)
            {
                return View(result);
            }

            result = MasgetDepository.SubcompanyGet(UserId, 1, (Logic.ChannelType)ChannelType);
            return View(result);
        }

        public ActionResult TreatyQuery()
        {
            respMasgetModel<respTreatyQueryModel> result = new respMasgetModel<respTreatyQueryModel>();
            result.ret = -100;

            string BankCard = TQuery.GetString("BankCard");
            int ChannelType = TQuery.GetInt("ChannelType");

            if (string.IsNullOrEmpty(BankCard))
            {
                return View(result);
            }
            result = MasgetDepository.TreatyQuery( 1, (Logic.ChannelType)ChannelType, BankCard);
            return View(result);
        }


        public ActionResult PaymentjournalGet()
        {
            respMasgetModel<respPaymentjournalGetModel> result = new respMasgetModel<respPaymentjournalGetModel>();
            result.ret = -100;

            int requestId = TQuery.GetInt("requestId");
            int ChannelType = TQuery.GetInt("ChannelType");

            if (requestId==0)
            {
                return View(result);
            }
            result = MasgetDepository.PaymentjournalGet(requestId, 1, (Logic.ChannelType)ChannelType);
            return View(result);
        }
    }
}