﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Web.Mvc;
using ITOrm.Utility.ITOrmApi;

using ITOrm.Host.BLL;
using System.Data.SqlClient;
using ITOrm.Utility.Serializer;
using ITOrm.Host.Models;
using ITOrm.Core.Memcached.Impl;
using ITOrm.Utility.StringHelper;
using Memcached.ClientLibrary;
using ITOrm.Core.Logging;
using ITOrm.Utility.Message;
using ITOrm.Utility.Const;
using ITOrm.Utility.Cache;
using ITOrm.Utility.Log;
using ITOrm.Utility.Helper;
using ITOrm.Utility.Client;
using System.IO;
using System.Drawing;

namespace ITOrm.Api.Controllers
{
    public class UploadController : Controller
    {
        public static UserImageBLL userImageDao = new UserImageBLL();
        public static UserEventRecordBLL userEventDao = new UserEventRecordBLL();
        public string UpImg(int cid=0,int UserId=0)
        {

            try
            {
                string base64 = TQuery.GetString("base64");

                byte[] bmpBytes = Convert.FromBase64String(base64);


                //base64 = "/9j/4AAQSkZJRgABAQEAYABgAAD/4QA6RXhpZgAATU0AKgAAAAgAA1EQAAEAAAABAQAAAFERAAQAAAABAAAAAFESAAQAAAABAAAAAAAAAAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDL/2wBDAQkJCQwLDBgNDRgyIRwhMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjL/wAARCAAZACMDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwDuKKWkr6A+JCilpKBiYoo4ooAfSU6kpAJS4opT0oGhtFLRQB//2Q==";
                if (base64.Length < 100)
                {
                    return ApiReturnStr.getError(-100, "图片太小，不能作为照片上传。");
                }
                var fileLength = Convert.ToInt32(base64.Length - (base64.Length / 8) * 2);//文件字节
                if (fileLength >= 1024 * 1024 / 2)
                {
                    return ApiReturnStr.getError(-100, "上传图片大小不能大于512K。");
                }
                string url = Constant.StaticHost+ "Upload/UpImg";
                JObject data = new JObject();
                data["cid"] = cid;
                data["UserId"] = UserId;
                data["dic"] = "users";
                data["base64"] = base64;
                string json = string.Empty;
                int state= HttpHelper.HttpPostJson(url, data.ToString(), System.Text.Encoding.UTF8, out json);
                if (state ==200)
                {
                    reqApiModel<JObject> model = JsonConvert.DeserializeObject<reqApiModel<JObject>>(json);
                    if (model.backState == 0)
                    {
                        UserImage userImage = new UserImage();
                        userImage.CTime = DateTime.Now;
                        userImage.FileName = model.Data["filename"].ToString();
                        userImage.Ip = Ip.GetClientIp();
                        userImage.PlatForm = cid;
                        userImage.State = 0;
                        userImage.Url = model.Data["Url"].ToString();
                        userImage.UserId = UserId;
                        int result = userImageDao.Insert(userImage);
                        model.Data["ID"] = result;
                        userEventDao.UserEventInit(cid, UserId, Ip.GetClientIp(), result > 0 ? 1 : 0, "Upload", "UpImg", $"{{ImgUrl:{model.Data["ImgUrl"]},version:{TQuery.GetString("version")}}}");
                        return ApiReturnStr.getApiData(result > 0 ? 0 : -100, result > 0 ? "上传成功" : "上传失败", model.Data);
                    }
                    
                }
                return ApiReturnStr.getApiData(-100,$"上传失败:httpStatus:{state},message:{json}");
            }
            catch (Exception ex)
            {
                Logs.WriteLog(string.Format("cmd=UpImg,cid:{0},UserId:{1},ip:{2},ex:{3}",cid,UserId,Ip.GetClientIp(),ex.Message) , "d:\\Log\\Upload", "UpImg");
                
                return ApiReturnStr.getError(-100, "上传图片失败，请稍后再试。");
            }
        }
    }
}