using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ITOrm.Utility.Log
{
    public class Logs
    {
        private static readonly object obj = new object();
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="path">日志记录路径 如:E:\\APP\\Log\\WebService</param>
        /// <param name="bllName">业务模块名称 如: LookupProcessor</param>
        public static void WriteLog(string msg, string path, string bllName, LogType logType= LogType.Day)
        {
            try
            {
                path = ITOrm.Utility.Const.Constant.IsDebug ? path.Replace("d:\\Log", "d:\\LogTest") : path;
                Monitor.Enter(obj);
                DateTime dt = DateTime.Now;
                string message = "\r\n" + dt.ToString() + ": " + msg + "\r\n";
                string Ppath = string.Empty;
                string AName = string.Empty;
                switch (logType)
                {
                    case LogType.Day:
                        Ppath= string.Format("{0}\\{1}\\{2}", path, bllName, dt.ToString("yyyy-MM"));
                        AName= string.Format("{0}\\{1}.log", Ppath, dt.ToString("yyyy-MM-dd"));
                        break;
                    case LogType.Hour:
                        Ppath = string.Format("{0}\\{1}\\{2}", path, bllName, dt.ToString("yyyy-MM-dd"));
                        AName = string.Format("{0}\\{1}.log", Ppath, dt.Hour);
                        break;
                    case LogType.Mouth:
                        break;
                    default:
                        break;
                }
                
                if (!System.IO.Directory.Exists(Ppath))
                    System.IO.Directory.CreateDirectory(Ppath);
            
                using (System.IO.FileStream MyFileStream = new System.IO.FileStream(AName, System.IO.FileMode.Append))
                {
                    byte[] byte_arr = Encoding.Default.GetBytes(message);
                    MyFileStream.Write(byte_arr, 0, byte_arr.Length);
                    MyFileStream.Close();
                }
            }
            catch
            {
                //throw e;
            }
            finally
            {
                Monitor.Pulse(obj);
                Monitor.Exit(obj);
            }
        }

        public enum LogType
        {
            Day,
            Hour,
            Mouth
        }
    }
}
