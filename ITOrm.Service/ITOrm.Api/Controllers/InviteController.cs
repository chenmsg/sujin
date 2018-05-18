﻿using ITOrm.Host.BLL;
using ITOrm.Host.Models;
using ITOrm.Utility.Cache;
using ITOrm.Utility.Const;
using ITOrm.Utility.ITOrmApi;
using ITOrm.Utility.Message;
using ITOrm.Utility.StringHelper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using ITOrm.Utility.Helper;

namespace ITOrm.Api.Controllers
{
    public class InviteController : Controller
    {
        public static UsersBLL usersDao = new UsersBLL();
        public static SendMsgBLL sendMsgDao = new SendMsgBLL();
        // GET: Invite
        [HttpGet]
        public ActionResult Reg(string u)
        {
            ResultModelData<Users> result = new ResultModelData<Users>();

            string BaseUserId = ITOrm.Utility.Encryption.AESEncrypter.AESDecrypt(u, Constant.SystemAESKey);
           
            if (string.IsNullOrEmpty(BaseUserId))
            {
                result.backState = -100;
                result.message = "参数有误";
            }
            var user = usersDao.Single(Convert.ToInt32(BaseUserId));
            result.Data = user;
            return View(result);
        }


        // GET: Invite
        [HttpPost]
        public ActionResult Reg(string mobile,string mcode,string pwd,string baseUserId, string regGuid)
        {
            ResultModelData<Users> result = new ResultModelData<Users>();


            //密码加密
            pwd = ITOrm.Utility.Encryption.SecurityHelper.GetMD5String(pwd);
            string parms = $"mobile={mobile}&mcode={mcode}&password={pwd}&regGuid={regGuid}&baseUserId={baseUserId}";
            var regResult = ApiRequest.getApiData<JObject>("Users/Register", parms);

            if (regResult.backState == 0)
            {
                return new RedirectResult("/itapi/invite/Prompt");
            }
            else
            {
                result.backState = -100;
                result.message = regResult.message;
            }
            var user = usersDao.Single(baseUserId);
            result.Data = user;
            return View(result);
        }

        [ValidateInput(false)]
        public ActionResult Prompt()
        {
            return View();
        }


        /// <summary>
        /// 生成图形验证码
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetImgCode(int width = 73, int height = 28 ,string guid="")
        {
            if (guid.Length != 36)
            {
                string result = ApiReturnStr.getError(-100, "参数错误");
                return Content(result);
            }
            string key = ITOrm.Utility.Const.Constant.reg_img_code + guid;
            if (MemcachHelper.Exists(key))
            {
                string result = ApiReturnStr.getError(-100, "有效期内一个guid只能请求一次");
                return Content(result);
            }
            int fontsize = 20;
            string code = string.Empty;
            byte[] bytes = ValidateCode.CreateValidateGraphic(out code, 4, width, height, fontsize);
            MemcachHelper.Set(key, code, ITOrm.Utility.Const.Constant.img_code_expires);

            return File(bytes, @"image/jpeg");
        }


        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <returns></returns>
        public string SendMsgCode(int cid = 0, string mobile = "", string vcode = "", string guid = "")
        {
            
            var result = ApiRequest.getApiData<JObject>("Users/SendMsgCode", $"mobile={mobile}&vcode={vcode}&guid={guid}");
            return JsonConvert.SerializeObject(result);

        }


             

    }
}