using System.Text;
using System.Web.Mvc.Html;
namespace System.Web.Mvc
{

    public static class PagerHelper
    {
        /// <summary>  
        /// 分页Pager显示  （后台）
        /// </summary>   
        /// <param name="html"></param>  
        /// <param name="currentPageStr">标识当前页码的QueryStringKey</param>   
        /// <param name="pageSize">每页显示</param>  
        /// <param name="totalCount">总数据量</param>  
        /// <returns></returns> 
        public static MvcHtmlString Pager(this HtmlHelper html, string currentPageStr, int pageSize, int totalCount)
        {
            var queryString = html.ViewContext.HttpContext.Request.QueryString;
            int currentPage = 1; //当前页  
            var totalPages = Math.Max((totalCount + pageSize - 1) / pageSize, 1); //总页数  
            var dict = new System.Web.Routing.RouteValueDictionary(html.ViewContext.RouteData.Values);
            var output = new System.Text.StringBuilder();
            if (queryString.Count > 0)
            {
                //与相应的QueryString绑定 
                foreach (string key in queryString.Keys)
                {
                    if (queryString[key] != null && !string.IsNullOrEmpty(key))
                    {
                        dict[key] = queryString[key];
                        if (dict[currentPageStr] != null)
                        {
                            int.TryParse(dict[currentPageStr].ToString(), out currentPage);
                        }
                    }
                }
            }
            else
            {
                //获取 ～/Page/{page number} 的页号参数
                if (dict[currentPageStr] != null)
                {
                    int.TryParse(dict[currentPageStr].ToString(), out currentPage);
                }
            }
            output.Append("<div class=\"pagination pagination-centered\"><ul>");
            if (currentPage <= 0) currentPage = 1;
            if (totalPages > 1)
            {

                if (currentPage != 1)
                {
                    //处理首页连接  
                    dict[currentPageStr] = 1;
                    output.AppendFormat("<li>{0}</li> ", html.RouteLink("首页", dict));
                }
                if (currentPage > 1)
                {
                    //处理上一页的连接  
                    dict[currentPageStr] = currentPage - 1;
                    output.AppendFormat("<li>{0}</li>",html.RouteLink("上一页", dict));
                }
                else
                {
                    output.AppendFormat("<li class=\"disabled\" >{0}</li>", html.RouteLink("上一页", dict));
                }
                output.Append(" ");
                int currint = 5;
                for (int i = 0; i <= 10; i++)
                {
                    //一共最多显示10个页码，前面5个，后面5个  
                    if ((currentPage + i - currint) >= 1 && (currentPage + i - currint) <= totalPages)
                        if (currint == i)
                        {
                            //当前页处理  
                            output.Append(string.Format("<li class=\"active\">{0}</li>", html.RouteLink(currentPage.ToString(), dict)));
                        }
                        else
                        {
                            //一般页处理 
                            dict[currentPageStr] = currentPage + i - currint;
                            output.AppendFormat("<li>{0}</li>", html.RouteLink((currentPage + i - currint).ToString(), dict));
                        }
                    output.Append(" ");
                }
                if (currentPage < totalPages)
                {
                    //处理下一页的链接 
                    dict[currentPageStr] = currentPage + 1;
                    output.AppendFormat("<li>{0}</li>", html.RouteLink("下一页", dict));
                }
                else
                {
                    output.AppendFormat("<li class=\"disabled\">{0}</li>", html.RouteLink("下一页", dict));
                }
                output.Append(" ");
                if (currentPage != totalPages)
                {
                    dict[currentPageStr] = totalPages;
                    output.AppendFormat("<li >{0}</li>", html.RouteLink("末页", dict));
                }
                output.Append(" ");
            }
            //if (totalPages > 1)
            //{
            //    output.AppendFormat("{0} / {1}", currentPage, totalPages);//这个统计加不加都行 
            //}
            //else
            //{
            //    output.AppendFormat("{0} / {1}", 1, 1);//这个统计加不加都行 
            //}
            //output.Append(" 数量:").Append(totalCount);
            StringBuilder param = new StringBuilder();
            foreach (string key in queryString.Keys)
            {
                param.Append(key).Append("=").Append(queryString[key]).Append("&");
            }
            string strParam = param.ToString();
            if (!string.IsNullOrEmpty(strParam))
            {
                strParam = strParam.Substring(0, strParam.Length - 1);
            }
            output.Append("</ul>");
            output.Append("<input type=\"hidden\" id=\"hidParam\" value=\"" + strParam + "\" />");
            output.AppendFormat("<h6 >pages:{0}/{1} totalCount:{2} pageSize:{3}</h6>", totalPages > 1?currentPage:1, totalPages > 1?totalPages:1,totalCount,pageSize);
            output.Append("</div>");
            return MvcHtmlString.Create(output.ToString());
        }

