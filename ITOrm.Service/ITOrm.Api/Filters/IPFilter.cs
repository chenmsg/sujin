using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using ITOrm.Utility.Serializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ITOrm.Utility.ITOrmApi;

namespace ITOrm.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class IPFilter : ActionFilterAttribute
    {
        public static JsonSerializerSettings jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        public static string WhitelistIP = ConfigurationManager.AppSettings["WhitelistIP"];// 白名单Ip
        public static string BlacklistIP = ConfigurationManager.AppSettings["BlacklistIP"];// 黑名单Ip
        public static string VisitMode = ConfigurationManager.AppSettings["VisitMode"];// 黑名单Ip
        //
        // 摘要:
        //     在执行操作方法之前由 ASP.NET MVC 框架调用。
        //
        // 参数:
        //   ctx:
        //     筛选器上下文。
        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            IDictionary<string, object> param = ctx.ActionParameters;
            string jsoncallback = ctx.HttpContext.Request["jsoncallback"];
            // 验证黑白名单
            if (!limit())
            {
                string msg =  ApiReturnStr.getError(-3, "权限不足，请联系管理员");
                //var jsoncList = new jsonCommModelList<object>
                //{
                //    backStatus = 3,
                //    msg = "权限不足，请联系管理员",
                //};
                
                ctx.HttpContext.Response.Clear();
                //ctx.HttpContext.Response.Write(jsoncallback + "" + JsonConvert.SerializeObject(jsoncList, timeFormat) + "");
                ctx.HttpContext.Response.Write(msg);
                ctx.HttpContext.Response.End();
                ctx.Result = new EmptyResult();
            }
        }




        /// <summary>
        /// 验证黑白名单
        /// </summary>
        /// <returns></returns>
        private bool limit()
        {
            bool flag = true;
            
            string Ip = ITOrm.Utility.Client.Ip.GetClientIp();
            if (VisitMode == "Black")
            {
                if (BlacklistIP.Contains(Ip))//在黑名单内
                {
                    flag = false;
                }
            }
            else
            {
                if (!WhitelistIP.Contains(Ip))//不在白名单内
                {
                    flag = false;
                }
            }
            return flag;
        }

    }
}