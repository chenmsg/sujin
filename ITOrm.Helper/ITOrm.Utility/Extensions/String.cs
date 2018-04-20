using System;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using ITOrm.Utility.StringHelper;

/// <summary>
/// Extension methods for strings
/// </summary>
public static class StringExtensions
{

    #region 转换

    /// <summary>
    /// 将字符串转换为Int
    /// </summary>
    /// <param name="t"></param>
    /// <returns>当转换失败时返回0</returns>
    public static int ToInt(this string value)
    {
        return value.ToInt(0);
    }

    public static int ToInt(this string value, int defaultValue)
    {
        var result = defaultValue;
        return int.TryParse(value, out result) ? result : defaultValue;
    }


    public static decimal ToDecimal(this string value)
    {
        return value.ToDecimal(0);
    }

    public static decimal ToDecimal(this string value, decimal defaultValue)
    {
        var result = defaultValue;
        return decimal.TryParse(value, out result) ? result : defaultValue;
    }

    public static string ToNoTag(this string value)
    {
        return RegexHelper.Replace(value, RegexPattern.Tag, "");
    }

    public static decimal ToRoundDecimal(this string value, decimal defaultValue, int decimals)
    {
        var result = defaultValue;
        result = Math.Round(decimal.TryParse(value, out result) ? result : defaultValue, decimals);
        return result;
    }

    public static DateTime ToDateTime(this string value)
    {
        return DateTime.Parse(value);
    }

    public static DataTable ToDateTable(this string[] values)
    {
        DataTable dt = new DataTable();
        foreach (string value in values)
        {
            dt.Columns.Add(value);
        }

        return dt;
    }

    public static decimal ToMasgetAmount(this string value)
    {
        decimal k = value.ToDecimal();
        if (k == 0) return 0M;
        decimal result = k / 100;
        return result;
    }

    public static decimal ToMasgetYield(this string value)
    {
        decimal k = value.ToDecimal();
        if (k == 0) return 0M;
        decimal result = k / 10000000;
        return result;
    }

    public static string ToBankLastFour(this string value)
    {
        if (value.Length > 4)
        {
            return value.Substring(value.Length - 4, 4);
        }
        return value;
    }
    #endregion

    #region 判断

    public static bool IsEMail(this string value)
    {
        return IsMatch(value, RegexPattern.EMail);
    }

    public static bool IsMobile(this string value)
    {
        return IsMatch(value, RegexPattern.Mobile);
    }

    public static bool IsUserName(this string value)
    {
        return IsMatch(value, RegexPattern.UserName);
    }


    private static bool IsMatch(string value, string pattern, RegexOptions options)
    {
        return new Regex(pattern, options).IsMatch(value);
    }

    private static bool IsMatch(string value, string pattern)
    {
        return IsMatch(value, pattern, RegexOptions.IgnoreCase);
    }

    #endregion

    #region 替换

    /// <summary>
    /// Removes illegal characters from a directory
    /// </summary>
    /// <param name="DirectoryName">Directory name</param>
    /// <param name="ReplacementChar">Replacement character</param>
    /// <returns>DirectoryName with all illegal characters replaced with ReplacementChar</returns>
    public static string RemoveIllegalDirectoryNameCharacters(this string directoryName, char replacementChar)
    {
        if (string.IsNullOrEmpty(directoryName))
            return directoryName;
        foreach (char c in Path.GetInvalidPathChars())
            directoryName = directoryName.Replace(c, replacementChar);
        return directoryName;
    }


    /// <summary>
    /// Removes illegal characters from a file
    /// </summary>
    /// <param name="FileName">File name</param>
    /// <param name="ReplacementChar">Replacement character</param>
    /// <returns>FileName with all illegal characters replaced with ReplacementChar</returns>
    public static string RemoveIllegalFileNameCharacters(this string fileName, char replacementChar)
    {
        if (string.IsNullOrEmpty(fileName))
            return fileName;
        foreach (char c in Path.GetInvalidFileNameChars())
            fileName = fileName.Replace(c, replacementChar);
        return fileName;
    }

    public static string ConvertBank(this string str)
    {
        if(str=="平安银行")
        {
            return "深圳发展银行";
        }
        if (str == "广发银行")
        {
            return "广发银行股份有限公司";
        }
        if (str == "浦发银行")
        {
            return "浦东发展银行";
        }
        return str;
    }
    #endregion

}