using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Web;

namespace ITOrm.Utility.Serializer
{
    public class SerializerHelper
    {
        /// <summary>
        /// json序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string JsonSerializer<T>(T t, int encode = 0)
        {
            var jsonback =  HttpContext.Current.Request["jsoncallback"] == null ? "" : HttpContext.Current.Request["jsoncallback"];
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            string jsonString = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                ser.WriteObject(stream, t);
                jsonString = Encoding.UTF8.GetString(stream.ToArray());
                jsonString = Regex.Replace(jsonString, @"\\/Date\((\d+)\+0800\)\\/", match =>
                {
                    DateTime dt = new DateTime(1970, 1, 1);
                    dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                    dt = dt.ToLocalTime();
                    return dt.ToString("yyyy-MM-dd HH:mm:ss");
                });
            }
            if (encode == 0)
            {
                if (!string.IsNullOrEmpty(jsonback))
                {
                    return jsonback + "(" + jsonString + ")";
                }
                else
                {
                    return jsonString;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonback))
                {
                    return jsonback + "(" + HttpUtility.UrlEncode(jsonString) + ")";
                }
                else
                {
                    return HttpUtility.UrlEncode(jsonString);
                }

            }
        }
        /// <summary>
        /// json序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string JsonSerializerByWinForm<T>(T t, int encode = 0)
        {
            var jsonback = "";
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            string jsonString = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                ser.WriteObject(stream, t);
                jsonString = Encoding.UTF8.GetString(stream.ToArray());
                jsonString = Regex.Replace(jsonString, @"\\/Date\((\d+)\+0800\)\\/", match =>
                {
                    DateTime dt = new DateTime(1970, 1, 1);
                    dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                    dt = dt.ToLocalTime();
                    return dt.ToString("yyyy-MM-dd HH:mm:ss");
                });
            }
            if (encode == 0)
            {
                if (!string.IsNullOrEmpty(jsonback))
                {
                    return jsonback + "(" + jsonString + ")";
                }
                else
                {
                    return jsonString;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonback))
                {
                    return jsonback + "(" + HttpUtility.UrlEncode(jsonString) + ")";
                }
                else
                {
                    return HttpUtility.UrlEncode(jsonString);
                }

            }
        }
        /// <summary>
        /// json序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string JsonSerializer<T>(T t, bool changeDateTime)
        {
            string jsonString = string.Empty;
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

            using (MemoryStream stream = new MemoryStream())
            {
                ser.WriteObject(stream, t);
                jsonString = Encoding.UTF8.GetString(stream.ToArray());
                if (changeDateTime)
                {
                    jsonString = Regex.Replace(jsonString, @"\\/Date\((\d+)\+0800\)\\/", match =>
                    {
                        DateTime dt = new DateTime(1970, 1, 1);
                        dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                        dt = dt.ToLocalTime();
                        return dt.ToString("yyyy-MM-dd HH:mm:ss");
                    });
                }
            }
            var jsonback = HttpContext.Current.Request["jsoncallback"] == null ? "" : HttpContext.Current.Request["jsoncallback"];
            if (!string.IsNullOrEmpty(jsonback))
            {
                return jsonback + "(" + jsonString + ")";
            }
            else
            {
                return jsonString;
            }


        }

        public static string JsonSerializer<T>(IList<T> t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(IList<T>));
            string jsonString = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                ser.WriteObject(stream, t);
                jsonString = Encoding.UTF8.GetString(stream.ToArray());
                jsonString = Regex.Replace(jsonString, @"\\/Date\((\d+)\+0800\)\\/", match =>
                {
                    DateTime dt = new DateTime(1970, 1, 1);
                    dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                    dt = dt.ToLocalTime();
                    return dt.ToString("yyyy-MM-dd HH:mm:ss");
                });
            }
            var jsonback = HttpContext.Current.Request["jsoncallback"] == null ? "" : HttpContext.Current.Request["jsoncallback"];
            if (!string.IsNullOrEmpty(jsonback))
            {
                return jsonback + "(" + jsonString + ")";
            }
            else
            {
                return jsonString;
            }
        }

        /// <summary>
        /// json反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string jsonString)
        {
            jsonString = HttpUtility.UrlDecode(jsonString);
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            T obj;
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                obj = (T)ser.ReadObject(ms);
            }
            return obj;
        }
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }
        /// <summary>    
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串    
        /// </summary>    
        public  static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }
    }

    [DataContract]
    public class jsonCommModelList<T>
    {
        private DateTime _value = DateTime.Now;
        [DataMember(Order = 0)]
        public int backStatus { get; set; }
        [DataMember(Order = 1)]
        public string msg { get; set; }
        [DataMember(Order = 2)]
        public IList<T> Data { get; set; }
        [DataMember(Order = 3)]
        public Int32 recordCount { get; set; }
        [DataMember(Order = 4)]
        public Int32 pageCount { get; set; }
        [DataMember(Order = 5)]
        public DateTime currTime
        {
            set { _value = value; }
            get { return _value; }
        }
    }
    [DataContract]
    public class jsonCommModel<T>
    {
        private DateTime _value = DateTime.Now;
        [DataMember(Order = 0)]
        public int backStatus { get; set; }
        [DataMember(Order = 1)]
        public string msg { get; set; }
        [DataMember(Order = 2)]
        public T Data { get; set; }
        [DataMember(Order = 3)]
        public Int32 recordCount { get; set; }
        [DataMember(Order = 4)]
        public Int32 pageCount { get; set; }
        [DataMember(Order = 5)]
        public DateTime currTime
        {
            set { _value = value; }
            get { return _value; }
        }

    }
}
