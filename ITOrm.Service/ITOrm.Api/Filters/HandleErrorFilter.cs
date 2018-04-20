using System.Web.Mvc;
using ITOrm.Utility.Log;
using ITOrm.Utility.ITOrmApi;
//全局错误信息捕获
public class HandleErrorFilter : HandleErrorAttribute
{
    public override void OnException(ExceptionContext filterContext)
    {
        
        //记录错误日志
         Logs.WriteLog($"URL:{filterContext.HttpContext.Request.Url} 错误原因: {filterContext.Exception.Message}", "d:\\Log\\ITorm", "apiError");
        //返回错误信息
         string msg = ApiReturnStr.getError(500, filterContext.Exception.Message);
         filterContext.HttpContext.Response.Write(msg);
         filterContext.HttpContext.Response.End();

    }
}