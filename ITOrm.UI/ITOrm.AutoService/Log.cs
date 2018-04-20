using System;
using System.IO;
using System.Text;

namespace ITOrm.AutoService
{
    public class Log
    {
        private StreamWriter swLog;    //成功
        private StreamWriter faLog;    //失败

        private string _groupName;
        public string GroupName
        {
            set { _groupName = value; }
        }


        #region  操作成功日志
        /// <summary>
        /// 执行成功日志
        /// </summary>
        /// <param name="log">日志信息</param>
        public void mLog(string log)
        {
            if (swLog == null)
            {
                OpenSwLog();
            }
            try
            {
                string s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-->" + log;
                Console.WriteLine(s);
                swLog.WriteLine(s);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { CloseSwLog(); }
        }
        /// <summary>
        /// 打开成功日志文件
        /// </summary>
        private void OpenSwLog()
        {
            try
            {
                if (swLog == null)
                {
                    string sPath = Directory.GetCurrentDirectory() + "\\log\\";
                    string sName = DateTime.Now.ToString("yyyyMMdd") + "_" + _groupName + ".txt";

                    if (!Directory.Exists(sPath))
                    {
                        Directory.CreateDirectory(sPath);
                    }

                    swLog = new StreamWriter(sPath + sName, true, Encoding.Default, 1024);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        /// <summary>
        /// 关闭成功日志文件
        /// </summary>
        private void CloseSwLog()
        {
            if (swLog != null)
            {
                swLog.Flush();
                swLog.Close();
                swLog = null;
            }
        }
        #endregion

        #region 操作失败日志
        /// <summary>
        /// 执行失败日志
        /// </summary>
        /// <param name="log"></param>
        public void fLog(string log)
        {
            if (faLog == null)
            {
                OpenFLog();
            }
            try
            {
                string s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-->" + log;
                Console.WriteLine(s);
                faLog.WriteLine(s);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally { CloseFLog(); }
        }
        /// <summary>
        /// 打开失败日志文件
        /// </summary>
        private void OpenFLog()
        {
            try
            {
                if (faLog == null)
                {
                    string sPath = Directory.GetCurrentDirectory() + "\\log\\";
                    string sName = DateTime.Now.ToString("yyyyMMdd") + "_" + _groupName + "failed.txt";

                    if (!Directory.Exists(sPath))
                    {
                        Directory.CreateDirectory(sPath);
                    }

                    faLog = new StreamWriter(sPath + sName, true, Encoding.Default, 1024);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        /// <summary>
        /// 关闭失败日志文件
        /// </summary>
        private void CloseFLog()
        {
            if (faLog != null)
            {
                faLog.Flush();
                faLog.Close();
                faLog = null;
            }
        }
        #endregion
    }
}
