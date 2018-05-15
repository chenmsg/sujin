using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITOrm.Payment.Yeepay;
using ITOrm.Utility.Helper;
using ITOrm.Utility.StringHelper;
using ITOrm.Utility.Const;
using ITOrm.Manage.Filters;

namespace ITOrm.Manage.Controllers
{
    public class YeepayController : Controller
    {


        public ActionResult Main()
        {
            return View();
        }

        // GET: Yeepa
        [AdminFilter]
        public ActionResult Index()
        {
            return View();
        }


        

        public ActionResult QueryFeeSetApi()
        {
            int UserId = TQuery.GetInt("UserId");
            int ProductType = TQuery.GetInt("ProductType");
            respQueryFeeSetApiModel result = new respQueryFeeSetApiModel();
            if (UserId == 0 || ProductType == 0)
            {
                result.backState = -100;
                result.message = "";
            }
            else
            {
                result = YeepayDepository.QueryFeeSetApi(UserId, 1, ProductType);
            }
             
            return View(result);
        }


        public ActionResult CustomerInforQuery()
        {
            int UserId = TQuery.GetInt("UserId");
            string Mobile = TQuery.GetSafeString("Mobile");
            respCustomerInforQueryModel result = new respCustomerInforQueryModel();
            if (UserId == 0 )
            {
                if (!string.IsNullOrEmpty(Mobile))
                {
                    result = YeepayDepository.CustomerInforQuery(Mobile, (int)Logic.Platform.系统);
                }
                else
                {
                    result.backState = -100;
                    result.message = "";
                }
                
            }
            else
            {
                   result= YeepayDepository.CustomerInforQuery(UserId, (int)Logic.Platform.系统);
            }

            return View(result);
        }

        public ActionResult CustomerBalanceQuery()
        {
            int UserId = TQuery.GetInt("UserId");
            int BalanceType = TQuery.GetInt("BalanceType");
            respCustomerBalanceQueryModel result = new respCustomerBalanceQueryModel();
            if (UserId == 0)
            {
                result.backState = -100;
                result.message = "";
            }
            else
            {
                result = YeepayDepository.CustomerBalanceQuery(UserId, (int)Logic.Platform.系统, BalanceType);
            }
            return View(result);
        }

        public ActionResult TransferQuery()
        {
            //externalNo
            string externalNo = TQuery.GetString("externalNo");
            respTransferQueryModel result = new respTransferQueryModel();
            if (string.IsNullOrEmpty(externalNo))
            {
                result.backState = -100;
                result.message = "";
            }
            else
            {
                result = YeepayDepository.TransferQuery(externalNo, (int)Logic.Platform.系统);
            }
            return View(result);
        }

        public ActionResult TradeReviceQuery()
        {
            string requestId = TQuery.GetString("requestId");
            respTradeReviceQueryModel result = new respTradeReviceQueryModel();
            if (string.IsNullOrEmpty(requestId))
            {
                result.backState = -100;
                result.message = "";
            }
            else
            {
                result = YeepayDepository.TradeReviceQuery(requestId, (int)Logic.Platform.系统);
            }
            return View(result);
        }

        public ActionResult LendTargetFeeQuery()
        {
            int UserId = TQuery.GetInt("UserId");
            decimal Amount = TQuery.GetDecimal("Amount",0m);
            respLendTargetFeeQueryModel result = new respLendTargetFeeQueryModel();
            if (UserId==0||Amount==0m)
            {
                result.backState = -100;
                result.message = "";
            }
            else
            {
                result = YeepayDepository.LendTargetFeeQuery(UserId, (int)Logic.Platform.系统,Amount);
            }
            return View(result);
        }

        public ActionResult QueryRJTBalance()
        {
            respQueryRJTBalanceModel result = YeepayDepository.QueryRJTBalance( (int)Logic.Platform.系统);
            return View(result);
        }
    }
}