using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ITOrm.EF.Models.Helper
{

    //整理此工具类，用于平日开发，里面的方法均是比较独立的小方法，并且是经常用到的方法
    public static class Util
    {
        #region 方法 String常用方法大全

        /// <summary>
        /// 检测是否为空 如果是空，返回:true;如果非空，返回:false
        /// </summary>
        /// <param name="str">所要判断的字符串</param>
        /// <returns>如果是空，返回:true;如果非空，返回:false</returns>
        public static bool IsNULL(this string str)
        {
            if (!string.IsNullOrEmpty(str) && !"".Equals(str) && str.Trim().Length > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 判断字符串是否都是有空白字符组成，空字符串将返回false
        /// </summary>
        /// <returns>如果该字符串都是空白字符，返回<c>true</c> ，否则返回<c>false</c></returns>
        public static bool IsWhiteSpace(this string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            if (s.Length == 0)
                return false;

            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsWhiteSpace(s[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 裁剪字符串，对Substring方法的改良，参数超出范围不抛出异常
        /// 若裁剪起始位置startIndex超出字符串长度，则返回空字符串,若裁剪长度length超出范围，则返回从startIndex开始的全部字符
        /// </summary>
        /// <param name="s"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CutString(this string s, int startIndex, int length)
        {
            //若裁剪起始位置超出字符串长度，则返回空字符串
            if (startIndex >= s.Length) return string.Empty;
            //字符串裁剪后的剩余长度
            int remainLength = s.Length - startIndex;
            if (length > remainLength)
                return s.Substring(startIndex);
            else
                return s.Substring(startIndex, length);
        }

        /// <summary>
        /// 字符串如果操过指定长度则将超出的部分用...代替
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_Length">指定长度</param>
        /// <returns>截取后的字符串...</returns>
        public static string GetSubString(this string p_SrcString, int p_Length)
        {
            return p_SrcString.GetSubString(0, p_Length, "...");
        }

        /// <summary>
        /// 字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(this string p_SrcString, int p_Length, string p_TailString)
        {
            return p_SrcString.GetSubString(0, p_Length, p_TailString);
        }
        /// <summary>
        /// 取指定长度的字符串，按字节截取，支持中拼音混用！注：中文一个字符为两个字节，拼音和数字及半角的字符都占一个字节！
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_StartIndex">起始位置</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(this string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {
            string myResult = p_SrcString;
            if (string.IsNullOrEmpty(myResult))
            {
                return string.Empty;
            }
            else
            {
                //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
                /*
                 if (System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\u4e00 - \u9fa5]+") ||
                     System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\xAC00-\xD7A3]+"))
                 {
                     //当截取的起始位置超出字段串长度时
                     if (p_StartIndex >= p_SrcString.Length)
                     {
                         return "";
                     }
                     else
                     {
                         return p_SrcString.Substring(p_StartIndex,
                                                        ((p_Length + p_StartIndex) > p_SrcString.Length) ? (p_SrcString.Length - p_StartIndex) : p_Length);
                     }
                 }
                 */
                //默认就是处理中拼音的了
                if (p_Length >= 0)
                {
                    byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);
                    //当字符串长度大于起始位置
                    if (bsSrcString.Length > p_StartIndex)
                    {
                        int p_EndIndex = bsSrcString.Length;
                        //当要截取的长度在字符串的有效长度范围内
                        if (bsSrcString.Length > (p_StartIndex + p_Length))
                        {
                            p_EndIndex = p_Length + p_StartIndex;
                        }
                        else
                        {   //当不在有效范围内时,只取到字符串的结尾

                            p_Length = bsSrcString.Length - p_StartIndex;
                            p_TailString = "";
                        }
                        int nRealLength = p_Length;
                        int[] anResultFlag = new int[p_Length];
                        byte[] bsResult = null;
                        int nFlag = 0;
                        for (int i = p_StartIndex; i < p_EndIndex; i++)
                        {
                            if (bsSrcString[i] > 127)
                            {
                                nFlag++;
                                if (nFlag == 3)
                                {
                                    nFlag = 1;
                                }
                            }
                            else
                            {
                                nFlag = 0;
                            }

                            anResultFlag[i] = nFlag;
                        }

                        if ((bsSrcString[p_EndIndex - 1] > 127) && (anResultFlag[p_Length - 1] == 1))
                        {
                            nRealLength = p_Length + 1;
                        }
                        bsResult = new byte[nRealLength];
                        Array.Copy(bsSrcString, p_StartIndex, bsResult, 0, nRealLength);
                        myResult = Encoding.Default.GetString(bsResult);
                        myResult = myResult + p_TailString;
                    }
                }

                return myResult;
            }
        }

        /// <summary>
        /// 获取字符串的长度，一个汉字算2个字符 
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>返回字符串长度</returns>
        public static int GetStringLength(this string str)
        {
            if (str.Length == 0) return 0;
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(str);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }
            }
            return tempLen;
        }

        /// <summary>
        /// 判断字符串是否注入SQL语句
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static bool SqlInJection(string str)
        {

            if (str != null)
            {
                str = str.ToLower();
                string Fy_In = "'|;|and|(|)|exec|insert|select|delete|update|count|*|%|chr|mid|master|truncate|char|declare";
                string[] fy_inf = Fy_In.Split('|');

                for (int i = 0; i < fy_inf.Length; i++)
                {
                    if (str.IndexOf(fy_inf[i]) != -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 随机产生一个全局唯一标识的GUID字符串
        /// </summary>
        public static string GetGUID
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }

        /// <summary>
        /// 将字符串的首字母转换成大写，后面的字母转换为小写，比如将user或USER转换成User
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string InitialToUpper(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return string.Concat(s.Substring(0, 1).ToUpper(), s.Substring(1).ToLower());
        }

        /// <summary>
        /// 将字符串的首字母转换成小写，比如将User转换成user
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string InitialToLower(string s)
        {
            return string.Concat(s.Substring(0, 1).ToLower(), s.Substring(1));
        }

        #endregion

        #region HTML操作
        /// <summary>
        /// 移除所有的Html标记 旧方法
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveAllHtml(string content)
        {
            string regexstr = @"<[^>]*>";
            return Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 去除Html字符串的Html代码 保留空格、回车和换行 多用于纯TextBox中
        /// </summary>
        /// <param name="content">源字符串</param>
        /// <returns>结果</returns>
        public static string RemoveHtml(string content)
        {
            //System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(@"</?(?!br|/?p)[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(@"</?[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(@"</?(?![^\x00-\xff])[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            string s = regEx.Replace(content, "");
            s = s.Replace("\r\n", "<br>");
            s = s.Replace("\n", "<br>");
            //s = s.Replace("\x20", "&nbsp;");
            s = s.Replace("\x20", " ");//硬空格
            return s;
        }

        /// <summary>
        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveUnsafeHtml(string content)
        {
            content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            return content;
        }
        /// <summary>
        /// 从HTML中获取文本,保留br,p,img
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string GetTextFromHTML(string HTML)
        {
            Regex regEx = new System.Text.RegularExpressions.Regex(@"</?(?!br|/?p|img)[^>]*>", RegexOptions.IgnoreCase);
            return regEx.Replace(HTML, "");
        }
        #endregion

        #region 常用类型转换，对象转换

        /// <summary>
        /// 将对象转换为Int32类型,转换失败返回0
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StringToInt(string str)
        {
            return StringToInt(str, 0);
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StringToInt(string str, int defValue)
        {
            if (string.IsNullOrEmpty(str) || str.Trim().Length >= 11 || !Regex.IsMatch(str.Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
                return defValue;

            int rv;
            if (Int32.TryParse(str, out rv))
                return rv;

            return Convert.ToInt32(StringToFloat(str, defValue));
        }

        /// <summary>
        /// 将对象转换成整数；如果不是整数，则返回0
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns>如果不是整数，则返回0</returns>
        public static int ObjectToInt(object strValue)
        {
            if ((strValue != null) && IsInt(strValue.ToString()))
                return int.Parse(strValue.ToString());
            return 0;
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ObjectToInt(object strValue, int defValue)
        {
            if (strValue != null)
                return StringToInt(strValue.ToString(), defValue);
            return defValue;
        }

        /// <summary>
        /// 判断字符串是否是整数或负整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static Boolean IsInt(String str)
        {

            if (!str.IsNULL())
            {
                return false;
            }
            if (str.Length > 10)
            {
                return false;
            }
            if (str.StartsWith("-"))
            {
                str = str.Substring(1, str.Length - 1);
            }
            char[] chArray = str.ToCharArray();
            foreach (char ch in chArray)
            {
                if (!char.IsDigit(ch))
                {
                    return false;
                }
            }
            if (chArray.Length == 10)
            {
                if (int.Parse(chArray[0].ToString()) > 2)
                {
                    return false;
                }
                if ((int.Parse(chArray[0].ToString()) == 2) && (int.Parse(chArray[1].ToString()) > 0))
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 将string 转换为List&lt;string&gt;,用指定的分隔符转换
        /// </summary>
        /// <param name="strObjects">字符串</param>
        /// <param name="splitChar">分隔符</param>
        /// <returns></returns>
        public static List<string> StringToList(string strObjects, char splitChar)
        {
            List<string> objects = new List<string>();
            if (!String.IsNullOrEmpty(strObjects))
            {
                string[] str = strObjects.Split(new char[] { splitChar }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in str)
                {
                    if (!objects.Contains(s))
                        objects.Add(s);
                }
            }
            return objects;
        }

        /// <summary>
        /// 检查参数不为空
        /// </summary>
        /// <param name="value">待检查的参数值</param>
        /// <param name="parameterName">参数名称</param>
        public static void ArgumentNotNull(object value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// 转换为Int64?，这里会将字符串true转换为1，false转换为0
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Int64 ToInt64(string s, Int64 defaultValue)
        {
            if (string.IsNullOrEmpty(s)) return defaultValue;
            if (s.ToLower().Trim() == "true") return 1;
            if (s.ToLower().Trim() == "false") return 0;

            Int64 n = 0;
            if (Int64.TryParse(s, out n))
                return n;
            else
                return defaultValue;
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StringToFloat(string strValue, float defValue)
        {
            if ((strValue == null) || (strValue.Length > 10))
                return defValue;

            float intValue = defValue;
            if (strValue != null)
            {
                bool IsFloat = Regex.IsMatch(strValue, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsFloat)
                    float.TryParse(strValue, out intValue);
            }
            return intValue;
        }

        /// <summary>
        /// 从 srcString 的开头剔除掉 trimString
        /// </summary>
        /// <param name="srcString"></param>
        /// <param name="trimString"></param>
        /// <returns></returns>
        public static String TrimStart(String srcString, String trimString)
        {
            if (srcString == null) return null;
            if (trimString == null) return srcString;
            if (IsNULL(srcString)) return String.Empty;
            if (srcString.StartsWith(trimString) == false) return srcString;
            return srcString.Substring(trimString.Length);
        }

        /// <summary>
        /// 从 srcString 的末尾剔除掉 trimString
        /// </summary>
        /// <param name="srcString"></param>
        /// <param name="trimString"></param>
        /// <returns></returns>
        public static String TrimEnd(String srcString, String trimString)
        {
            if (IsNULL(trimString)) return srcString;
            if (srcString.EndsWith(trimString) == false) return srcString;
            if (srcString.Equals(trimString)) return "";
            return srcString.Substring(0, srcString.Length - trimString.Length);
        }


        #endregion

        #region 加密

        /// <summary>
        /// 获得32位的MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5_32(this string input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] data = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获得16位的MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5_16(this string input)
        {
            return input.MD5_32().Substring(8, 16);
        }

        /// <summary>
        /// 获得8位的MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5_8(this string input)
        {
            return input.MD5_32().Substring(8, 8);
        }

        #endregion

        #region 其他方法
        /// <summary>
        /// 从应用程序根目录的文件中根据类型名称查找该类型所属的程序集
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Assembly FindAssemblyFromAppDirectory(string typeName)
        {
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string binPath = Path.Combine(rootPath, "bin");
            DirectoryInfo dir = new DirectoryInfo(rootPath);
            FileInfo[] files;
            files = dir.GetFiles("*.dll", SearchOption.TopDirectoryOnly);

            foreach (FileInfo file in files)
            {
                Assembly assembly = Assembly.LoadFile(file.FullName);
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (type.FullName == typeName)
                        return assembly;
                }
            }

            if (Directory.Exists(binPath) == false)
            {
                binPath = rootPath;
            }

            dir = new DirectoryInfo(binPath);
            files = dir.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
            foreach (FileInfo file in files)
            {
                Assembly assembly = Assembly.LoadFile(file.FullName);
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (type.FullName == typeName)
                        return assembly;
                }
            }
            return null;
        }

        /// <summary>
        /// 判断一个字符串是否是有效的整数值
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsInteger(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            int totalChars = text.Length;
            for (int i = 0; i < totalChars; i++)
            {
                if (char.IsNumber(text[i]) == false)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 判断是否是Decimal类型
        /// </summary>
        /// <param name="TBstr0">判断数据字符</param>
        /// <returns>true是false否</returns>
        public static bool IsDecimal(string TBstr0)
        {
            bool IsBool = false;
            string Intstr0 = "1234567890";
            string IntSign0, StrInt, StrDecimal;
            int IntIndex0, IntSubstr, IndexInt;
            int decimalbool = 0;
            int db = 0;
            bool Bf, Bl;
            if (TBstr0.Length > 2)
            {
                IntIndex0 = TBstr0.IndexOf(".");
                if (IntIndex0 != -1)
                {
                    string StrArr = ".";
                    char[] CharArr = StrArr.ToCharArray();
                    string[] NumArr = TBstr0.Split(CharArr);
                    IndexInt = NumArr.GetUpperBound(0);
                    if (IndexInt > 1)
                    {
                        decimalbool = 1;
                    }
                    else
                    {
                        StrInt = NumArr[0].ToString();
                        StrDecimal = NumArr[1].ToString();
                        //--- 整数部分－－－－－
                        if (StrInt.Length > 0)
                        {
                            if (StrInt.Length == 1)
                            {
                                IntSubstr = Intstr0.IndexOf(StrInt);
                                if (IntSubstr != -1)
                                {
                                    Bf = true;
                                }
                                else
                                {
                                    Bf = false;
                                }
                            }
                            else
                            {
                                for (int i = 0; i <= StrInt.Length - 1; i++)
                                {
                                    IntSign0 = StrInt.Substring(i, 1).ToString();
                                    IntSubstr = Intstr0.IndexOf(IntSign0);
                                    if (IntSubstr != -1)
                                    {
                                        db = db + 0;
                                    }
                                    else
                                    {
                                        db = i + 1;
                                        break;
                                    }
                                }

                                if (db == 0)
                                {
                                    Bf = true;
                                }
                                else
                                {
                                    Bf = false;
                                }
                            }
                        }
                        else
                        {
                            Bf = true;
                        }
                        //----小数部分－－－－
                        if (StrDecimal.Length > 0)
                        {
                            for (int j = 0; j <= StrDecimal.Length - 1; j++)
                            {
                                IntSign0 = StrDecimal.Substring(j, 1).ToString();
                                IntSubstr = Intstr0.IndexOf(IntSign0);
                                if (IntSubstr != -1)
                                {
                                    db = db + 0;
                                }
                                else
                                {
                                    db = j + 1;
                                    break;
                                }
                            }
                            if (db == 0)
                            {
                                Bl = true;
                            }
                            else
                            {
                                Bl = false;
                            }
                        }
                        else
                        {
                            Bl = false;
                        }
                        if ((Bf && Bl) == true)
                        {
                            decimalbool = 0;
                        }
                        else
                        {
                            decimalbool = 1;
                        }

                    }

                }
                else
                {
                    decimalbool = 1;
                }

            }
            else
            {
                decimalbool = 1;
            }

            if (decimalbool == 0)
            {
                IsBool = true;
            }
            else
            {
                IsBool = false;
            }

            return IsBool;
        }

        /// <summary>
        /// 判断一个字符串是否是有效的布尔值
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsBoolean(string text)
        {
            Regex booleanFormatRegex = new Regex("^(true|false)$", RegexOptions.IgnoreCase);
            return booleanFormatRegex.IsMatch(text);
        }

        /// <summary>
        /// 将字符串转换成 System.Decimal 类型。如果str不是整数或小数，返回0
        /// </summary>
        /// <param name="str"></param>
        /// <returns>如果str不是整数或小数，返回0</returns>
        public static decimal StringToDecimal(string str)
        {
            if (!IsDecimal(str))
            {
                return 0;
            }
            return Convert.ToDecimal(str);
        }


        /// <summary>
        /// 将字符串(不区分大小写)转换成 Boolean 类型。只有字符串等于1或者true时，才返回true
        /// </summary>
        /// <param name="str"></param>
        /// <returns>只有字符串等于1或者true时，才返回true</returns>
        public static bool StringToBool(string str)
        {
            if (str == null)
            {
                return false;
            }
            if (str.ToUpper().Equals("TRUE"))
            {
                return true;
            }
            if (str.ToUpper().Equals("FALSE"))
            {
                return false;
            }
            return (str.Equals("1") || str.ToUpper().Equals("TRUE"));
        }
        #endregion

        #region 排序方法

        /// <summary>
        /// 冒泡排序
        /// </summary>
        /// <param name="list"></param>
        public static void BubbleSort(int[] list)
        {
            //int[] iArrary = new int[] { 1, 5, 13, 6, 10, 55, 99, 2, 87, 12, 34, 75, 33, 47 };
            int i, j, temp;
            bool done = false;
            j = 1;
            while ((j < list.Length) && (!done))
            {
                done = true;
                for (i = 0; i < list.Length - j; i++)
                {
                    if (list[i] > list[i + 1])
                    {
                        done = false;
                        temp = list[i];
                        list[i] = list[i + 1];
                        list[i + 1] = temp;
                    }
                }
                j++;
            }
        }

        /// <summary>
        /// 选择排序
        /// </summary>
        /// <param name="list"></param>
        public static void SelectionSort(int[] list)
        {
            int min;
            for (int i = 0; i < list.Length - 1; i++)
            {
                min = i;
                for (int j = i + 1; j < list.Length; j++)
                {
                    if (list[j] < list[min])
                        min = j;
                }
                int t = list[min];
                list[min] = list[i];
                list[i] = t;
            }
        }

        /// <summary>
        /// 插入排序
        /// </summary>
        /// <param name="list"></param>
        public static void InsertionSort(int[] list)
        {
            for (int i = 1; i < list.Length; i++)
            {
                int t = list[i];
                int j = i;
                while ((j > 0) && (list[j - 1] > t))
                {
                    list[j] = list[j - 1];
                    --j;
                }
                list[j] = t;
            }
        }


        /// <summary>
        /// 希尔排序是将组分段,进行插入排序
        /// </summary>
        /// <param name="list"></param>
        public static void ShellSort(int[] list)
        {
            int inc;
            for (inc = 1; inc <= list.Length / 9; inc = 3 * inc + 1) ;
            for (; inc > 0; inc /= 3)
            {
                for (int i = inc + 1; i <= list.Length; i += inc)
                {
                    int t = list[i - 1];
                    int j = i;
                    while ((j > inc) && (list[j - inc - 1] > t))
                    {
                        list[j - 1] = list[j - inc - 1];
                        j -= inc;
                    }
                    list[j - 1] = t;
                }
            }
        }


        #endregion
    }
}
