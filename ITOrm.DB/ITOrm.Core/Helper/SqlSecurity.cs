using System;
using System.Text.RegularExpressions;

namespace ITOrm.Core.Helper
{
    /// <summary>
    /// SQL语句的安全帮助类
    /// </summary>
    public class SqlSecurity
    {
        private string SafeSqlLiteral(string inputSQL)
        {
            return inputSQL.Replace("'", "''");
        }

        /// <summary>
        /// 检查输入的字符串是否符合注入Sql的规则
        /// </summary>
        /// <param name="Input">待验证字符串</param>
        /// <returns>true 表示符合注入规则，false表示不符合注入规则（非注入字符串）</returns>
        public static bool CheckInjection(string inputSqlString)
        {
            string re = @"(select)|(\-\-)|(insert)|(delete)|(count)|(create)|(drop)|(alter)|(drop)|(update)|( asc )|( mid | char )|( exec )|( or )|(declare)|(')|(dbcc)|(sp_)";
            return Regex.IsMatch(inputSqlString, re, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }


        /// <summary>
        /// 检查输入的字符串是否符合注入Sql的规则
        /// </summary>
        /// <param name="Input">待验证字符串</param>
        /// <returns>true 表示符合注入规则，false表示不符合注入规则（非注入字符串）</returns>
        public static string ReplaceInjection(string inputSqlString)
        {
            if (string.IsNullOrEmpty(inputSqlString)) return inputSqlString;

            string re = @"(select)|(insert)|(delete)|(count)|(create)|(drop)|(alter)|(like)|(\-\-)|(%)|(=)|(')|(drop)|(update)|( asc )|( mid | char )|( exec )|( or )|(declare)|(')|(dbcc)|(sp_)";
            return Regex.Replace(inputSqlString, re, " ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        /// <summary>
        /// 检查输入的字符串是否符合注入Sql的规则,忽略“=”
        /// </summary>
        /// <param name="Input">待验证字符串</param>
        /// <returns>true 表示符合注入规则，false表示不符合注入规则（非注入字符串）</returns>
        public static string ReplaceInjection2(string inputSqlString)
        {
            string re = @"(select)|(insert)|(delete)|(count)|(create)|(drop)|(alter)|(like)|(\-\-)|(%)|(')|(drop)|(update)|( asc )|( mid | char )|( exec )|( or )|(declare)|(')|(dbcc)|(sp_)";
            return Regex.Replace(inputSqlString, re, " ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }
        /// <summary>
        /// 检查输入的字符串是否符合注入Sql的规则,忽略“=”,"'"
        /// </summary>
        /// <param name="Input">待验证字符串</param>
        /// <returns>true 表示符合注入规则，false表示不符合注入规则（非注入字符串）</returns>
        public static string ReplaceInjection3(string inputSqlString)
        {
            string re = @"(select)|(insert)|(delete)|(count)|(create)|(drop)|(alter)|(like)|(\-\-)|(%)|(drop)|(update)|( asc )|( mid | char )|( exec )|( or )|(declare)|(dbcc)|(sp_)";
            return Regex.Replace(inputSqlString, re, " ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        /// <summary>
        /// 获得字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isCheck">是否进行Sql注入验证</param>
        /// <returns></returns>
        public static string GetStringQueryPara(string paravalue, bool isCheck)
        {
            string result = String.Empty;

            if (!String.IsNullOrEmpty(paravalue))
            {
                result = paravalue.Trim().Replace("'", " ");
                if (isCheck && CheckInjection(result))
                {
                    result = "";
                }
            }
            return result;
        }

        /// <summary>
        /// 获得字符串，仅包括字符[a-zA-Z0-9\u4e00-\u9fa5]
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isCheck">是否进行Sql注入验证</param>
        /// <returns></returns>
        public static string GetStringOnlyWord(string paravalue, bool isCheck)
        {
            string result = String.Empty;
            if (!String.IsNullOrEmpty(paravalue))
            {
                Regex reg = new Regex("[^a-zA-Z0-9\u4e00-\u9fa5]");
                result = reg.Replace(paravalue.Trim(), " ");
                result = Regex.Replace(result, " +", " ");
                if (isCheck && CheckInjection(result))
                {
                    result = "";
                }
            }
            return result;
        }

        /// <summary>
        /// 获得整数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetIntQueryPara(string paravalue)
        {
            int result = 0;
            if (!String.IsNullOrEmpty(paravalue))
            {
                try
                {
                    result = int.Parse(paravalue.Trim());
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 获得Decimal
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static decimal GetDecimalQueryPara(string paravalue)
        {
            decimal result = 0.0m;
            if (!String.IsNullOrEmpty(paravalue))
            {
                try
                {
                    result = decimal.Parse(paravalue.Trim());
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 获得Decimal
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static float GetFloatQueryPara(string paravalue)
        {
            float result = 0.0f;
            if (!String.IsNullOrEmpty(paravalue))
            {
                try
                {
                    result = float.Parse(paravalue.Trim());
                }
                catch { }
            }
            return result;
        }


        /// <summary>
        /// 获得字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isCheck">是否进行Sql注入验证</param>
        /// <returns></returns>
        public static string GetStringQueryPara(string paravalue, bool isCheck, string defaultvalue)
        {
            string result = defaultvalue;

            if (!String.IsNullOrEmpty(paravalue))
            {
                result = paravalue.Trim().Replace("'", " ");
                if (isCheck && CheckInjection(result))
                {
                    result = "";
                }
            }
            return result;
        }

        /// <summary>
        /// 获得整数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetIntQueryPara(string paravalue, int defaultvalue)
        {
            int result = defaultvalue;
            if (!String.IsNullOrEmpty(paravalue))
            {
                try
                {
                    result = int.Parse(paravalue.Trim());
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 获得Decimal
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static decimal GetDecimalQueryPara(string paravalue, decimal defaultvalue)
        {
            decimal result = defaultvalue;
            if (!String.IsNullOrEmpty(paravalue))
            {
                try
                {
                    result = decimal.Parse(paravalue.Trim());
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 获得Decimal
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static float GetFloatQueryPara(string paravalue, float defaultvalue)
        {
            float result = defaultvalue;
            if (!String.IsNullOrEmpty(paravalue))
            {
                try
                {
                    result = float.Parse(paravalue.Trim());
                }
                catch { }
            }
            return result;
        }
    }
}
