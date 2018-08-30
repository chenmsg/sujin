using ITOrm.Host.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ITOrm.Host.Models;
using ITOrm.Utility.Helper;
using ITOrm.Utility.Const;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
namespace ITOrm.Manage.Controllers
{
    public class TimedTaskController : Controller
    {
        public static TimedTaskBLL timedTaskDao = new TimedTaskBLL();
        public KeyValueBLL keyValueDao = new KeyValueBLL();
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


        public ActionResult openChannel(int chan)
        {
            var result = new ResultModel();
            result.backState = -100;
            result.message = "未知错误";
            int resultId = 0;
            int TypeId = (int)Logic.KeyValueType.支付通道管理;
            Logic.ChannelType ct = (Logic.ChannelType)chan;
            var kv = keyValueDao.Single(" TypeId=@TypeId and keyId=@chan ", new { TypeId, chan });
            if (kv != null && kv.ID > 0)
            {


                //添加定时任务记录
                var stime = JObject.Parse(kv.Value)["StartTime"];
                DateTime execTime = Convert.ToDateTime(DateTime.Now.ToString($"yyyy-MM-dd {stime}")).AddDays(1);

                JObject value = new JObject();
                value["keyId"] = kv.ID;
                value["remark"] = $"{ct}通道定时开启";
                //不能五分钟内连续创建任务
                if (timedTaskDao.Count(" DATEDIFF(MINUTE,CTime,GETDATE())<5 ") > 0)
                {
                    return new RedirectResult($"/Prompt?state=-100&msg=不能五分钟内连续创建任务&url=/TimedTask/");
                }
                resultId = timedTaskDao.Init(Logic.TimedTaskType.通道开启, execTime, value.ToString());
                result.backState = resultId > 0 ? 0 : -100;
                result.message = resultId > 0 ? "任务创建成功" : "任务创建失败";
            }

            return new RedirectResult($"/Prompt?state={result.backState}&msg={result.message}&url=/TimedTask/");
        }


        public ActionResult DeleteTask(int ID)
        {
            var task= timedTaskDao.Single(ID);
            task.State = -1;
            task.UTime = DateTime.Now;
            bool flag = timedTaskDao.Update(task);
            int backState = flag ? 0 : -100;
            string message= flag ? "任务删除成功" : "任务删除失败";
            return new RedirectResult($"/Prompt?state={backState}&msg={message}&url=/TimedTask/");
        }
    }
}