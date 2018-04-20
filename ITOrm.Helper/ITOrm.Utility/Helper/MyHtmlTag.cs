using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc.Html;
using System.Web.Mvc;

namespace System.Web.Mvc
{

    public static class MyDropDownList
    {
        
        /// <summary>
        /// 下拉列表框
        /// </summary>
        /// <param name="html"></param>
        /// <param name="list">SelectListItem集合</param>
        /// <param name="firstText">第一列的Text</param>
        /// <param name="firstValue">第一列的Value</param>
        /// <param name="SelectVal">选中的值</param>
        /// <param name="htmlAttr">标签属性</param>
        /// <returns></returns>
        public static MvcHtmlString MyDrop(this HtmlHelper html, IEnumerable<SelectListItem> list, string firstText, string firstValue, string SelectVal, object htmlAttr)
        {
            IDictionary<string, object> attr = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttr);
            StringBuilder output = new StringBuilder();
            TagBuilder tagBuilder = new TagBuilder("select");
            tagBuilder.MergeAttributes<string, object>(attr);
            if (!string.IsNullOrEmpty(firstText))
            {
                if (SelectVal == firstValue)
                {
                    output.Append("<option selected=\"selected\" value='").Append(firstValue).Append("'>").Append(firstText).Append("</option>");
                }
                else
                {
                    output.Append("<option value='").Append(firstValue).Append("'>").Append(firstText).Append("</option>");
                }
                
            }
            foreach (var item in list)
            {
                if (item.Value == SelectVal)
                {
                    output.Append("<option selected=\"selected\" value='").Append(item.Value).Append("'>").Append(item.Text).Append("</option>");
                }
                else
                {
                    output.Append("<option  value='").Append(item.Value).Append("'>").Append(item.Text).Append("</option>");
                }
            }
            tagBuilder.InnerHtml = output.ToString();
            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));

        }


        /// <summary>
        ///自定义 Radio
        /// </summary>
        /// <param name="html"></param>
        /// <param name="list">SelectListItem集合</param>
        /// <param name="firstText">第一列的Text</param>
        /// <param name="firstValue">第一列的Value</param>
        /// <param name="SelectVal">选中的值</param>
        /// <param name="htmlAttr">标签属性</param>
        /// <returns></returns>
        public static MvcHtmlString MyRadio(this HtmlHelper html, IEnumerable<SelectListItem> list, string SelectVal, string Name)
        {

            StringBuilder output = new StringBuilder();
            output.Append("<div class=\"radio\">");
            foreach (var item in list)
            {
                output.Append("<label class=\"radio-inline\">");
                if (SelectVal == item.Value)
                {
                    output.Append($"<input type=\"radio\" name=\"{Name}\" id=\"optionsRadios1\" value=\"{item.Value}\" checked=\"checked\" />{item.Text}");
                }
                else
                {
                    output.Append($"<input type=\"radio\" name=\"{Name}\" id=\"optionsRadios1\" value=\"{item.Value}\"  />{item.Text}");
                }

                output.Append("</label>");
            }
            output.Append("</div>");
            return new MvcHtmlString(output.ToString());

        }

        /// <summary>
        /// 自定义span标签 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static MvcHtmlString MySpan(this HtmlHelper html, string text, string color)
        {

            string tag = $"<span style='color:{color}'>{text}</span>";
            return new MvcHtmlString(tag);

        }

        public static MvcHtmlString MyTips(this HtmlHelper html,string text,string title,string content)
        {
            string tag = $"<a tabindex = \"0\" class=\"\" role=\"button\" data-placement=\"bottom\" data-toggle=\"popover\" data-trigger=\"focus\" title=\"{title}\" data-content=\"{content}\">{text}</a>";
            return new MvcHtmlString(tag);
        }
        public static MvcHtmlString MyTips(this HtmlHelper html, string text, string title, string[] contents)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in contents)
            {
                sb.Append(item).Append("<br/>");
            }
            string tag = $"<a tabindex = \"0\" class=\"\" role=\"button\" data-placement=\"bottom\" data-toggle=\"popover\" data-trigger=\"focus\" title=\"{title}\" data-content=\"{sb.ToString()}\">{text}</a>";
            return new MvcHtmlString(tag);
        }
    }
        
    
}