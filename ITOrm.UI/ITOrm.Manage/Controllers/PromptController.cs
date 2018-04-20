using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITOrm.Manage.Controllers
{
    public class PromptController : Controller
    {
        // GET: Prompt
        public ActionResult Index(int state,string msg)
        {
            return View();
        }
    }
}