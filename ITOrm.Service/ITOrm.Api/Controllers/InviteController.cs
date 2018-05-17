using ITOrm.Host.BLL;
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

namespace ITOrm.Api.Controllers
{
    public class InviteController : Controller
    {
        public static UsersBLL usersDao = new UsersBLL();
        public static SendMsgBLL sendMsgDao = new SendMsgBLL();
        // GET: Invite
        public ActionResult Reg(string u)
        {
            string BaseUserId = ITOrm.Utility.Encryption.AESEncrypter.AESDecrypt(u, Constant.SystemAESKey);
            if (string.IsNullOrEmpty(BaseUserId))
            {
                return Content("参数有误");
            }
            var user = usersDao.Single(Convert.ToInt32(BaseUserId));
            return View(user);
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
            #region 验证
            if (!ITOrm.Utility.StringHelper.TypeParse.IsMobile(mobile))
            {
                return ApiReturnStr.getError(-100, "手机号格式验证失败");
            }
            if (guid.Length != 36)
            {
                return ApiReturnStr.getError(-100, "唯一标识错误");
            }
            if (vcode.Trim().Length != 4)
            {
                return ApiReturnStr.getError(-100, "验证码错误");
            }

            string imgKey = ITOrm.Utility.Const.Constant.reg_img_code + guid;

            if (!ITOrm.Utility.Cache.MemcachHelper.Exists(imgKey))
            {
                return ApiReturnStr.getError(-101, "图形验证码过期");
            }
            string cacheImgCode = ITOrm.Utility.Cache.MemcachHelper.Get(imgKey).ToString();

            if (vcode.Trim().ToLower() != cacheImgCode.ToLower())
            {
                return ApiReturnStr.getError(-100, "图形验证码错误");
            }
            var modelUsers = usersDao.Single(" mobile=@mobile ", new { mobile });
            if (modelUsers != null && modelUsers.UserId > 0)
            {
                return ApiReturnStr.getError(-100, "该手机号已注册");
            }
            if (sendMsgDao.ValidateRegisterCnt(mobile))
            {
                return ApiReturnStr.getError(-100, "验证码发送次数超限");
            }
            #endregion

            var regGuid = ITOrm.Utility.StringHelper.Util.GetGUID;



            //发送短信
            var resultMsg = SystemSendMsg.Send(Logic.EnumSendMsg.注册短信, mobile);

            SendMsg model = new SendMsg();
            model.TypeId = (int)Logic.EnumSendMsg.注册短信;
            model.Context = resultMsg.content;
            model.CTime = DateTime.Now;
            model.IP = ITOrm.Utility.Client.Ip.GetClientIp();
            model.Merchant = resultMsg.Merchant;
            model.Mobile = mobile;
            model.Platform = cid;
            model.Service = "reg";
            model.RelationId = resultMsg.relationId;
            model.State = resultMsg.backState ? 2 : 1;
            model.UTime = DateTime.Now;
            int result = sendMsgDao.Insert(model);

            if (resultMsg.backState && result > 0)
            {
                #region 销毁
                ITOrm.Utility.Cache.MemcachHelper.Delete(imgKey);
                #endregion

                string key = Constant.reg_mobile_code + regGuid;
                var cacheData = new JObject();
                cacheData["mobile"] = mobile;
                cacheData["code"] = resultMsg.code;
                MemcachHelper.Set(key, cacheData.ToString(), ITOrm.Utility.Const.Constant.mobile_code_expires);


                var data = new JObject();
                data["regGuid"] = regGuid;
                if (Constant.IsDebug)
                {
                    data["code"] = resultMsg.code;
                }
                return ApiReturnStr.getApiData(0, "发送成功", data);
            }

            return ApiReturnStr.getApiData(-100, "发送失败");


        }

    }
}