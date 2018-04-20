using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using ITOrm.Core.Logging;
using ITOrm.Core.Helper;
using ITOrm.Core.Utility.Json;

namespace ITOrm.Core.Utility.Helper
{
    public static class ApiRequest
    {
        public static ILogger log = LogManager.GetCurrentClassLogger();

        //将下面的配置文件保存到web.config中
        //<!--调用api接口参数  List<ArticleInfo> list = "Home.List".DataList<ArticleInfo>(""); -->
        //<add key="api.username" value="j1e2c3tAPI"/>
        //<add key="api.password" value="RI@#Lump1289"/>
        //<add key="api.md5key" value="EfseIelwse34s#l2@fw"/>
        //<add key="api.url" value="http://192.168.1.113:8061/"/>

        /// <summary>
        /// 与API接口交互
        /// </summary>
        /// <param name="target"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string RequestApi(this string target, string param, string method = "POST")
        {
            string ret = string.Empty;
            try
            {
                target = target.Replace(".", "/").ToLower();
                string userName = ConfigHelper.GetAppSettings("api.username");
                string passWord = ConfigHelper.GetAppSettings("api.password");
                string md5key = ConfigHelper.GetAppSettings("api.md5key");
                string url = ConfigHelper.GetAppSettings("api.url") + target;

                string buildParam = param;
                char[] arrayParam = param.ToArray();
                Array.Sort(arrayParam);//对字符串进行排序
                buildParam = new string(arrayParam);

                string key = string.Format("{0}{1}{2}{3}{4}", userName, passWord, target, md5key, buildParam).ToLower();
                string sign = key.MD5_32();
                string body = string.Empty;
                StringBuilder requestStringUri = new StringBuilder();
                requestStringUri.Append(url);

                if (!string.IsNullOrEmpty(param))
                {

                    body = string.Format("cluname={0}&sign={1}&{2}", userName, sign, param);

                    if (method.ToUpper().Contains("GET"))
                    {
                        requestStringUri.AppendFormat("?{0}", body);
                    }
                }
                else
                {
                    body = string.Format("cluname={0}&sign={1}", userName, sign);
                    if (method.ToUpper().Contains("GET"))
                    {
                        requestStringUri.AppendFormat("?{0}", body);
                    }
                }

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestStringUri.ToString());
                request.Method = method;
                request.KeepAlive = false;
                if (method.ToUpper().Contains("POST"))
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    byte[] aryBuf = Encoding.GetEncoding("utf-8").GetBytes(body);
                    request.ContentLength = aryBuf.Length;
                    using (Stream writer = request.GetRequestStream())
                    {
                        writer.Write(aryBuf, 0, aryBuf.Length);
                        writer.Close();
                        writer.Dispose();
                    }
                }

                using (WebResponse response = request.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                    ret = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                }
            }
            catch (Exception e)
            {
                log.Error("读取接口错误，错误信息：", e);
            }
            return ret;
        }

        public static T RequestApi<T>(this string target, string param, string method = "POST")
        {
            T list = default(T);
            try
            {
                string resu = RequestApi(target, param, method);
                if (!string.IsNullOrEmpty(resu))
                {
                    T seclist = JsonHelper.JSONToObject<T>(resu);
                    return seclist;
                }
            }
            catch (Exception e)
            {
                log.Error("读取接口错误，错误信息：", e);
            }
            return list;
        }

        /// <summary>
        /// 获取API接口数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apistr"></param>
        /// <param name="resulturl"></param>
        /// <returns></returns>
        public static T Data<T>(this string target, string param, string method = "POST")
        {
            T list = default(T);
            try
            {
                string resu = RequestApi(target, param, method);
                if (!string.IsNullOrEmpty(resu))
                {
                    JsonCommModel<T> seclist = JsonHelper.JSONToObject<JsonCommModel<T>>(resu);
                    if (null != seclist && seclist.BackStatus == 0)
                    {
                        list = seclist.Data;
                    }
                    else
                    {
                        log.Error("读取接口错误，错误信息：" + seclist.Msg);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("读取接口错误，错误信息：", e);
            }
            return list;
        }

        public static List<T> DataList<T>(this string target, string param, string method = "POST")
        {
            List<T> list = default(List<T>);
            try
            {
                string resu = RequestApi(target, param, method);
                if (!string.IsNullOrEmpty(resu))
                {
                    JsonCommModelList<T> seclist = JsonHelper.JSONToObject<JsonCommModelList<T>>(resu);
                    if (null != seclist && seclist.BackStatus == 0)
                    {
                        list = seclist.Data;
                    }
                    else
                    {
                        log.Error("读取接口错误，错误信息：" + seclist.Msg);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("读取接口错误，错误信息：", e);
            }
            return list;
        }

        public static List<T> DataList<T>(this string target, string param, out int totalCount, out int pageIndex, string method = "POST")
        {
            List<T> list = default(List<T>);
            totalCount = 0;
            pageIndex = 0;
            try
            {
                string resu = RequestApi(target, param, method);
                if (!string.IsNullOrEmpty(resu))
                {
                    JsonCommModelList<T> seclist = JsonHelper.JSONToObject<JsonCommModelList<T>>(resu);
                    if (null != seclist && seclist.BackStatus == 0)
                    {
                        list = seclist.Data;
                        totalCount = seclist.TotalCount;
                        pageIndex = seclist.PageIndex;
                    }
                    else
                    {
                        log.Error("读取接口错误，错误信息：" + seclist.Msg);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("读取接口错误，错误信息：", e);
            }
            return list;
        }
    }
}
