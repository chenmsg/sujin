using ITOrm.Core.Helper;
using System;
using System.Collections;
using System.Net;
using System.Text.RegularExpressions;

namespace ITOrm.Core.Utility.Helper
{
    public class PhotoHelper
    {
        /// <summary>
        /// 获取内容里面的第一张图片
        /// </summary>
        /// <param name="content"></param>
        /// <param name="defaultImgSrc">不存在图片，返回默认图片</param>
        /// <returns></returns>
        public static string GetContentFirstPhoto(string content)
        {
            content = content != null ? content.ToLower() : null;
            if (content != null && content != "" && content.IndexOf("<img") > -1)
            {
                //这里是获取数组中第一个图片地址，当然也可以获取文章中其他图片，只需修改索引号。  
                return GetImgUrl(content, @"<img[^>]+src=\s*(?:'(?<src>[^']+)'|""(?<src>[^""]+)""|(?<src>[^>\s]+))\s*[^>]*>", "src")[0].ToString();
            }
            return null;
        }

        /// <summary>  
        /// 获取文章中图片地址的方法  
        /// </summary>  
        /// <param name="html">文章内容</param>  
        /// <param name="regstr">正则表达式</param>  
        /// <param name="keyname">关键属性名</param>  
        /// <returns></returns>  
        public static ArrayList GetImgUrl(string html, string regstr, string keyname)
        {
            ArrayList resultStr = new ArrayList();
            Regex r = new Regex(regstr, RegexOptions.IgnoreCase);
            MatchCollection mc = r.Matches(html);

            foreach (Match m in mc)
            {
                resultStr.Add(m.Groups[keyname].Value.ToLower());
            }
            if (resultStr.Count > 0)
            {
                return resultStr;
            }
            else
            {
                //没有地址的时候返回空字符  
                resultStr.Add("");
                return resultStr;
            }
        }



        ///fileUrl:远程文件路径，包括IP地址以及详细的路径
        public static bool RemoteFileExists(string fileUrl)
        {
            bool result = false;//下载结果
            WebResponse response = null;
            try
            {
                WebRequest req = WebRequest.Create(fileUrl);
                response = req.GetResponse();
                result = response == null ? false : true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return result;
        }

    }
}
