using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Web.UI;
using ITOrm.Utility.Encryption;
using ITOrm.Utility.Log;
using ITOrm.Utility.Serializer;
using Newtonsoft.Json;

namespace ITOrm.Utility.ITOrmApi
{
    public class ApiRequest
    {
        private static readonly string dict = "ITOrm";
        public static JsonSerializerSettings jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }; 

        #region 主要方法
        /// <summary>
        /// 获取api接口数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apistr"></param>
        /// <param name="resulturl"></param>
        /// <returns></returns>
        public static jsonCommModel<T> getApiData<T>(string target, string param, string method = "POST", string urltype = "")
        {
            jsonCommModel<T> json = default(jsonCommModel<T>);
            try
            {
                
                //var json = JsonConvert.SerializeObject(response, Formatting.Indented, jSetting);
                string resu = RequestApi(target, param, method, urltype);
                if (!string.IsNullOrEmpty(resu))
                {
                    json = JsonConvert.DeserializeObject<jsonCommModel<T>>(resu,jSetting);
                    //json = SerializerHelper.JsonDeserialize<jsonCommModel<T>>(resu);
                    if (null == json)
                    {
                        return getError<T>(1,"解析错误");
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog("读取接口错误，错误信息：" + ex.Message, "d:\\Log\\" + dict, target);
                return getError<T>(1, "网络异常：" + ex.Message);
            }
            return json;
        }

        public static jsonCommModelList<T> getApiDataList<T>(string target, string param, string method = "POST")
        {
            jsonCommModelList<T> jsonList = default(jsonCommModelList<T>);
            try
            {


                string resu = RequestApi(target, param, method);
                if (!string.IsNullOrEmpty(resu))
                {
                    jsonList = JsonConvert.DeserializeObject<jsonCommModelList<T>>(resu,jSetting);
                    //jsonList = SerializerHelper.JsonDeserialize<jsonCommModelList<T>>(resu);
                    if (null == jsonList)
                    {
                        return getErrorList<T>(1, "解析错误" );
                    }
   
                }

            }
            catch (Exception ex)
            {
                Logs.WriteLog("读取接口错误，错误信息：" + ex.Message, "d:\\Log\\" + dict, target);
                return getErrorList<T>(1, "网络异常" + ex.Message);
            }
            return jsonList;
        }
        #endregion

        /// <summary>
        /// 与api接口交互
        /// </summary>
        /// <param name="target"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string RequestApi(string target, string param, string method = "POST",string urltype="")
        {
            string ret = string.Empty;
            string userName = System.Web.Configuration.WebConfigurationManager.AppSettings["itorm.api.itormName"] != null ? System.Web.Configuration.WebConfigurationManager.AppSettings["itorm.api.itormName"].ToString() : "";
            var passWord = System.Web.Configuration.WebConfigurationManager.AppSettings["itorm.api.webpass"] != null ? System.Web.Configuration.WebConfigurationManager.AppSettings["itorm.api.webpass"].ToString() : "";
            var md5key = System.Web.Configuration.WebConfigurationManager.AppSettings["itorm.api.strMd5Key"] != null ? System.Web.Configuration.WebConfigurationManager.AppSettings["itorm.api.strMd5Key"].ToString() : "";
          
            var buildParam = param;
            var arrayParam = param.ToArray();
            Array.Sort(arrayParam);//对字符串进行排序
            buildParam = new string(arrayParam);

            string key = string.Format("{0}{1}{2}{3}{4}", userName, passWord, target, md5key, buildParam);
            string sign = SecurityHelper.GetMD5String(key);
            string body = string.Empty;
            //StringBuilder requestStringUri = new StringBuilder((System.Web.Configuration.WebConfigurationManager.AppSettings["fuka.api.root"]) + target + "/");
            StringBuilder requestStringUri = new StringBuilder();
            string arrStr = System.Web.Configuration.WebConfigurationManager.AppSettings["urltype"] != null ? System.Web.Configuration.WebConfigurationManager.AppSettings["urltype"].ToString() : "";
            var arr = arrStr.Split(',');
            if (arr.Any(s => s.Equals(target.ToLower())))
            {

                var lctapiurltongbu = System.Web.Configuration.WebConfigurationManager.AppSettings["itorm.api.urltongbu"] != null ? System.Web.Configuration.WebConfigurationManager.AppSettings["itorm.api.urltongbu"].ToString() : System.Web.Configuration.WebConfigurationManager.AppSettings["itorm.api.url"].ToString();
                Logs.WriteLog("读取url：" + lctapiurltongbu, "d:\\Log\\" + dict, target);
                requestStringUri.Append(lctapiurltongbu + target);
            }
            else
            {
                requestStringUri.Append((System.Web.Configuration.WebConfigurationManager.AppSettings["itorm.api.url"]) + target);
            }
            //if (urltype == "orderurl")//订单接口地址
            //{
            //    requestStringUri.Append((System.Web.Configuration.WebConfigurationManager.AppSettings["fuka.api.order"]) + target + "/");
            //}
            //else
            //{
            //    requestStringUri.Append((System.Web.Configuration.WebConfigurationManager.AppSettings["fuka.api.root"]) + target);
            //}
        
            if (!string.IsNullOrEmpty(param))
            {

                body = string.Format("itormName={0}&sign={1}&{2}", userName, sign, param);

                if (method.ToLower().Contains("get"))
                    requestStringUri.AppendFormat("?{0}", body);
            }
            else
            {
                body = string.Format("itormName={0}&sign={1}", userName, sign);
                if (method.ToLower().Contains("get"))
                    requestStringUri.AppendFormat("?{0}", body);
            }

            //Logs.kufaLog("开始发送请求：" + requestStringUri, "d:\\Log\\LCTlogs", target);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestStringUri.ToString());
            request.Method = method;
            request.KeepAlive = false; 
            if (method.ToLower().Contains("post"))
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
                StreamReader reader = new StreamReader(response.GetResponseStream()
                    , Encoding.GetEncoding("utf-8")
                    );
                ret = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
            }

            //Logs.kufaLog("获得请求数据：" + ret, "d:\\Log\\fuka20logs", target);
            return ret;
        }
        

    
        public static T RequestApi<T>(string target, string param, string method = "POST")
        {
            T list = default(T);
            try
            {


                string resu = RequestApi(target, param, method);
                if (!string.IsNullOrEmpty(resu))
                {
                    var seclist = JsonConvert.DeserializeObject<T>(resu);
                    return seclist;
                }

            }
            catch (Exception ex)
            {
                Logs.WriteLog("读取接口错误，错误信息：" + ex.Message, "d:\\Log\\fuka20logs", target);

            }
            return list;
        }


        //public static IList<T> getApiDataList<T>(string target, string param, string method = "POST")
        //{
        //    IList<T> list = default(List<T>);
        //    try
        //    {


        //        string resu = RequestApi(target, param, method);
        //        if (!string.IsNullOrEmpty(resu))
        //        {
        //            jsonCommModelList<T> seclist = SerializerHelper.JsonDeserialize<jsonCommModelList<T>>(resu);
        //            if (null != seclist && seclist.backStatus == 0)
        //            {
        //                list = seclist.Data;
        //            }
        //            else
        //            {

        //                Logs.kufaLog("读取接口错误，错误信息：" + seclist.msg, "d:\\Log\\fuka20logs", target);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.kufaLog("读取接口错误，错误信息：" + ex.Message, "d:\\Log\\fuka20logs", target);

        //    }
        //    return list;
        //}

        //public static IList<T> getApiDataList<T>(string target, string param, out int pageCount, out int recordCount, string method = "POST")
        //{
        //    IList<T> list = default(List<T>);
        //    pageCount = 0;
        //    recordCount = 0;
        //    try
        //    {
        //        string resu = RequestApi(target, param, method);
        //        if (!string.IsNullOrEmpty(resu))
        //        {
        //            jsonCommModelList<T> seclist = SerializerHelper.JsonDeserialize<jsonCommModelList<T>>(resu);
        //            if (null != seclist && seclist.backStatus == 0)
        //            {
        //                list = seclist.Data;
        //                pageCount = seclist.pageCount;
        //                recordCount = seclist.recordCount;
        //            }
        //            else
        //            {
        //                Logs.kufaLog("读取接口错误，错误信息：" + seclist.msg, "d:\\Log\\fuka20logs", target);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.kufaLog("读取接口错误，错误信息：" + ex.Message + "target=" + target + "||||param=" + param, "d:\\Log\\fuka20wenyqlogs", target);

        //    }
        //    return list;
        //}

        //编写自助签名调试平台所用
        public static RequestApiModel RequestApiTest(string lcturl, string userName, string passWord, string md5key, string target, string param, string version = "", string method = "POST", string urltype = "")
        {
            string ret = string.Empty;
            //string userName = System.Web.Configuration.WebConfigurationManager.AppSettings["lct.api.username"] != null ? System.Web.Configuration.WebConfigurationManager.AppSettings["lct.api.username"].ToString() : "";
            //var passWord = System.Web.Configuration.WebConfigurationManager.AppSettings["lct.api.webpass"] != null ? System.Web.Configuration.WebConfigurationManager.AppSettings["lct.api.webpass"].ToString() : "";
            //var md5key = System.Web.Configuration.WebConfigurationManager.AppSettings["lct.api.strMd5Key"] != null ? System.Web.Configuration.WebConfigurationManager.AppSettings["lct.api.strMd5Key"].ToString() : "";
            var buildParam = param;
            if (!string.IsNullOrEmpty(param))
            {
                var arrayParam = param.ToArray();
                Array.Sort(arrayParam);//对字符串进行排序
                buildParam = new string(arrayParam);
            }

            

            string key = string.Format("{0}{1}{2}{3}{4}", userName, passWord, target, md5key, buildParam);
            string sign = SecurityHelper.GetMD5String(key);
            string body = string.Empty;
            //StringBuilder requestStringUri = new StringBuilder((System.Web.Configuration.WebConfigurationManager.AppSettings["fuka.api.root"]) + target + "/");
            StringBuilder requestStringUri = new StringBuilder();

            requestStringUri.Append(lcturl + target);
            //if (urltype == "orderurl")//订单接口地址
            //{
            //    requestStringUri.Append((System.Web.Configuration.WebConfigurationManager.AppSettings["fuka.api.order"]) + target + "/");
            //}
            //else
            //{
            //    requestStringUri.Append((System.Web.Configuration.WebConfigurationManager.AppSettings["fuka.api.root"]) + target);
            //}

            if (!string.IsNullOrEmpty(param))
            {

                body = string.Format("itormName={0}&sign={1}&{2}&version={3}", userName, sign, param,version);

                if (method.ToLower().Contains("get"))
                    requestStringUri.AppendFormat("?{0}", body);
            }
            else
            {
                body = string.Format("itormName={0}&sign={1}&version={2}", userName, sign, version);
                if (method.ToLower().Contains("get"))
                    requestStringUri.AppendFormat("?{0}", body);
            }

            //Logs.kufaLog("开始发送请求：" + requestStringUri, "d:\\Log\\LCTlogs", target);

            RequestApiModel model = new RequestApiModel();
            model.buildParam = buildParam;
            model.Md5Str = key;
            model.Md5Sign = sign;
            model.Url = requestStringUri.ToString() + "?" + body;

            return model;
        }

        public static jsonCommModel<T> getError<T>(int backStatus = 0, string msg="")
        {
            jsonCommModel<T> json = new jsonCommModel<T>();
            json.backStatus = backStatus;
            json.msg = msg;
            return json;
        }
        //List
        public static jsonCommModelList<T> getErrorList<T>(int backStatus = 0, string msg = "")
        {
            jsonCommModelList<T> json = new jsonCommModelList<T>();
            json.backStatus = backStatus;
            json.msg = msg;
            return json;
        }
       
    }
    //编写自助签名调试平台对象
    public class RequestApiModel
    {
        /// <summary>
        /// 排序后的结果
        /// </summary>
        public string buildParam{get;set;}

        /// <summary>
        /// 加密前结果
        /// </summary>
        public string Md5Str{get;set;}

        /// <summary>
        /// 加密后结果
        /// </summary>
        public string Md5Sign{get;set;}

        public string Url{get;set;}
    }

}
