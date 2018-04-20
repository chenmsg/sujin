using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ITOrm.Core.Utility.Files
{
    #region =============HttpConnect==================
    /// <summary>
    /// 远程读取网页地址 的摘要说明
    /// </summary>
    public class HttpConnect
    {
        public HttpConnect()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>参数</summary>
        private Dictionary<string, string> m_params = new Dictionary<string, string>();
        /// <summary>Cookie</summary>
        private CookieContainer m_container = new CookieContainer();

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="_name">名称</param>
        /// <returns>返回值</returns>
        public string this[string _name]
        {
            get
            {
                if (m_params.ContainsKey(_name.ToUpper()))
                    return m_params[_name.ToUpper()];
                return null;
            }
            set
            {
                m_params[_name.ToUpper()] = value;
            }
        }

        /// <summary>
        /// 创建请求
        /// </summary>
        /// <param name="_command">命令</param>
        /// <param name="_method">提交方式</param>
        /// <returns>Http请求</returns>
        public HttpWebRequest CreateRequest(string _url, string _method, string _other)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(_url);
            request.Method = _method;
            request.CookieContainer = m_container;
            byte[] data = ASCIIEncoding.Default.GetBytes(_other);
            if (data.Length > 0)
            {
                //request.ContentLength = data.Length;
                request.GetRequestStream().Write(data, 0, data.Length);
                request.GetRequestStream().Flush();
            }
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_url"></param>
        /// <param name="_method"></param>
        /// <param name="_params"></param>
        /// <returns></returns>
        public string GetResponseString(string _url, string _method)
        {
            return GetResponseString(_url, _method, "", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_url"></param>
        /// <param name="_method"></param>
        /// <param name="_params"></param>
        /// <returns></returns>
        public string GetResponseString(string _url, string _method, string _other)
        {
            return GetResponseString(_url, _method, _other, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_url"></param>
        /// <param name="_method"></param>
        /// <param name="_params"></param>
        /// <returns></returns>
        public string GetResponseString(string _url, string _method, string _other, string _referer)
        {
            HttpWebRequest request = CreateRequest(_url, _method, _other);
            if (_referer != null)
                request.Referer = _referer;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string result;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_url"></param>
        /// <param name="_method"></param>
        /// <param name="_filename"></param>
        public void GetResponseFile(string _url, string _method, string _filename, string _other)
        {
            HttpWebRequest request = CreateRequest(_url, _method, _other);
            request.Timeout = 5000;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            byte[] buffer = new byte[response.ContentLength];
            int size = stream.Read(buffer, 0, buffer.Length);
            while (size < buffer.Length)
            {
                size += stream.Read(buffer, size, buffer.Length - size);
            }
            using (FileStream fs = new FileStream(_filename, FileMode.OpenOrCreate))
            {
                fs.Write(buffer, 0, buffer.Length);
            }
        }

    }
    #endregion
}
