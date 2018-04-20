using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Configuration;
using System.Web;
using System.Xml;

namespace ITOrm.Utility.Encryption
{
    public class SiteSettingsHelper
    {

        private static SiteSettingsHelper _instance = new SiteSettingsHelper();
      
        /// <summary>
        /// Get setting
        /// </summary>
        public static SiteSettingsHelper Instance
        {
            get { return _instance ?? (_instance = new SiteSettingsHelper()); }
        }
        public T GetSetting<T>(String key, T defaultValue)
        {
            if (ConfigurationManager.AppSettings[key] == null)
                return defaultValue;
            if (typeof(T) == typeof(String))
                return (T)(object)ConfigurationManager.AppSettings[key];
            if (typeof(T) == typeof(int))
                return (T)(object)(Int32.Parse(ConfigurationManager.AppSettings[key]));
            if (typeof(T) == typeof(bool))
                return (T)(object)(bool.Parse(ConfigurationManager.AppSettings[key]));
            return (T)(object)ConfigurationManager.AppSettings[key];
        }
        public string GetMethod(string infor)
        {
            return infor.Split('-')[1];
        }
        public string Week(DateTime date)
        {
            string[] weekdays = { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            var week = weekdays[Convert.ToInt32(date.DayOfWeek)];
            return week;
        }

        public string SiteVersion { get { return GetSetting("SiteVersion", DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)); } }

        public string EncryptionKey
        {
            get { return GetSetting("EncryptionKey", "5db2fd833fe624e086277ee8fdb3deb7"); }
        }


    }
}
