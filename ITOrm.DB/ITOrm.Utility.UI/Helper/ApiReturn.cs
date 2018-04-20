using ITOrm.Core.Utility.Json;
using ITOrm.Core.Utility.PagerWebdiyer;
using System.Collections.Generic;

namespace ITOrm.Core.Utility.Helper
{
    public static class ApiReturn
    {
        public static string DataList<T>(List<T> t)
        {
            string returnstr = "";
            JsonCommModel<List<T>> jsoncList = new JsonCommModel<List<T>>();
            if (t != null && t.Count > 0)
            {
                jsoncList.BackStatus = 0;
                jsoncList.Msg = "success";
                jsoncList.Data = t;
            }
            else
            {
                jsoncList.BackStatus = 1;
                jsoncList.Msg = "fail,数据异常";
            }
            returnstr = JsonHelper.ObjectToJSON(jsoncList);
            return returnstr;
        }

        public static string Data<T>(T t)
        {
            string returnstr = "";
            JsonCommModel<T> jsoncList = new JsonCommModel<T>();
            if (t != null)
            {
                jsoncList.BackStatus = 0;
                jsoncList.Msg = "success";
                jsoncList.Data = t;
            }
            else
            {
                jsoncList.BackStatus = 1;
                jsoncList.Msg = "fail,数据异常";
            }
            returnstr = JsonHelper.ObjectToJSON(jsoncList);
            return returnstr;
        }


        public static string DataPageList<T>(PagedList<T> list, int totalCount, int pageIndex)
        {
            string returnstr = "";
            JsonCommModel<PagedList<T>> jsoncList = new JsonCommModel<PagedList<T>>();
            if (list != null && list.Count > 0)
            {
                jsoncList.BackStatus = 0;
                jsoncList.Msg = "success";
                jsoncList.Data = list;
                jsoncList.TotalCount = totalCount;
                jsoncList.PageIndex = pageIndex;
            }
            else
            {
                jsoncList.BackStatus = 1;
                jsoncList.Msg = "fail,数据异常";
            }
            returnstr = JsonHelper.ObjectToJSON(jsoncList);
            return returnstr;
        }
    }
}
