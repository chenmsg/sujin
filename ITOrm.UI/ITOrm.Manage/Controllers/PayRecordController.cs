using ITOrm.Host.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITOrm.Utility.Helper;
using ITOrm.Host.Models;
using Newtonsoft.Json.Linq;
using ITOrm.Payment.Yeepay;
using Newtonsoft.Json;
using ITOrm.Manage.Filters;
using System.Text;
using ITOrm.Utility.Const;

namespace ITOrm.Manage.Controllers
{
    public class PayRecordController : Controller
    {
        public static PayRecordBLL payRecordDao = new PayRecordBLL();
        public static ViewPayRecordBLL viewPayRecordDao = new ViewPayRecordBLL();
        [AdminFilter]
        public ActionResult Index(int pageIndex = 1, int Type = -1, string KeyValue = "", int State = -200, int Platform = -1, int ChannelType = -1, string StartTime = "", string EndTime = "")
        {

            #region where 条件
            StringBuilder where = new StringBuilder();
            where.Append("1=1");
            switch (Type)
            {
                case 0://用户ID
                    where.AppendFormat(" and UserId={0}", KeyValue);
                    break;
                case 1://手机号
                    where.AppendFormat(" and Mobile='{0}' ", KeyValue);
                    break;
                case 2://姓名
                    where.AppendFormat(" and  UserId in( SELECT UserId FROM dbo.Users WHERE RealName like '%{0}%')", KeyValue);
                    break;
                case 3://身份证
                    where.AppendFormat(" and  UserId in( SELECT UserId FROM dbo.Users WHERE IdCard ='{0}') ", KeyValue);
                    break;
                default:
                    break;
            }
            if (State != -200)
            {
                where.AppendFormat(" and State={0}", State);
            }
            if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
            {
                where.AppendFormat(" and CTime BETWEEN '{0}' AND '{1}'", StartTime, EndTime);
            }
            if (Platform != -1)
            {
                where.AppendFormat(" and Platform={0}", Platform);
            }
            if (ChannelType != -1)
            {
                where.AppendFormat(" and ChannelType={0}", ChannelType);
            }
            #endregion

            decimal TotalAmount = ITOrm.Utility.Helper.DapperHelper.ExecScalarSql<decimal>($" select isnull(sum(Amount),0) from View_PayRecord where {where} ");
            decimal TotalIncome = ITOrm.Utility.Helper.DapperHelper.ExecScalarSql<decimal>($" select isnull(sum(Income),0) from View_PayRecord where {where} ");
            decimal TotalDrawIncome = ITOrm.Utility.Helper.DapperHelper.ExecScalarSql<decimal>($" select isnull(sum(DrawIncome),0) from View_PayRecord where {where} ");

            ViewBag.TotalAmount = TotalAmount;
            ViewBag.TotalIncome = TotalIncome;
            ViewBag.TotalDrawIncome = TotalDrawIncome;
            int totalCount = 0;
            var listPay = viewPayRecordDao.GetPaged(10, pageIndex, out totalCount, where.ToString());
            JArray list = new JArray();
            if (listPay != null)
            {
                list = JArray.FromObject(listPay);
            }

            return View(new ResultModel(list, totalCount));
        }


        public ActionResult Statistics()
        {
            return View();
        }

        public string QueryPayRecordBydate(string time = "")
        {

            var date = string.IsNullOrEmpty(time) ? DateTime.Now.AddMonths(-1) : Convert.ToDateTime(time);
            var list = ITOrm.Utility.Helper.DapperHelper.ExecuteProcedure<QueryPayRecordBydateModel>("proc_QueryPayRecordBydate", new { date });
            var listGroup = list.GroupBy(m => m.ChannelType).ToList();
            JObject data = new JObject();
            JObject xAxis = new JObject();

            data["Title"] = date.ToString("yyyyMM") + "月收款交易统计";
            data["xAxis"] = xAxis;

            //x坐标轴
            List<int> listDays = new List<int>();
            DateTime startTime = Convert.ToDateTime(date.ToString("yyyy-MM-01"));
            DateTime endTime = Convert.ToDateTime(startTime.AddMonths(1));
            while (startTime < endTime)
            {
                listDays.Add(startTime.Day);
                startTime = startTime.AddDays(1);
            }
            xAxis["categories"] = JArray.FromObject(listDays);


            //数据
            JArray listSeries = new JArray();
            data["series"] = listSeries;


            //加载全部数据
            JObject objCtAll = new JObject();
            objCtAll["name"] = "全部";
            JArray listDataAll = new JArray();
            objCtAll["data"] = listDataAll;
            foreach (var item in listDays)
            {

                var day = item > 9 ? item.ToString() : "0" + item.ToString();
                string today = date.ToString("yyyy-MM-" + day);
                var Amount = list.FindAll(m => m.date == today).Sum(m => m.Amount);
                listDataAll.Add(Amount);
            }
            listSeries.Add(objCtAll);


            foreach (var ct in listGroup)
            {
                JObject objCt = new JObject();
                objCt["name"] = ((Logic.ChannelType)ct.Key).ToString();
                JArray listData = new JArray();
                objCt["data"] = listData;
                foreach (var item in listDays)
                {

                    var day = item > 9 ? item.ToString() : "0" + item.ToString();
                    string today = date.ToString("yyyy-MM-" + day);
                    var dateAmount = list.Find(m => m.ChannelType == ct.Key && m.date == today);
                    listData.Add(dateAmount == null ? 0M : dateAmount.Amount);
                }
                listSeries.Add(objCt);
            }

            return data.ToString();
        }

        public class QueryPayRecordBydateModel
        {
            public string date { get; set; }
            public decimal Amount { get; set; }
            public int ChannelType { get; set; }
        }


        public ActionResult RankingList(int Top = 10, string StartTime = "", string EndTime = "")
        {
            StartTime = string.IsNullOrEmpty(StartTime) ?"2018-01-01" : StartTime;
            EndTime = string.IsNullOrEmpty(EndTime) ? DateTime.Now.AddMonths(1).ToString("yyyy-MM-01") : EndTime;
            var list = ITOrm.Utility.Helper.DapperHelper.ExecuteProcedure<dynamic>("proc_QueryTopPaySum", new { Top,StartTime,EndTime });
            return View(list);
        }
    }
}