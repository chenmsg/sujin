using System;
using System.Text;
using System.IO;

namespace ITOrm.Core.Utility.Files
{
    public class HtmlHelper
    {
        public HtmlHelper() { }

        private static StreamReader sr;
        private static StreamWriter sw;

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="Coding">文件编码</param>
        /// <returns></returns>
        public static string Reader(string Path, string Coding)
        {
            string str = "";
            if (File.Exists(Path))
            {
                try
                {
                    sr = new StreamReader(Path, Encoding.GetEncoding(Coding));
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }

            return str;
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="Text">写入内容</param>
        /// <param name="Coding">文件编码</param>
        public static void Writer(string Path, string Text, string Coding)
        {
            if (File.Exists(Path))
            {
                Delete(Path);
            }
            try
            {
                sw = new StreamWriter(Path, false, Encoding.GetEncoding(Coding));
                sw.WriteLine(Text);
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        public static void Delete(string Path)
        {
            if (File.Exists(Path))
            {
                try
                {
                    File.Delete(Path);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
        }

        /// <summary>
        /// 删除指定目录下的所有文件
        /// </summary>
        /// <param name="Path">目录路径</param>
        public static void AllDelete(string Path)
        {
            FileInfo[] fi = (new DirectoryInfo(Path)).GetFiles();
            for (int x = 0; x < fi.Length; x++)
            {
                try
                {
                    fi[x].Delete();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
        }

    }
}
