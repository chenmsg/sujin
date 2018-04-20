using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITOrm.Core.Memcached.Impl;
using Newtonsoft.Json;
namespace ITOrm.Utility.Cache
{
    public static class MemcachHelper
    {
        public static MemcachedDao dao = MemcachedDao.GetInstance();
        public static object Get(string key)
        {
            return dao.Get(key);
        }

        public static void Set(string key ,object obj,int min)
        {
            TimeSpan ts = DateTime.Now.AddSeconds(min) - DateTime.Now;
            dao.Store(key, obj, ts);
        }
        public static void Set(string key, object obj, DateTime exprise)
        {
            TimeSpan ts = exprise - DateTime.Now;
            dao.Store(key, obj, ts);
        }

        public static bool Delete(string key)
        {
            return dao.Remove(key);
        }

        public static bool DeleteAll()
        {
            return dao.RemoveAll();
        }

        public static bool Exists(string key)
        {
            return dao.Exists(key);
        }

        public delegate T InsertCacheFun<T>();


        /// <summary>
        /// //委托方法判断是否存在，不存在就缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dt">到期日期</param>
        /// <param name="getDataFun">方法</param>
        /// <returns></returns>
        public static T Get<T>(string key, DateTime dt, InsertCacheFun<T> getDataFun)
        {
            object obj = null;
            if (!Exists(key))
            {
                var objList= getDataFun();
                obj = JsonConvert.SerializeObject(objList);
                Set(key, obj, dt);

            }
            else
            {
                obj = Get(key);
            }
            return JsonConvert.DeserializeObject<T>(obj.ToString());
        }

        /// <summary>
        /// 委托方法判断是否存在，不存在就缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="min">到期秒数</param>
        /// <param name="getDataFun">方法</param>
        /// <returns></returns>
        public static T Get<T>(string key, int min, InsertCacheFun<T> getDataFun)
        {
            object obj = null;
            if (!Exists(key))
            {
                obj = getDataFun();
                Set(key, JsonConvert.SerializeObject(obj), min);
                obj = Get(key);
            }
            else
            {
                obj = Get(key);
            }
            return JsonConvert.DeserializeObject<T>(obj.ToString());
        }

        /// <summary>
        /// 委托方法判断是否存在，不存在就缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="min">到期秒数</param>
        /// <param name="getDataFun">方法</param>
        /// <returns></returns>
        public static string GetString(string key, int min, InsertCacheFun<string> getDataFun)
        {
            string obj = string.Empty;
            if (!Exists(key))
            {
                obj = getDataFun();
                Set(key, obj, min);
            }
            else
            {
                obj = Get(key) as string;
            }
            return obj.ToString();
        }
    }
}
