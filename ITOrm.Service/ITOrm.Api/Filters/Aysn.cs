using System.Web.Mvc;
using ITOrm.Utility.Serializer;
using ITOrm.Utility.Log;
using ITOrm.Core.Helper;
using ITOrm.Utility.ITOrmApi;

namespace ITOrm.Api.Filters
{
    public class Aysn : ActionFilterAttribute
    {
        private bool Open = ConfigHelper.GetAppSettings("AysnOpen") == "true" ? true : false;//总开关
        private bool IsDebug = ConfigHelper.GetAppSettings("IsDebug") == "true" ? true : false;//是否是测试环境
        private bool AysnSetting = ConfigHelper.GetAppSettings("AysnSetting") == "true" ? true : false;//读config属性  true 为异步接口，false 为同步接口

        private bool _Setting = false;
        /// <summary>
        /// 传True 就是同步处理  传False就是异步处理      //默认为异步处理
        /// </summary>
        public bool Setting { get { return _Setting; } set { _Setting = value; } }

        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            if (Open)//总开关  打开
            {
                if (AysnSetting != Setting)
                {
                    string result = "";
                    //var jsoncList = new jsonCommModelList<object>
                    //{
                    //    backStatus = 3,
                    //    msg = "",
                    //};
                    if (Setting)
                    {
                        result = ApiReturnStr.getError(-3, "对不起，接口不接受同步请求");
                        //jsoncList.msg = "对不起，接口不接受同步请求";
                    }
                    else
                    {
                        result = ApiReturnStr.getError(-3, "对不起，接口不接受异步请求");
                        //jsoncList.msg = "对不起，接口不接受异步请求";
                    }
                    string action = ctx.ActionDescriptor.ControllerDescriptor.ControllerName + "/" + ctx.ActionDescriptor.ActionName;
                    string userName = (ctx.HttpContext.Request["userName"] as string);
                    string userid = (ctx.HttpContext.Request["userid"] as string);
                    Logs.WriteLog("userName=" + userName + "&action=" + action + "," + result + "&userid=" + userid, "d:\\Log\\Aysn", "Aysn");
                    ctx.HttpContext.Response.Clear();
                    //ctx.HttpContext.Response.Write(SerializerHelper.JsonSerializer<jsonCommModelList<object>>(jsoncList));
                    ctx.HttpContext.Response.Write(result);
                    ctx.HttpContext.Response.End();
                    ctx.Result = new EmptyResult();
                }
            }
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