        /// <summary>
        /// 分页（前台）
        /// </summary>
        /// <param name="html"></param>
        /// <param name="currentPageStr">分页标识</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="totalCount">总个数</param>
        /// <param name="IsLast">是否显示上一页</param>
        /// <param name="IsNext">是否显示下一页</param>
        /// <param name="IsIndex">是否显示第一页（首页）</param>
        /// <param name="IsEnd">是否显示末页</param>
        /// <param name="IsShowPage">是否显示  当前页/总页数</param>
        /// <returns></returns>
        public static MvcHtmlString Pager(this HtmlHelper html, string currentPageStr, int pageSize, int totalCount, bool IsLast, bool IsNext, bool IsIndex, bool IsEnd, bool IsShowPage)
        {
            var queryString = html.ViewContext.HttpContext.Request.QueryString;
            int currentPage = 1; //当前页  
            var totalPages = Math.Max((totalCount + pageSize - 1) / pageSize, 1); //总页数  
            var dict = new System.Web.Routing.RouteValueDictionary(html.ViewContext.RouteData.Values);
            var output = new System.Text.StringBuilder();
            if (queryString.Count > 0)
            {
                //与相应的QueryString绑定 
                foreach (string key in queryString.Keys)
                {
                    if (queryString[key] != null && !string.IsNullOrEmpty(key))
                    {
                        dict[key] = queryString[key];
                        if (dict[currentPageStr] != null)
                        {
                            int.TryParse(dict[currentPageStr].ToString(), out currentPage);
                        }
                    }
                }
            }
            else
            {
                //获取 ～/Page/{page number} 的页号参数
                if (dict[currentPageStr] != null)
                {
                    int.TryParse(dict[currentPageStr].ToString(), out currentPage);
                }
            }
            output.Append("<div class=\"fenye\">");
            if (currentPage <= 0) currentPage = 1;
            if (totalPages > 1)
            {

                if (currentPage != 1 && IsIndex)
                {
                    //处理首页连接  
                    dict[currentPageStr] = 1;
                    output.AppendFormat("{0} ", html.RouteLink("首页", dict));
                }
                if (IsLast)
                {
                    if (currentPage > 1)
                    {
                        //处理上一页的连接  
                        dict[currentPageStr] = currentPage - 1;
                        output.Append(html.RouteLink("上一页", dict));
                    }
                    else
                    {
                        output.Append("<span class=\"disabled\">上一页</span>");
                    }
                }
                int currint = 5;
                for (int i = 0; i <= 10; i++)
                {
                    //一共最多显示10个页码，前面5个，后面5个  
                    if ((currentPage + i - currint) >= 1 && (currentPage + i - currint) <= totalPages)
                        if (currint == i)
                        {
                            //当前页处理  
                            output.Append(string.Format("{0}", "<span class=\"current\">" + currentPage + "</span>"));
                        }
                        else
                        {
                            //一般页处理 
                            dict[currentPageStr] = currentPage + i - currint;
                            output.Append(html.RouteLink((currentPage + i - currint).ToString(), dict));
                        }
                }
                if (IsNext)
                {
                    if (currentPage < totalPages)
                    {
                        //处理下一页的链接 
                        dict[currentPageStr] = currentPage + 1;
                        output.Append(html.RouteLink("下一页", dict));
                    }
                    else
                    {
                        output.Append("<span class=\"disabled\">下一页</span>");
                    }
                }

                output.Append(" ");
                if (IsEnd)
                {
                    if (currentPage != totalPages)
                    {
                        dict[currentPageStr] = totalPages;
                        output.Append(html.RouteLink("末页", dict));
                    }
                }
                output.Append(" ");
            }
            if (IsShowPage)
            {
                if (totalPages > 1)
                {
                    output.AppendFormat("{0} / {1}", currentPage, totalPages);//这个统计加不加都行 
                }
                else
                {
                    output.AppendFormat("{0} / {1}", 1, 1);//这个统计加不加都行 
                }
            }
            output.Append("</div>");
            return MvcHtmlString.Create(output.ToString());
        }

