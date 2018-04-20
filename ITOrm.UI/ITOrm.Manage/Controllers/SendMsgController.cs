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
using ITOrm.Utility.Const;
using System.Text;

namespace ITOrm.Manage.Controllers
{
    public class SendMsgController : Controller
    {
        public static SendMsgBLL sendMsgDao = new SendMsgBLL();
        public static UsersBLL userDao = new UsersBLL();
        // GET: SendMsg
        public ActionResult Index(int pageIndex = 1, int Type = -1,int TypeId=0, string KeyValue = "", string StartTime = "", string EndTime = "")
        {
            KeyValue = KeyValue.Trim();
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
            if (TypeId != 0)
            {
                where.AppendFormat(" and TypeId={0}",TypeId);
            }
            if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
            {
                where.AppendFormat(" and CTime BETWEEN '{0}' AND '{1}'", StartTime, EndTime);
            }
    
            #endregion

            int totalCount = 0;
            var listUsers = sendMsgDao.GetPaged(10, pageIndex, out totalCount, where.ToString());
            JArray list = new JArray();
            if (listUsers != null)
            {
                list = JArray.FromObject(listUsers);
            }
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    int UserId = item["UserId"].ToInt();
                    item["RealName"] = "--";
                    if (UserId != 0)
                    {
                        Users user = userDao.Single(UserId);
                        item["RealName"] = user.RealName;
                    }
                }
            }
            return View(new ResultModel(list, totalCount));
        }
    }
}