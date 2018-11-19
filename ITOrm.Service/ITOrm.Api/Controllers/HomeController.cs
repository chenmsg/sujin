using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Web.Mvc;
using ITOrm.Utility.ITOrmApi;

using ITOrm.Host.BLL;
using System.Data.SqlClient;
using ITOrm.Utility.Serializer;
using ITOrm.Host.Models;
using ITOrm.Core.Memcached.Impl;
using Memcached.ClientLibrary;
namespace ITOrm.Api.Controllers
{
    public class HomeController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }
       
    }

}