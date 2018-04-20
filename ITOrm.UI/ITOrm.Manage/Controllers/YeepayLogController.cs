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
    public class YeepayLogController : Controller
    {
        public static YeepayLogParasBLL yeepayLogParasDao = new YeepayLogParasBLL();
        public static YeepayLogBLL yeepayLogDao = new YeepayLogBLL();

        public ActionResult Index(int pageIndex = 1, int Type = -1, string KeyValue = "", int ChannelType = -1, int TypeId = -1, int State = -200, string StartTime = "", string EndTime = "")
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
                    where.AppendFormat(" and  UserId in( SELECT UserId FROM dbo.Users WHERE Mobile='{0}')", KeyValue);
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
            if (TypeId != -1)
            {
                where.AppendFormat(" and TypeId={0}", TypeId);
            }
            if (State != -200)
            {
                where.AppendFormat(" and State={0}", State);
            }
            if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
            {
                where.AppendFormat(" and CTime BETWEEN '{0}' AND '{1}'", StartTime, EndTime);
            }
            if (ChannelType != -1)
            {
                where.AppendFormat(" and ChannelType={0}", ChannelType);
            }
            #endregion
            int totalCount = 0;
            var list = yeepayLogDao.GetPaged(10, pageIndex, out totalCount, where.ToString(), null, "order by id desc");
            return View(new ResultModel<YeepayLog>(list, totalCount));
        }



        public string QueryLogRecord(int requestId = 0)
        {
            var listLogParas = yeepayLogParasDao.GetQuery("LogId=@requestId", new { requestId });

            JArray list = new JArray();
            if (listLogParas != null)
            {
                foreach (var item in listLogParas)
                {
                    JObject data = new JObject();
                    switch (item.InOrOut)
                    {
                        case 0:
                            data["title"] = $"提交参数（{item.CTime}）";
                            break;
                        case 1:
                            data["title"] = $"返回参数（{item.CTime}）";
                            break;
                        default:
                            data["title"] = $"回调参数（{item.CTime}）";
                            break;
                    }
                    data["content"] = item.Msg;
                    list.Add(data);
                }
            }
            return list.ToString();
        }


        public string GetSelectBox(int ChannelType = 0)
        {
            List<SelectListItem> listTypeId = null;
            if (ChannelType == 0)
            {
                listTypeId = ITOrm.Utility.Helper.EnumHelper.GetEnumItemToListItem(typeof(ITOrm.Payment.Yeepay.Enums.YeepayType));
            }
            else if (ChannelType == 1 || ChannelType == 2)
            {
                listTypeId = ITOrm.Utility.Helper.EnumHelper.GetEnumItemToListItem(typeof(ITOrm.Payment.Masget.Enums.MasgetType));

            }
            else if (ChannelType == 3)
            {
                listTypeId = ITOrm.Utility.Helper.EnumHelper.GetEnumItemToListItem(typeof(ITOrm.Payment.Teng.Enums.TengType));
            }
            else
            {
                listTypeId = new List<SelectListItem>();
            }
            return JsonConvert.SerializeObject(listTypeId);

        }
    }
}