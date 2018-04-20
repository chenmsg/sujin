/**************************************
* 作用：数据类型检测
* 作者：Stone
* 日期: 2008-02-11
**************************************/
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ITOrm.Utility.StringHelper
{
    /// <summary>
    /// 数据类型检测
    /// </summary>
    public class TypeParse
    {       
        /// <summary>
        /// 判断是否为有效邮箱
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns></returns>
        public static bool IsMail(string s)
        {
            return Regex.IsMatch(s, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            //^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$;            
            //^([\\w-.]+)@(([[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.)|(([\\w-]+.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(]?)$
        }

        /// <summary>
        /// 用户名
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsUserName(string s)
        {
            return Regex.IsMatch(s, @"^\\w+$");
        }
        /// <summary>
        /// 验证真实姓名
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsChinese(string s)
        {
            return Regex.IsMatch(s, @"^[\u4E00-\u9FA5]{2,5}(?:·[\u4E00-\u9FA5]{2,6})*$");
        }

        /// <summary>
        /// 判断是否为字符或是数字
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns></returns>
        public static bool IsCharNum(string s)
        {
            return Regex.IsMatch(s, @"^[A-Za-z0-9]+$");
        }

        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(object Expression)
        {
            int ret;
            return int.TryParse(Expression.ToString(), out ret);
        }

        /// <summary>
        /// 判断是整数或小数
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object Expression)
        {
            int ret;
            return int.TryParse(Expression.ToString(), out ret);
        }
        /// <summary>
        /// 判断是否是浮点型数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsFloat(object Expression)
        {
            float ret;            
            return float.TryParse(Expression.ToString(), out ret);
        }
        /// <summary>
        /// 判断是否是货币类型数字（十进制）
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsDecimal(object Expression)
        {
            decimal ret;
            return decimal.TryParse(Expression.ToString(), out ret);
        }
        ///   <summary> 
        ///   判断一个字符串是否是正整数型的字符串 
        ///   </summary> 
        ///   <param   name= "strValue "> 字符串 </param> 
        ///   <returns> 是则返回true，否则返回false </returns> 
        public static bool IsUnsign(string strValue)
        {
            int ret;
            if (strValue == "")
            {
                return false;
            }
            if (strValue.Substring(0, 1) == "-")
            {
                return false;
            }
            else
            {
                return int.TryParse(strValue, out ret);
            }
        }

        ///   <summary> 
        ///   判断一个字符串是否是正数型的字符串 
        ///   </summary> 
        ///   <param   name= "strValue "> 字符串 </param> 
        ///   <returns> 是则返回true，否则返回false </returns> 
        public static bool IsUnsignDouble(string strValue)
        {
            if (strValue == "")
                return false;
            double ret;
            if (strValue.Substring(0, 1) == "-")
            {
                return false;
            }
            else
            {
                return double.TryParse(strValue, out ret);
            }
        }

        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(object Expression, bool defValue)
        {
            if (Expression != null)
            {
                if (string.Compare(Expression.ToString(), "true", true) == 0)
                {
                    return true;
                }
                else if (string.Compare(Expression.ToString(), "false", true) == 0)
                {
                    return false;
                }
            }
            return defValue;
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(object Expression, int defValue)
        {
            if (Expression != null)
            {
                string str = Expression.ToString();
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                    {
                        return Convert.ToInt32(str);
                    }
                }
            }
            return defValue;
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(object strValue, float defValue)
        {
            if ((strValue == null) || (strValue.ToString().Length > 10))
            {
                return defValue;
            }

            float intValue = defValue;
            if (strValue != null)
            {
                bool IsFloat = Regex.IsMatch(strValue.ToString(), @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsFloat)
                {
                    intValue = Convert.ToSingle(strValue);
                }
            }
            return intValue;
        }

        /// <summary>
        /// 判断给定的字符串数组(strNumber)中的数据是不是都为数值型
        /// </summary>
        /// <param name="strNumber">要确认的字符串数组</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsNumericArray(string[] strNumber)
        {
            if (strNumber == null)
            {
                return false;
            }
            if (strNumber.Length < 1)
            {
                return false;
            }
            foreach (string id in strNumber)
            {
                if (!IsNumeric(id))
                {
                    return false;
                }
            }
            return true;

        }

        /// <summary>
        /// 将字符串数组转成字符串
        /// </summary>
        /// <param name="strArray">字符串数组</param>        
        /// <param name="Separator">分隔符</param>
        /// <returns>字符串</returns>
        public static string StringArrayToString(string[] strArray, char Separator)
        {
            string str = string.Empty;
            for (int i = 0; i < strArray.Length; i++)
            {
                str = str + strArray[i] + Separator;
            }
            return str.Substring(0, str.Length - 1);
        }

        /// <summary>
        /// 判断字符串是否在字符串数组中存在
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="strArray">字符串数组</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsStringArray(string str, string[] strArray)
        {
            bool s = false;
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i] == str)
                {
                    s = true;
                }
            }
            return s;
        }
        /// <summary>
        /// 验证电话号码
        /// </summary>
        /// <param name="str_telephone"></param>
        /// <returns></returns>
        public static bool IsTelephone(string str_telephone)
        {
            string strrphone = @"^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$";
            return System.Text.RegularExpressions.Regex.IsMatch(str_telephone, strrphone);
            //return System.Text.RegularExpressions.Regex.IsMatch(str_telephone, @"^(\(\d{3,4}-)|\d{3.4}-)?\d{7,8}$");

        }
        /// <summary>
        /// 验证手机号码
        /// </summary>
        /// <param name="str_handset"></param>
        /// <returns></returns>
        public static bool IsMobile(string str_handset)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_handset, @"^\d{11}$");
        }
        /// <summary>
        /// 验证身份证号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsIdentity(string str)
        {
            string number17 = str.Substring(0, 17);
            string number18 = str.Substring(17);
            string check = "10X98765432";
            int[] num = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            int sum = 0;
            for (int i = 0; i < number17.Length; i++)
            {
                sum += Convert.ToInt32(number17[i].ToString()) * num[i];
            }
            sum %= 11;
            if (number18.Equals(check[sum].ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 验证护照
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsPassport(string str)
        {
            bool flag=System.Text.RegularExpressions.Regex.IsMatch(str, @"^((1[45][0-9]{7})|(G[0-9]{8})|(P[0-9]{7})|([A-Z]{2}[0-9]{7})|(S[0-9]{7,8})|(D[0-9]+))$");
            return flag;
        }
    }
}
