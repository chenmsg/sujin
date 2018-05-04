using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using ITOrm.Utility.Serializer;
using ITOrm.Utility.Encryption;
using ITOrm.Host.BLL;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ITOrm.Utility.ITOrmApi;
using ITOrm.Host.Models;
using ITOrm.Utility.Cache;
using ITOrm.Utility.Const;
using ITOrm.Utility.Log;

namespace ITOrm.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateAtrributeFilter : ActionFilterAttribute
    {
        public static ChannelBLL channelDao = new ChannelBLL();
        public static JsonSerializerSettings jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        public  static List<Channel> listChannel = MemcachHelper.Get<List<Channel>>(Constant.list_api_channel_key , DateTime.Now.AddDays(7), () =>
        {
            return channelDao.GetQuery(" 1=1 ");
        });

        //
        // 摘要:
        //     在执行操作方法之前由 ASP.NET MVC 框架调用。
        //
        // 参数:
        //   ctx:
        //     筛选器上下文。
        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            IDictionary<string, object> param = ctx.ActionParameters;
            string jsoncallback = ctx.HttpContext.Request["jsoncallback"];
            string action = ctx.ActionDescriptor.ControllerDescriptor.ControllerName+"/"+ctx.ActionDescriptor.ActionName;
            string userName = (ctx.HttpContext.Request["itormName"] as string);
            string version = (ctx.HttpContext.Request["version"] as string);//版本号
            string encode = (ctx.HttpContext.Request["encode"] as string);
            string sign = (ctx.HttpContext.Request["sign"] as string);

            //if (action.ToLower() == "debug/tool" || action.ToLower() == "pay/cashier")
            //{
            //    return;
            //}
            //if (userName == "itormios")
            //{
            //    Logs.WriteLog($"action:{action}", "d:\\Log\\", "OnActionExecuting");
            //}
            
            string arrStr = System.Web.Configuration.WebConfigurationManager.AppSettings["AccessLogin"] != null ? System.Web.Configuration.WebConfigurationManager.AppSettings["AccessLogin"].ToString() : "";
            var arr = arrStr.Split(',');
            if (arr.Any(s => s.ToLower().Equals(action.ToLower())))
            {
                return ;//不需要验证
            }
            if (action.ToLower().Contains("debug"))
            {
                return ;
            }

            string[] reqAllKeys = HttpContext.Current.Request.Params.AllKeys;//获取所有传过来的key
            //param.Add("version", version);//强制版本号参与签名
            StringBuilder strParam = new StringBuilder("");
            foreach (var key in param.Keys)
            {
                //映射接口参数必须同request参数匹配
                if (key != "userName" && key != "sign" &&key!= "version" && key!= "base64" && reqAllKeys.Any(m => m == key))
                {
                    if (encode == "1")
                    {
                        strParam.AppendFormat("{0}={1}&", key, System.Web.HttpUtility.UrlEncode(param[key].ToString()));
                    }
                    else
                    {
                        strParam.AppendFormat("{0}={1}&", key, param[key]);
                    }
                }
            }
            if (encode == "1")
            {
                strParam.AppendFormat("{0}={1}&", "encode", "1"); 
            }
            string msg = "";
            int cid = 0;
            if (false == Login(userName, sign, action, strParam.ToString().TrimEnd('&'),version, out msg,out cid))
            {
                #region 签名错误
                string[] paramtersKey = System.Web.HttpContext.Current.Request.Form.AllKeys;
                var sortedParamtersKey = from s in paramtersKey
                                         orderby s ascending
                                         select s;
                StringBuilder str = new StringBuilder();
                str.Append("{");
                foreach (string key in sortedParamtersKey)
                {
                    str.AppendFormat("\"{0}\":\"{1}\",", key, System.Web.HttpContext.Current.Request.Form[key].Trim());
                }
                if (str.Length > 0)
                {
                    str.Remove(str.Length - 1, 1);//移除最后一个逗号
                }
                str.Append("}");
                //返回后日志记录
                Logs.WriteLog($"签名错误参数：userName:{userName},action:{action},sign:{sign},strParam:{strParam.ToString().TrimEnd('&')},version:{version},全部参数{str.ToString()}", "d:\\Log\\System", "签名错误");
                #endregion


                //var jsoncList = new jsonCommModelList<object>
                //{
                //    backStatus = 2,
                //    msg = msg,
                //};
                var result = ApiReturnStr.getError(-2, msg);
                ctx.HttpContext.Response.Clear();
                ctx.HttpContext.Response.Write(result);
                ctx.HttpContext.Response.End();
                ctx.Result = new EmptyResult();
            }
            else
            {
                ctx.ActionParameters["cid"] = cid;
                ctx.ActionParameters["version"] = version;
                ctx.HttpContext.Response.Clear();
                ctx.HttpContext.Response.ContentType = "text/plain";
                ctx.HttpContext.Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
            }
        }

        private bool Login(string userName, string sign, string targer, string strParam,string version, out string msg,out int cid)
        {
            cid = 0;
            msg = "fail，登录验证异常,签名错误";
           
            bool flag = false;
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(sign) && !string.IsNullOrEmpty(targer))
            {
                var passWord = "";
                var md5key = "";
                var buildParam = strParam;
                var hasUserName = GetApiBusinessByUserName(userName, out passWord, out md5key,out cid);

                //获取ApiBusiness失败直接返回false
                if (false == hasUserName)
                {
                    msg = "登录失败，用户不存在！";
                    return flag;
                }
                    

                var arrayParam = strParam.ToArray();
                Array.Sort(arrayParam);//对字符串进行排序
                buildParam = new string(arrayParam);

                string key = string.Format("{0}{1}{2}{3}{4}", userName, passWord, targer, md5key, buildParam);
                if (sign == SecurityHelper.GetMD5String(key))
                {
                    msg = "Sucess,签名验证通过。";
                    flag = true;
                }
                if (string.IsNullOrEmpty(version))
                {
                    msg = "未传版本号";
                    flag = false;
                }
            }
            return flag;
        }

        /// <summary>
        /// 根据用户名获取用户ApiBusiness身份认证
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">约定密码</param>
        /// <param name="md5key">约定key</param>
        /// <returns></returns>
        private bool GetApiBusinessByUserName(string userName, out string passWord, out string md5key,out int cid)
        {
            var retVal = false;
            passWord = md5key = "";
            cid = 0;
            if (string.IsNullOrEmpty(userName))
                return retVal;

            //if (userName == "ITOrm" && passWord == "ITOrm" && md5key == "ITOrm")
            //    return true;

            Channel api = listChannel.Find(m => m.CName.Equals(userName));

            if (api != null)
            {

                passWord = api.CPwd;
                md5key = api.CMd5Key;
                cid = api.CId;
                retVal = true;
            }
            return retVal;
        }

    }
}