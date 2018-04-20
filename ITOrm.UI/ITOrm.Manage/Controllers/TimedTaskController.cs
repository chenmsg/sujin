using ITOrm.Host.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ITOrm.Host.Models;
using ITOrm.Utility.Helper;

namespace ITOrm.Manage.Controllers
{
    public class TimedTaskController : Controller
    {
        public static TimedTaskBLL timedTaskDao = new TimedTaskBLL();
        // GET: TimedTask
        public ActionResult Index(int pageIndex = 1,int TypeId = 0, string StartTime = "", string EndTime = "")
        {
            #region where 条件
            StringBuilder where = new StringBuilder();
            where.Append("1=1");
            if (TypeId != 0)
            {
                where.AppendFormat(" and TypeId={0}", TypeId);
            }
            if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
            {
                where.AppendFormat(" and ExecTime BETWEEN '{0}' AND '{1}'", StartTime, EndTime);
            }

            #endregion

            int totalCount = 0;
            var list= timedTaskDao.GetPaged(10, pageIndex, out totalCount, where.ToString());
            return View(new ResultModel<TimedTask>(list, totalCount));
        }
    }
}