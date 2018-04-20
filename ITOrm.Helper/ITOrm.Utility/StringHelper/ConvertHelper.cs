using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITOrm.Utility.StringHelper
{
    public static class ConvertHelper
    {
        public static long ToInt64(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return 0L;
            }
            try
            {
                return Convert.IsDBNull(value) ? 0 : Convert.ToInt64(value);
            }
            catch
            {
                return 0L;
            }
        }


        public static int ToInt32(object value)
        {
            return Convert.IsDBNull(value) ? 0 : Convert.ToInt32(value);
        }

        public static bool ToBoolean(object value)
        {
            return !Convert.IsDBNull(value) && Convert.ToBoolean(value);
        }

        public static DateTime ToDateTime(object value)
        {
            return Convert.IsDBNull(value) ? DateTime.MinValue : Convert.ToDateTime(value);
        }

        public static DateTime? ToNullableDateTime(Object value)
        {
            return Convert.IsDBNull(value) ? (DateTime?)null : Convert.ToDateTime(value);
        }

        public static byte[] ToByteArray(object obj)
        {
            return Convert.IsDBNull(obj) ? null : (byte[])obj;
        }

        public static decimal ToDecimal(object obj)
        {
            return Convert.IsDBNull(obj) ? 0.0m : Convert.ToDecimal(obj);
        }

        public static double ToDouble(object obj)
        {
            return Convert.IsDBNull(obj) ? 0.0 : Convert.ToDouble(obj);
        }

        public static string ToString(object obj)
        {
            return Convert.ToString(obj);
        }

        public static object ToDbType<T>(T obj) where T : class
        {
            if (obj == null) return DBNull.Value;
            return obj;
        }

        public static object ToDbType<T>(T? nullable) where T : struct
        {
            if (nullable == null) return DBNull.Value;
            return nullable.Value;
        }

        public static List<long> SplitToLongList(string strSrc)
        {
            return strSrc.Split(',').Select(str => long.Parse(str)).ToList();
        }

        public static List<string> SplitToStringList(string strSrc)
        {
            return strSrc.Split(',').ToList();
        }

        public static string ToStringByIEnumerable<T>(IEnumerable<T> value)
        {
            var result = new System.Text.StringBuilder();
            foreach (var obj in value)
            {
                result.Append(Convert.ToString(obj));
                result.Append(",");
            }
            return result.ToString().TrimEnd(',');
        }

     

        public static Guid ToGuid(object value)
        {
            return Convert.IsDBNull(value) ? Guid.Empty : Guid.Parse(value.ToString());
        }
    }
}
