using System;
using Newtonsoft.Json.Linq;
using System.Web.Mvc;
using ITOrm.Utility.ITOrmApi;

using ITOrm.Host.BLL;
using ITOrm.Host.Models;
using ITOrm.Utility.StringHelper;
using ITOrm.Utility.Cache;
using ITOrm.Utility.Const;
using ITOrm.Utility.Log;
using ITOrm.Utility.Message;
using ITOrm.Utility.Client;
namespace ITOrm.Api.Controllers
{


    public class ForgetController : Controller
    {
        public static UsersBLL userDao = new UsersBLL();
        public static SendMsgBLL sendMsgDao = new SendMsgBLL();
        public static UserEventRecordBLL userEventDao = new UserEventRecordBLL();
        // GET: Forget
        /// <summary>
        /// 生成图形验证码
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetImgCode(int width = 73, int height = 28, string guid = "")
        {
            if (guid.Length != 36)
            {
                string result = ApiReturnStr.getError(-100, "参数错误");
                return Content(result);
            }
            string key = ITOrm.Utility.Const.Constant.forget_img_code + guid;
            if (MemcachHelper.Exists(key))
            {
                string result = ApiReturnStr.getError(-100, "有效期内一个guid只能请求一次");
                return Content(result);
            }
            int fontsize = 20;
            string code = string.Empty;
            byte[] bytes = ValidateCode.CreateValidateGraphic(out code, 4, width, height, fontsize);
            ITOrm.Utility.Cache.MemcachHelper.Set(key, code, ITOrm.Utility.Const.Constant.img_code_expires);
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

            string imgKey = ITOrm.Utility.Const.Constant.forget_img_code + guid;

            if (!ITOrm.Utility.Cache.MemcachHelper.Exists(imgKey))
            {
                return ApiReturnStr.getError(-101, "图形验证码过期");
            }
            string cacheImgCode = ITOrm.Utility.Cache.MemcachHelper.Get(imgKey).ToString();

            if (vcode.Trim().ToLower() != cacheImgCode.ToLower())
            {
                return ApiReturnStr.getError(-100, "图形验证码错误");
            }
            var modelUsers = userDao.Single(" mobile=@mobile ", new { mobile });
            if (modelUsers == null || modelUsers.UserId == 0)
            {
                return ApiReturnStr.getError(-100, "该手机号未注册");
            }
            if (sendMsgDao.ValidateForgetCnt(mobile))
            {
                return ApiReturnStr.getError(-100, "验证码发送次数超限");
            }
            #endregion

            var regGuid = Util.GetGUID;

            //发送短信
            var resultMsg = SystemSendMsg.Send(Logic.EnumSendMsg.忘记密码短信,mobile);

            SendMsg model = new SendMsg();
            model.Context = resultMsg.content;
            model.CTime = DateTime.Now;
            model.TypeId =(int) Logic.EnumSendMsg.忘记密码短信;
            model.IP = ITOrm.Utility.Client.Ip.GetClientIp();
            model.Merchant = resultMsg.Merchant;
            model.Mobile = mobile;
            model.Platform = cid;
            model.Service = "forget";
            model.RelationId = resultMsg.relationId;
            model.State = resultMsg.backState ? 2 : 1;
            model.UTime = DateTime.Now;
            int result = sendMsgDao.Insert(model);

            if (resultMsg.backState && result > 0)
            {
                #region 销毁
                ITOrm.Utility.Cache.MemcachHelper.Delete(imgKey);
                #endregion

                string key = Constant.forget_mobile_code + regGuid;
                var cacheData = new JObject();
                cacheData["mobile"] = mobile;
                cacheData["code"] = resultMsg.code;
                MemcachHelper.Set(key, cacheData.ToString(), ITOrm.Utility.Const.Constant.mobile_code_expires);


                var data = new JObject();
                data["forgetGuid"] = regGuid;
                if (Constant.IsDebug)
                {
                    data["code"] = resultMsg.code;
                }
                return ApiReturnStr.getApiData(0, "发送成功", data);
            }

            return ApiReturnStr.getApiData(-100, "发送失败");
            
        }


