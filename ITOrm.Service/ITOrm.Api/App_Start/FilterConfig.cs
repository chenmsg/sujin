using ITOrm.Api.Filters;
using System.Web;
using System.Web.Mvc;

namespace ITOrm.Api
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //全局错误信息捕获
            filters.Add(new HandleErrorFilter());
            //IP验证
            filters.Add(new IPFilter());
            //签名验证
            if (!ITOrm.Utility.Const.Constant.IsSign)
            {
                filters.Add(new ValidateAtrributeFilter());
            }
            
            //异步同步服务器验证
            filters.Add(new Aysn());
        }
    }
}
