using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITOrm.Host.BLL;
using ITOrm.Host.Models;
namespace ITOrm.Manage.Filters
{
    public class AdminFilter : ActionFilterAttribute
    {
       
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (filterContext.HttpContext.Session["AdminUser"] == null)
            {
                filterContext.Result = new RedirectResult("/home/login");
            }
            else
            {
                

            }
            //filterContext.HttpContext.Session["AdminUser"] += "TestFilter OnActionExecuting<br/>";
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //filterContext.HttpContext.Session["AdminUser"] += "TestFilter OnActionExecuted<br/>";
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //filterContext.HttpContext.Session["AdminUser"] += "TestFilter OnResultExecuting<br/>";
        }



        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {

            //filterContext.HttpContext.Session["AdminUser"] += "TestFilter OnResultExecuted<br/>";

        }
    }
}