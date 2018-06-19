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
using ITOrm.Payment.Const;

namespace ITOrm.Manage.Controllers
{
    public class AccountRecordController : Controller
    {
        public static AccountRecordBLL accountRecordDao = new AccountRecordBLL();
        public static UsersBLL userDao = new UsersBLL();
        // GET: AccountRecord
        public ActionResult Index(int pageIndex = 1, int Type = -1, string KeyValue = "",int TypeId=-1 , int InOrOut = -100, string StartTime = "", string EndTime = "")
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
                    where.AppendFormat(" and  UserId in( SELECT UserId FROM dbo.Users WHERE Mobile='{0}')", KeyValue);
                    break;
                case 2://姓名
                    where.AppendFormat(" and  UserId in( SELECT UserId FROM dbo.Users WHERE RealName like '%{0}%')", KeyValue);
                    break;
                case 3://身份证
                    where.AppendFormat(" and  UserId in( SELECT UserId FROM dbo.Users WHERE IdCard ='{0}') ", KeyValue);
                    break;
                case 4://关联ID
                    where.AppendFormat(" and KeyId='{0}'", KeyValue);
                    break;
                default:
                    break;
            }
            if (TypeId != -1)
            {
                where.AppendFormat(" and TypeId={0}", TypeId);
            }
            if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
            {
                where.AppendFormat(" and CTime BETWEEN '{0}' AND '{1}'", StartTime, EndTime);
            }
            if (InOrOut != -100)
            {
                where.AppendFormat(" and InOrOut={0}", InOrOut);
            }
            #endregion

            int totalCount = 0;
            var listUsers = accountRecordDao.GetPaged(10, pageIndex, out totalCount, where.ToString());
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
                    Users user = userDao.Single(UserId);
                    item["RealName"] = string.IsNullOrEmpty(user.RealName) ? "--"  : user.RealName;
                }
            }
            return View(new ResultModel(list, totalCount));
        }
    }
}