        /// <summary>
        /// 验证短信验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="regGuid"></param>
        /// <param name="mcode"></param>
        /// <returns></returns>
        public string ValidateMobileCode(string mobile="", string forgetGuid = "",string mcode="")
        {
            #region 验证
            if (!ITOrm.Utility.StringHelper.TypeParse.IsMobile(mobile))
            {
                return ApiReturnStr.getError(-100, "手机号格式验证失败");
            }
            if (forgetGuid.Length != 36)
            {
                return ApiReturnStr.getError(-100, "短信令牌有误");
            }
            if (mcode.Trim().Length != 6)
            {
                return ApiReturnStr.getError(-100, "手机验证码错误");
            }

            string mobileKey = ITOrm.Utility.Const.Constant.forget_mobile_code + forgetGuid;

            if (!ITOrm.Utility.Cache.MemcachHelper.Exists(mobileKey))
            {
                return ApiReturnStr.getError(-100, "手机验证码已过期");
            }
            JObject cacheMobileCode =JObject.Parse(  ITOrm.Utility.Cache.MemcachHelper.Get(mobileKey).ToString());

            if (mobile != cacheMobileCode["mobile"].ToString())
            {
                return ApiReturnStr.getError(-100, "当前手机号不是接收短信的手机号");
            }

            if (mcode.Trim() != cacheMobileCode["code"].ToString())
            {
                return ApiReturnStr.getError(-100, "手机验证码错误");
            }



            #endregion

            JObject data = new JObject();
            string guid = Util.GetGUID;
            data["forgetGuid"] = guid;
            data["mobile"] = mobile;
            string key = Constant.forget_token + guid;
            MemcachHelper.Delete(mobileKey);//销毁手机短信验证码
            MemcachHelper.Set(key, data.ToString(), ITOrm.Utility.Const.Constant.token_expires);
            return ApiReturnStr.getApiData(0, "验证成功", data);
        }


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="forgetGuid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string UpdatePassword(int cid=0,string forgetGuid ="",string password="")
        {
            #region 验证
            if (forgetGuid.Length != 36)
            {
                return ApiReturnStr.getError(-100, "验证令牌有误");
            }
            if (password.Length != 32)
            {
                return ApiReturnStr.getError(-100, "密码格式错误");
            }
            string key = Constant.forget_token + forgetGuid;
            if (!MemcachHelper.Exists(key))
            {
                return ApiReturnStr.getError(-100, "验证令牌过期，请重试！");
            }
            JObject obj= JObject.Parse(MemcachHelper.Get(key).ToString());
            string mobile = obj["mobile"].ToString();
            Users model = userDao.Single(" mobile= @mobile ", new { mobile });
            if (model != null && model.UserId > 0)
            {
                model.Password = password;
                model.UTime = DateTime.Now;
                var flag= userDao.Update(model);
                userEventDao.UserForget(cid,model.UserId, flag ? 1 : 0, Ip.GetClientIp(), key, password,TQuery.GetString("version"));//事件日志
                Logs.WriteLog(string.Format("Action:Forget,Cmd:UpdatePassword,UserId:{0},Mobile{1},forgetGuid:{2},State:{3}", model.UserId,mobile,forgetGuid,flag), "d:\\Log\\ITOrm", "ForgetUpdatePassword");
                MemcachHelper.Delete(key);//销毁令牌
                return ApiReturnStr.getError(flag ? 0 : -100, flag ? "修改成功" : "修改失败");
            }
            #endregion
            Logs.WriteLog(string.Format("Action:Forget,Cmd:UpdatePassword,Mobile{0},forgetGuid:{1},用户不存在", mobile, forgetGuid), "d:\\Log\\ITOrm", "ForgetUpdatePassword");
            return ApiReturnStr.getError(-100,"用户不存在");
        }

        public string GetGuid()
        {
            return Util.GetGUID;
        }

    }
}