using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ITOrm.Api
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //默认添加区域itapi
            routes.MapRoute(
                name: "Default",
                url: "itapi/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );




            routes.MapRoute(
                name: "home-index",
                url: "home/index",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}
