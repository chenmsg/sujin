using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Core.Helper
{
    /// <summary>
    /// 时间帮助类
    /// </summary>
    public class TimeHelper
    {
        #region 时间间隔
        /// <summary>
        /// 计算日期间隔
        /// </summary>
        /// <param name="d1">要参与计算的其中一个日期字符串
        /// <param name="d2">要参与计算的另一个日期字符串
        /// <returns>一个表示日期间隔的TimeSpan类型</returns>
        public static TimeSpan toResult(string d1, string d2)
        {
            try
            {
                DateTime date1 = DateTime.Parse(d1);
                DateTime date2 = DateTime.Parse(d2);
                return toResult(date1, date2);
            }
            catch
            {
                throw new Exception("字符串参数不正确!");
            }
        }
        /// <summary>
        /// 计算日期间隔
        /// </summary>
        /// <param name="d1">要参与计算的其中一个日期
        /// <param name="d2">要参与计算的另一个日期
        /// <returns>一个表示日期间隔的TimeSpan类型</returns>
        public static TimeSpan toResult(DateTime d1, DateTime d2)
        {
            TimeSpan ts;
            if (d1 > d2)
            {
                ts = d1 - d2;
            }
            else
            {
                ts = d2 - d1;
            }
            return ts;
        }

        /// <summary>
        /// 计算日期间隔
        /// </summary>
        /// <param name="d1">要参与计算的其中一个日期字符串
        /// <param name="d2">要参与计算的另一个日期字符串
        /// <param name="drf">决定返回值形式的枚举
        /// <returns>一个代表年月日的int数组，具体数组长度与枚举参数drf有关</returns>
        public static int[] toResult(string d1, string d2, DiffResultFormat drf)
        {
            try
            {
                DateTime date1 = DateTime.Parse(d1);
                DateTime date2 = DateTime.Parse(d2);
                return toResult(date1, date2, drf);
            }
            catch
            {
                throw new Exception("字符串参数不正确!");
            }
        }
        /// <summary>
        /// 计算日期间隔
        /// </summary>
        /// <param name="d1">要参与计算的其中一个日期
        /// <param name="d2">要参与计算的另一个日期
        /// <param name="drf">决定返回值形式的枚举
        /// <returns>一个代表年月日的int数组，具体数组长度与枚举参数drf有关</returns>
        public static int[] toResult(DateTime d1, DateTime d2, DiffResultFormat drf)
        {
            #region 数据初始化
            DateTime max;
            DateTime min;
            int year;//年
            int month;//月
            int day;//天
            int hour;//小时
            int minute;//分钟
            int tempYear, tempMonth, tempDay, tempHour, tempMinute;
            if (d1 > d2)
            {
                max = d1;
                min = d2;
            }
            else
            {
                max = d2;
                min = d1;
            }
            tempYear = max.Year;
            tempMonth = max.Month;
            tempDay = max.Day;
            tempHour = max.Hour;
            tempMinute = max.Minute;
            if (max.Month < min.Month)
            {
                tempYear--;
                tempMonth = tempMonth + 12;
            }
            if (max.Day < min.Day)
            {
                tempMonth--;
                tempDay = tempDay + GetDay(tempYear, tempMonth);
            }
            if (max.Hour < min.Hour)
            {
                tempDay--;
                tempHour = tempHour + 24;
            }
            if (max.Minute < min.Minute)
            {
                tempHour--;
                tempMinute = tempMinute + 60;
            }
            year = tempYear - min.Year;
            month = tempMonth - min.Month;
            day = tempDay - min.Day;
            hour = tempHour - min.Hour;
            minute = tempMinute - min.Minute;
            #endregion
            #region 按条件计算
            if (drf == DiffResultFormat.dd)
            {
                TimeSpan ts = max - min;
                return new int[] { ts.Days };
            }
            if (drf == DiffResultFormat.mm)
            {
                return new int[] { month + year * 12 };
            }
            if (drf == DiffResultFormat.yy)
            {
                return new int[] { year };
            }
            if (drf == DiffResultFormat.yymm)
            {
                return new int[] { year, month };
            }
            if (drf == DiffResultFormat.yyMMddHHmm)
            {
                return new int[] { year, month, day, hour, minute };
            }
            return new int[] { year, month, day, hour, minute };
            #endregion
        }


        /// <summary>
        /// 计算日期间隔多少个月
        /// </summary>
        /// <param name="d1">要参与计算的其中一个日期
        /// <param name="d2">要参与计算的另一个日期
        /// <param name="drf">决定返回值形式的枚举
        /// <returns>一个代表年月日的int数组，具体数组长度与枚举参数drf有关</returns>
        public static int ToMonthResult(DateTime d1, DateTime d2)
        {
            #region 数据初始化
            DateTime max;
            DateTime min;
            int year;//年
            int month;//月
            int tempYear, tempMonth;
            if (d1 > d2)
            {
                max = d1;
                min = d2;
            }
            else
            {
                max = d2;
                min = d1;
            }
            tempYear = max.Year;
            tempMonth = max.Month;
            if (max.Month < min.Month)
            {
                tempYear--;
                tempMonth = tempMonth + 12;
            }
            year = tempYear - min.Year;
            month = tempMonth - min.Month;
            #endregion
            return (month + year * 12);
        }

        //计算某年的某月有多少天
        public static int GetDay(int y, int m)
        {
            if (m == 1 || m == 3 || m == 5 || m == 7 || m == 8 || m == 10 || m == 12)
            {
                return 31;
            }
            if (m == 4 || m == 6 || m == 9 || m == 11)
            {
                return 30;
            }
            else if (m == 2)
            {
                if (y % 400 == 0 || y % 4 == 0 && y % 100 != 0)
                {
                    return 29;
                }
                else
                {
                    return 28;
                }
            }
            return 0;
        }

        public static string Show(DateTime d1, DateTime d2)
        {
            int[] array = TimeHelper.toResult(d1, d2, TimeHelper.DiffResultFormat.yyMMddHHmm);
            string result = "";
            if (array != null && array.Length > 0)
            {
                if (array.Length > 0 && array[0] > 0)
                {
                    result += string.Format("{0}年", array[0]);
                }
                else if (array.Length > 1 && array[1] > 0)
                {
                    result += string.Format("{0}月", array[1]);
                }
                else if (array.Length > 2 && array[2] > 0)
                {
                    result += string.Format("{0}天", array[2]);
                }
                else if (array.Length > 3 && array[3] > 0)
                {
                    result += string.Format("{0}小时", array[3]);
                }
                else if (array.Length > 4 && array[4] > 0)
                {
                    result += string.Format("{0}分钟", array[4]);
                }
                result += "前";
            }
            return result;
        }
        /// <summary>
        /// 关于返回值形式的枚举
        /// </summary>
        public enum DiffResultFormat
        {
            /// <summary>
            /// 年数和月数
            /// </summary>
            yymm,
            /// <summary>
            /// 年数
            /// </summary>
            yy,
            /// <summary>
            /// 月数
            /// </summary>
            mm,
            /// <summary>
            /// 天数
            /// </summary>
            dd,
            /// <summary>
            /// 年数-月数-天数-小时-分钟
            /// </summary>
            yyMMddHHmm,
        }
        #endregion

        /// <summary>  
        /// 某日期是本月的第几周  
        /// </summary>  
        /// <param name="dtSel"></param>  
        /// <param name="sundayStart"></param>  
        /// <returns></returns>  
        public static int WeekOfMonth(DateTime dtSel, bool sundayStart)
        {
            //如果要判断的日期为1号，则肯定是第一周了   
            if (dtSel.Day == 1) return 1;
            else
            {
                //得到本月第一天   
                DateTime dtStart = new DateTime
                (dtSel.Year, dtSel.Month, 1);
                //得到本月第一天是周几   
                int dayofweek = (int)dtStart.DayOfWeek;
                //如果不是以周日开始，需要重新计算一下
                //dayofweek，详细风DayOfWeek枚举的定义   
                if (!sundayStart)
                {
                    dayofweek = dayofweek - 1;
                    if (dayofweek < 0) dayofweek = 7;
                }
                //得到本月的第一周一共有几天   
                int startWeekDays = 7 - dayofweek;
                //如果要判断的日期在第一周范围内，返回1   
                if (dtSel.Day <= startWeekDays) return 1;
                else
                {
                    int aday = dtSel.Day + 7 - startWeekDays;
                    return aday / 7 + (aday % 7 > 0 ? 1 : 0);
                }
            }
        }

        /// <summary>   
        /// 判断两个日期是否在同一周   
        /// </summary>   
        /// <param name="dtmS">开始日期</param>   
        /// <param name="dtmE">结束日期</param>  
        /// <returns></returns>   
        public static bool IsInSameWeek(DateTime dtmS, DateTime dtmE)
        {
            TimeSpan ts = dtmE - dtmS;
            double dbl = ts.TotalDays;
            int intDow = Convert.ToInt32(dtmE.DayOfWeek);
            if (intDow == 0) intDow = 7;
            if (dbl >= 7 || dbl >= intDow) return false;
            else return true;
        } 

    }
}
