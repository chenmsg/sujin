using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class DecimalExtensions
{
    /// <summary>
    /// 将字符串转换为Int
    /// </summary>
    /// <param name="t"></param>
    /// <returns>当转换失败时返回0</returns>
    public static int ToInt(this decimal value)
    {
        return (int)System.Math.Round(value);
    }


    /// <summary>
    /// 四舍五入
    /// </summary>
    /// <param name="Value">Value to round</param>
    /// <param name="Digits">Digits to round to</param>
    /// <param name="Rounding">Rounding mode to use</param>
    /// <returns></returns>
    public static decimal Rounding(this decimal value, int digits=2)
    {
        return System.Math.Round(value, digits, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// 转换为百分号
    /// </summary>
    /// <param name="Value">Value to round</param>
    /// <param name="Digits">Digits to round to</param>
    /// <param name="Rounding">Rounding mode to use</param>
    /// <returns></returns>
    public static string perCent(this decimal value)
    {
        int digits = 2;
        var result= (value*100M).Rounding(digits);
        return result.ToString("F2")+"%";
    }

}

