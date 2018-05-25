using ITOrm.Utility.Client;
using ITOrm.Utility.Const;
using ITOrm.Utility.ITOrmApi;
using ITOrm.Utility.Log;
using ITOrm.Utility.StringHelper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITOrm.Static.Controllers
{
    public class UploadController : Controller
    {
        [HttpPost]
        public string UpImg()
        {
            int cid = 0;
            int UserId =0;
            string dic = "";
            try
            {
                HttpRequestBase request = HttpContext.Request;
                Stream stream = request.InputStream;
                string json = string.Empty;
                string responseJson = string.Empty;
                if (stream.Length != 0)
                {
                    StreamReader streamReader = new StreamReader(stream);
                    json = streamReader.ReadToEnd();

                }
                JObject obj = JObject.Parse(json);
                string base64 = obj["base64"].ToString();
                 cid = obj["cid"].ToInt();
                 UserId = obj["UserId"].ToInt();
                 dic = obj["dic"].ToString();

                //base64 = "/9j/4AAQSkZJRgABAQEAYABgAAD/4QA6RXhpZgAATU0AKgAAAAgAA1EQAAEAAAABAQAAAFERAAQAAAABAAAAAFESAAQAAAABAAAAAAAAAAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDL/2wBDAQkJCQwLDBgNDRgyIRwhMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjL/wAARCAAZACMDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwDuKKWkr6A+JCilpKBiYoo4ooAfSU6kpAJS4opT0oGhtFLRQB//2Q==";
                if (base64.Length < 100)
                {
                    return ApiReturnStr.getError(-100, "图片太小，不能作为照片上传。");
                }
                var fileLength = Convert.ToInt32(base64.Length - (base64.Length / 8) * 2);//文件字节
                if (fileLength >= 1024 * 1024 * 2)
                {
                    return ApiReturnStr.getError(-100, "上传图片大小不能超过2M。");
                }

                string path = "upload/"+ dic + "/" + DateTime.Now.ToString("yyyyMMdd") + "/";
                string dicPath = Server.MapPath("~/" + path);

                if (!Directory.Exists(dicPath))//如果没有文件夹则创建
                {
                    System.IO.Directory.CreateDirectory(dicPath);
                }

                string file_ex = ".jpg";
                string url2 = Constant.StaticHost + path;
                //文件名
                string fileName = DateTime.Now.ToString("HHmmssfff");
                byte[] bmpBytes = Convert.FromBase64String(base64);
                //完整地址
                string aUrl = url2 + fileName + file_ex;

                if (aUrl.Length > 10)
                {
                    MemoryStream ms = new MemoryStream(bmpBytes);
                    Bitmap bmp = new Bitmap(ms);
                    bmp.Save(dicPath + fileName + file_ex, System.Drawing.Imaging.ImageFormat.Jpeg);
                }

                JObject data = new JObject();
                data["ImgUrl"] = aUrl;
                data["fileLength"] = fileLength;
                data["filename"]= fileName + file_ex;
                data["Url"]= path + fileName + file_ex;
                return ApiReturnStr.getApiData(0, "上传成功" , data);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(string.Format("static=UpImg,cid:{0},UserId:{1},ip:{2},ex:{3}", cid, UserId, Ip.GetClientIp(), ex.Message), "d:\\Log\\Upload", "UpImg");
                return ApiReturnStr.getError(-100, "上传图片失败，请稍后再试。");
            }
        }
    }
}