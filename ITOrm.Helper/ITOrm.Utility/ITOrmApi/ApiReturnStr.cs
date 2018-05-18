using ITOrm.Utility.Serializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace ITOrm.Utility.ITOrmApi
{
    public static class ApiReturnStr
    {
        public static JsonSerializerSettings jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        public static IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();

        static ApiReturnStr()
        {
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }
        public static string getApiDataList(JArray t)
        {
            var result = CreateOKJObject();
            result[DATA] = t;
            return result.ToString();
        }

        public static string getApiData(JObject t=null)
        {
            var result = CreateOKJObject();
            result[DATA] = t;
            return result.ToString();
        }
        public static string getApiData<T>(T t )
        {
            var result = CreateOKJObject();
            JObject data = JObject.FromObject(t);
            result[DATA] = data;
            return result.ToString();
        }


        public static string getApiData()
        {
            var result = CreateOKJObject();
            return result.ToString();
        }

        public static string getApiData(int backState = 0, string msg = "", JObject t = null)
        {
            var result = CreateOKJObject();
            result[DATA] = t;
            result[BACK_STATUS] = backState;
            result[MESSAGE] = msg;
            return result.ToString();
        }

        public static string getApiDataListByPage(JArray list, int recordCount,int pageIndex,int pageSize)
        {
            var result = CreateOKJObjectList();
            result[DATA][LIST] = list;
            result[DATA][PAGE_COUNT] = (recordCount % pageSize==0?(recordCount/pageSize):(recordCount/pageSize)+1);
            result[DATA][RECORD_COUNT] = recordCount;
            result[DATA][PAGE_INDEX] = pageIndex;
            result[DATA][PAGE_SIZE] = pageSize;
            return result.ToString();
        }

        public static string getError(int backState=-100,string msg="")
        {
            var result = CreateOKJObject();
            result[BACK_STATUS] = backState;
            result[MESSAGE] = msg;
            return result.ToString();
        }
        public static readonly string BACK_STATUS = "backStatus";
        public static readonly string MESSAGE = "message";
        public static readonly string DATA = "data";
        public static readonly string CURRTIME = "serverTime";
        public static JObject CreateOKJObject()
        {
            var result = new JObject();
            result[BACK_STATUS] = 0;
    	    result[MESSAGE] = "success";
            result[CURRTIME] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var data = new JObject();
            result[DATA] = data;
    	    return result;
        }
        public static readonly string LIST = "list";
        public static readonly string PAGE_COUNT = "pageCount";
        public static readonly string RECORD_COUNT = "recordCount";
        public static readonly string PAGE_INDEX = "pageIndex";
        public static readonly string PAGE_SIZE = "pageSize";

        public static JObject CreateOKJObjectList()
        {
            var result = CreateOKJObject();
            var data = new JObject();
            var list = new JObject();

            data[LIST] = list;
            data[PAGE_COUNT] = 0;
            data[RECORD_COUNT] = 0;
            data[PAGE_INDEX] = 1;
            data[PAGE_SIZE] = 10;
            result[DATA] = data;
            return result;
        }
    }
}
