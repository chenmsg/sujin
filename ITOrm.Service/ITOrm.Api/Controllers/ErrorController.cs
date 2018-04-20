using ITOrm.Utility.ITOrmApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITOrm.Api.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public string Index()
        {
            return ApiReturnStr.getError(1, "404找不到该地址");
        }
    }
}