        /// <summary>
        /// Ajax（前台）
        /// </summary>
        /// <param name="html"></param>
        /// <param name="currentPageStr">分页标识</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="totalCount">总个数</param>
        /// <param name="IsLast">是否显示上一页</param>
        /// <param name="IsNext">是否显示下一页</param>
        /// <param name="IsIndex">是否显示第一页（首页）</param>
        /// <param name="IsEnd">是否显示末页</param>
        /// <param name="IsShowPage">是否显示  当前页/总页数</param>
        /// <returns></returns>
        public static MvcHtmlString AjaxPager(this HtmlHelper html, string currentPageStr, int pageSize, int totalCount, bool IsLast, bool IsNext, bool IsIndex, bool IsEnd, bool IsShowPage, int qid, int currentPage)
        {
            //var queryString = html.ViewContext.HttpContext.Request.QueryString;
            //int currentPage = 1; //当前页  
            var totalPages = Math.Max((totalCount + pageSize - 1) / pageSize, 1); //总页数  
            var dict = new System.Web.Routing.RouteValueDictionary(html.ViewContext.RouteData.Values);
            var output = new System.Text.StringBuilder();
            //if (queryString.Count > 0)
            //{
            //    //与相应的QueryString绑定 
            //    foreach (string key in queryString.Keys)
            //    {
            //        if (queryString[key] != null && !string.IsNullOrEmpty(key))
            //        {
            //            dict[key] = queryString[key];
            //            if (dict[currentPageStr] != null)
            //            {
            //                int.TryParse(dict[currentPageStr].ToString(), out currentPage);
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    //获取 ～/Page/{page number} 的页号参数
            //    if (dict[currentPageStr] != null)
            //    {
            //        int.TryParse(dict[currentPageStr].ToString(), out currentPage);
            //    }
            //}
            output.Append("<div class=\"fenye\">");
            if (currentPage <= 0) currentPage = 1;
            if (totalPages > 1)
            {

                if (currentPage != 1 && IsIndex)
                {
                    //处理首页连接  
                    dict[currentPageStr] = 1;
                    output.Append("<a href='javascript:;' onclick=\"changePage(" + qid + ",1)\">首页</a>");
                }
                if (IsLast)
                {
                    if (currentPage > 1)
                    {
                        //处理上一页的连接  
                        dict[currentPageStr] = currentPage - 1;
                        output.Append("<a href=\"javascript:;\" onclick=\"changePage(" + qid + "," + (currentPage - 1) + ")\">上一页</a>");
                    }
                    else
                    {
                        output.Append("<span class=\"disabled\">上一页</span>");
                    }
                }
                int currint = 5;
                for (int i = 0; i <= 10; i++)
                {
                    //一共最多显示10个页码，前面5个，后面5个  
                    if ((currentPage + i - currint) >= 1 && (currentPage + i - currint) <= totalPages)
                        if (currint == i)
                        {
                            //当前页处理  
                            output.Append(string.Format("{0}", "<span class=\"current\">" + currentPage + "</span>"));
                        }
                        else
                        {
                            //一般页处理 
                            dict[currentPageStr] = currentPage + i - currint;
                            output.Append("<a href=\"javascript:;\" onclick=\"changePage(" + qid + "," + (currentPage + i - currint) + ")\">" + (currentPage + i - currint) + "</a>");
                        }
                }
                if (IsNext)
                {
                    if (currentPage < totalPages)
                    {
                        //处理下一页的链接 
                        dict[currentPageStr] = currentPage + 1;
                        output.Append("<a href=\"javascript:;\" onclick=\"changePage(" + qid + "," + (currentPage + 1) + ")\">下一页</a>");
                    }
                    else
                    {
                        output.Append("<span class=\"disabled\">下一页</span>");
                    }
                }

                output.Append(" ");
                if (IsEnd)
                {
                    if (currentPage != totalPages)
                    {
                        dict[currentPageStr] = totalPages;
                        output.Append("<a href=\"javascript:;\" onclick=\"changePage(" + qid + "," + totalPages + ")\">末页</a>");
                    }
                }
                output.Append(" ");
            }
            if (IsShowPage)
            {
                if (totalPages > 1)
                {
                    output.AppendFormat("{0} / {1}", currentPage, totalPages);//这个统计加不加都行 
                }
                else
                {
                    output.AppendFormat("{0} / {1}", 1, 1);//这个统计加不加都行 
                }
            }
            output.Append("</div>");
            return MvcHtmlString.Create(output.ToString());
        }

    }


}