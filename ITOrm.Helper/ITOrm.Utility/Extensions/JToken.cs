using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public static class JTokenExtensions
{
    /// <summary>
    /// 将字符串转换为Int
    /// </summary>
    /// <param name="t"></param>
    /// <returns>当转换失败时返回0</returns>
    public static int ToInt(this JToken item)
    {
        if (item != null && !string.IsNullOrEmpty(item.ToString()))
            return Convert.ToInt32(item.ToString());
        return 0;
    }

    /// <summary>
    /// 将字符串转换为ToDecimal
    /// </summary>
    /// <param name="t"></param>
    /// <returns>当转换失败时返回0</returns>
    public static decimal ToDecimal(this JToken item)
    {
        
        if (item != null && !string.IsNullOrEmpty(item.ToString()))
            return Convert.ToDecimal(item.ToString());
        return 0;
    }

    /// <summary>
    /// 将字符串转换为 ToDateTime
    /// </summary>
    /// <param name="t"></param>
    /// <returns>当转换失败时返回0</returns>
    public static DateTime ToDateTime(this JToken item)
    {
        
        if (item != null && !string.IsNullOrEmpty(item.ToString()))
            return Convert.ToDateTime(item.ToString());
        return DateTime.Now;
    }


    
}
