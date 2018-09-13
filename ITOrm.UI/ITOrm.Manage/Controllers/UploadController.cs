using ITOrm.Host.BLL;
using ITOrm.Host.Models;
using ITOrm.Utility.Client;
using ITOrm.Utility.Const;
using ITOrm.Utility.ITOrmApi;
using ITOrm.Utility.Log;
using ITOrm.Utility.StringHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITOrm.Manage.Controllers
{
    public class UploadController : Controller
    {
        public static UserImageBLL userImageDao = new UserImageBLL();
        // GET: Upload
        public string UpImg()
        {
            int cid = 1;
            int UserId = TQuery.GetInt("UserId");
            try
            {
                if (Request.Files.Count == 0) return "";
                var files = Request.Files[0];


                byte[] content = new byte[files.InputStream.Length];
                files.InputStream.Read(content, 0, Convert.ToInt32(files.InputStream.Length));


                string base64 = Convert.ToBase64String(content.ToArray()); ;

                byte[] bmpBytes = Convert.FromBase64String(base64);
                //base64 = "/9j/4AAQSkZJRgABAQEAYABgAAD/4QA6RXhpZgAATU0AKgAAAAgAA1EQAAEAAAABAQAAAFERAAQAAAABAAAAAFESAAQAAAABAAAAAAAAAAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDL/2wBDAQkJCQwLDBgNDRgyIRwhMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjL/wAARCAAZACMDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwDuKKWkr6A+JCilpKBiYoo4ooAfSU6kpAJS4opT0oGhtFLRQB//2Q==";
               
                string url = Constant.StaticHost + "Upload/UpImg";
                JObject data = new JObject();
                data["cid"] = cid;
                data["UserId"] = UserId;
                data["dic"] = "banner";
                data["base64"] = base64;
                string json = string.Empty;
                int state = HttpHelper.HttpPostJson(url, data.ToString(), System.Text.Encoding.UTF8, out json);
                if (state == 200)
                {
                    reqApiModel<JObject> model = JsonConvert.DeserializeObject<reqApiModel<JObject>>(json);
                    UserImage img = new UserImage();
                    img.UserId = UserId;
                    img.Url = model.Data["Url"].ToString();
                    img.PlatForm = (int)ITOrm.Utility.Const.Logic.Platform.系统;
                    int ImgId= userImageDao.Insert(img);
                    model.Data["ImgId"] = ImgId;
                    return ApiReturnStr.getApiData(model.backState,model.message, model.Data);
                }
                return ApiReturnStr.getApiData(-100, $"上传失败:httpStatus:{state},message:{json}");
            }
            catch (Exception ex)
            {
                return ApiReturnStr.getError(-100, "上传图片失败，请稍后再试。err:"+ex.Message);
            }
        }
    }
}