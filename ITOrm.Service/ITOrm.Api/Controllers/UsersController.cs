using System;
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
using ITOrm.Payment.Yeepay;
using static ITOrm.Payment.Yeepay.Enums;
using ITOrm.Payment.Masget;
namespace ITOrm.Api.Controllers
{
    public class UsersController : Controller
    {
        public static UsersBLL userDao = new UsersBLL();
        public static SendMsgBLL sendMsgDao = new SendMsgBLL();
        public static AccountBLL accountDao = new AccountBLL();
        public static UserEventRecordBLL userEventDao = new UserEventRecordBLL();
        public static UserBankCardBLL userBankCardDao = new UserBankCardBLL();
        public static UserImageBLL userImageDao = new UserImageBLL();
        public static PayRecordBLL payRecordDao = new PayRecordBLL();
        public static WithDrawBLL withDrawDao = new WithDrawBLL();
        public static YeepayUserBLL yeepayUserDao = new YeepayUserBLL();
        public static BankBLL bankDao = new BankBLL();
        public static KeyValueBLL keyValueDao = new KeyValueBLL();
        public static MasgetUserBLL masgetUserDao = new MasgetUserBLL();
        public static BankTreatyApplyBLL bankTreatyApplyDao = new BankTreatyApplyBLL();
        #region 注册模块



        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="mobile">手机号</param>
        /// <param name="password">密码</param>
        /// <param name="mcode">手机验证码</param>
        /// <param name="regGuid">令牌</param>
        /// <param name="baseUserId">推荐人</param>
        /// <returns></returns>
        public string Register(int cid = 0, string mobile = "", string password = "", string mcode = "", string regGuid = "", int baseUserId = 0)
        {
            Logs.WriteLog($"Register,cid:{cid},mobile:{mobile},password:{password},mcode:{mcode},regGuid:{regGuid},baseUserId:{baseUserId}", "d:\\Log\\ITOrm", "Register");
            #region 验证
            if (!TypeParse.IsMobile(mobile))
            {
                return ApiReturnStr.getError(-100, "手机号格式验证失败");
            }
            if (password.Length != 32)
            {
                return ApiReturnStr.getError(-100, "密码格式错误");
            }
            if (mcode.Length != 6)
            {
                return ApiReturnStr.getError(-100, "手机验证码格式错误");
            }
            if (regGuid.Length != 36)
            {
                return ApiReturnStr.getError(-100, "短信令牌格式错误");
            }

            string key = ITOrm.Utility.Const.Constant.reg_mobile_code + regGuid;
            if (!ITOrm.Utility.Cache.MemcachHelper.Exists(key))
            {
                return ApiReturnStr.getError(-100, "短信验证码已过期");
            }

            JObject mobileCodeData = JObject.Parse(ITOrm.Utility.Cache.MemcachHelper.Get(key).ToString());
            if (mobileCodeData["code"].ToString() != mcode)
            {
                return ApiReturnStr.getError(-100, "短信验证码错误");
            }
            if (mobileCodeData["mobile"].ToString() != mobile)
            {
                return ApiReturnStr.getError(-100, "手机号码不是接收短信的手机号码");
            }

            if (baseUserId > 0)
            {
                var baseUser = userDao.Single(baseUserId);
                if (baseUser == null || baseUser.UserId == 0)
                {
                    return ApiReturnStr.getError(-100, "该邀请人不存在");
                }
            }
            var modelUsers = userDao.Single(" mobile=@mobile ", new { mobile });
            if (modelUsers != null && modelUsers.UserId > 0)
            {
                return ApiReturnStr.getError(-100, "该手机号已注册");
            }
            #endregion

            var model = new Users();
            model.BaseUserId = baseUserId;
            model.CTime = DateTime.Now;
            model.Email = "";
            model.IdCard = "";
            model.IP = ITOrm.Utility.Client.Ip.GetClientIp();
            model.IsRealState = 0;
            model.Mobile = mobile;
            model.Password = password;
            model.PlatForm = cid;
            model.RealName = "";
            model.Soure = "";
            model.State = 0;
            model.UserName = mobile;
            model.UTime = DateTime.Now;
            model.RealTime = DateTime.Now;
            model.VipType = (int)Logic.VipType.SVip用户;
            var result = userDao.Insert(model);
            var account = new Account();
            account.UserId = result;
            account.CTime = DateTime.Now;
            account.UTime = DateTime.Now;
            account.Frozen = 0m;
            account.Available = 0m;
            account.Total = 0m;
            var resultAccount = accountDao.Insert(account);
            if (result > 0 && resultAccount > 0)
            {
                JObject obj = new JObject();
                obj["UserId"] = result;
                userEventDao.UserRegister(cid, Ip.GetClientIp(), result, 1, mobile, password, mcode, regGuid, baseUserId, TQuery.GetString("version"));
                ITOrm.Utility.Cache.MemcachHelper.Delete(key);//销毁本次验证码缓存
                return ApiReturnStr.getApiData(0, "注册成功", obj);
            }
            else
            {
                return ApiReturnStr.getError(-100, "注册失败");
            }
        }

        /// <summary>
        /// 检查手机号是否重复
        /// </summary>
        /// <returns></returns>
        public string CheckMobile(string mobile = "")
        {
            if (!ITOrm.Utility.StringHelper.TypeParse.IsMobile(mobile))
            {
                return ApiReturnStr.getError(-100, "手机号格式验证失败");
            }
            var model = userDao.Single(" mobile=@mobile ", new { mobile });
            if (model != null && model.UserId > 0)
            {
                return ApiReturnStr.getError(-100, "该手机号已注册");
            }
            else
            {
                return ApiReturnStr.getError(0, "用户不存在");
            }
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
            var modelUsers = userDao.Single(" mobile=@mobile ", new { mobile });
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
            var resultMsg = SystemSendMsg.Send(Logic.EnumSendMsg.注册短信,mobile);

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


        /// <summary>
        /// 检测邀请人
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public string CheckBaseUserIdByMobile(string mobile = "")
        {
            if (!ITOrm.Utility.StringHelper.TypeParse.IsMobile(mobile))
            {
                return ApiReturnStr.getError(-100, "邀请人手机号格式验证失败");
            }

            var model = userDao.Single(" mobile=@mobile ", new { mobile });
            if (model != null && model.UserId > 0 && model.IsRealState == 0)
            {
                return ApiReturnStr.getError(-1, "该邀请人还未通过实名认证");
            }
            if (model != null && model.UserId > 0)
            {
                JObject data = new JObject();
                data["mobile"] = mobile;
                data["RealName"] = Util.GetHiddenString(model.RealName, 0, model.RealName.Length - 1);
                data["UserId"] = model.UserId;
                return ApiReturnStr.getApiData(data);
            }
            else
            {
                return ApiReturnStr.getError(-1, "邀请码错误");
            }

        }


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
            string key = ITOrm.Utility.Const.Constant.reg_img_code + guid;
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


        #endregion

        #region 修改密码

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="UserId"></param>
        /// <param name="oldPwd"></param>
        /// <param name="newPwd"></param>
        /// <returns></returns>
        public string UpdatePassword(int cid = 0, int UserId = 0, string oldPwd = "", string newPwd = "")
        {

            Logs.WriteLog($"Action:User,Cmd:UpdatePassword,UserId:{UserId},oldPwd{oldPwd},newPwd:{oldPwd}", "d:\\Log\\ITOrm", "UserUpdatePassword");
            if (oldPwd.Length != 32 || newPwd.Length != 32 || UserId <= 0)
            {
                return ApiReturnStr.getError(-100, "参数错误");
            }

            Users model = userDao.Single(UserId);
            if (model == null || model.UserId < 0)
            {
                return ApiReturnStr.getError(-100, "用户不存在");
            }
            if (model.Password != oldPwd)
            {
                return ApiReturnStr.getError(-100, "旧密码与原密码不一致");
            }
            if (oldPwd == newPwd)
            {
                return ApiReturnStr.getError(-100, "旧密码与新密码一致");
            }
            model.Password = newPwd;
            model.UTime = DateTime.Now; ;
            bool flag = userDao.Update(model);
            userEventDao.UserUpdatePassword(cid, UserId, Ip.GetClientIp(), oldPwd, newPwd, flag ? 1 : 0,TQuery.GetString("version"));//事件日志
            Logs.WriteLog($"Action:User,Cmd:UpdatePassword,UserId:{UserId},oldPwd{oldPwd},newPwd:{newPwd},State:{flag}", "d:\\Log\\ITOrm", "UserUpdatePassword");
            return ApiReturnStr.getError(flag ? 0 : -100, flag ? "修改成功" : "修改失败");

        }


        #endregion

        #region 登录
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="mobile">手机号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public string Login(int cid = 0, string mobile = "", string password = "",string guid="")
        {

            if (!TypeParse.IsMobile(mobile))
            {
                return ApiReturnStr.getError(-100, "请输入正确的手机号");
            }
            if (password.Length != 32)
            {
                return ApiReturnStr.getError(-100, "密码格式不正确");
            }
            if (guid.Length != 36)
            {
                return ApiReturnStr.getError(-100, "唯一标识错误");
            }
            var model = userDao.Single("mobile=@mobile ", new { mobile });

            if (model != null && model.UserId > 0 && model.State < 0)
            {
                return ApiReturnStr.getError(-100, "您的账户被冻结，无法登录");
            }
            if (model != null && model.UserId > 0)
            {
                var result = userEventDao.UserCheckLogin(model.UserId);//检查用户是否可以登录
                if (!result.backState)
                {
                    return ApiReturnStr.getError(-100, result.msg);
                }

            }

            if (model != null && model.UserId > 0 && model.Password == password)
            {
                JObject data = new JObject();
                data["UserId"] = model.UserId;
                //记录登录状态
                ITOrm.Utility.Cache.MemcachHelper.Set(Constant.login_key+model.UserId,guid,DateTime.Now.AddYears(1));
                userEventDao.UserLogin(cid, mobile, password, Ip.GetClientIp(), model.UserId, 1, TQuery.GetString("version"),guid);//登录成功的日志
                return ApiReturnStr.getApiData(0, "登录成功", data);
            }

            if (model != null && model.UserId > 0)
            {
                userEventDao.UserLogin(cid, mobile, password, Ip.GetClientIp(), model.UserId, 0,TQuery.GetString("version"), guid);//登录失败的日志
                return ApiReturnStr.getError(-100, "用户名或密码错误(登录失败)");
            }
            return ApiReturnStr.getError(-100, "用户名或密码错误");

        }

        /// <summary>
        /// 检测设备
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="UserId"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public string CheckDevice(int cid=0,int UserId=0,string guid="")
        {
 
            if (guid.Length != 36)
            {
                return ApiReturnStr.getError(-100, "唯一标识错误");
            }

            if (UserId == 0 || Constant.IsDebug)
            {
                return ApiReturnStr.getError(0,$"UserId={UserId}或IsDebug={Constant.IsDebug}");
            }

            if (MemcachHelper.Get(Constant.login_key + UserId) != null)
            {
               
                var uuidCach = MemcachHelper.Get(Constant.login_key + UserId).ToString();
                if (guid == uuidCach)
                {
                    return ApiReturnStr.getError(0, "同设备");
                }
                else
                {
                    return ApiReturnStr.getError(-100, "您的账号已经在其它设备上登录，如果这不是您的操作，您的密码可能已经泄露，请及时处理");
                }
            }
            else
            {
                return ApiReturnStr.getError(0, "允许通行");
            }
        }
        #endregion

        #region 用户中心

        #region 查询个人信息
        /// <summary>
        /// 查询个人信息
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public string Single(int cid = 0, int UserId = 0)
        {
            if (UserId <= 0)
            {
                return ApiReturnStr.getError(-100, "参数错误");
            }

            JObject data = new JObject();
            var user = userDao.Single(UserId);


            if (user == null || user.UserId <= 0)
            {
                return ApiReturnStr.getError(-100, "用户不存在");
            }
            data["Mobile"] = ITOrm.Utility.StringHelper.Util.GetHiddenString(user.Mobile, 3, 4);
            data["IsRealState"] = user.IsRealState;
            data["IsRealStateText"] = user.IsRealState == 0 ? "未认证" : "已认证";
            data["RealName"] = user.RealName;
            data["IdCard"] = Util.GetHiddenString(user.IdCard, 6, 4);
            data["VipType"] = user.VipType;
            data["VipTypeTxt"] = ((Logic.VipType)user.VipType).ToString();
            data["AvatarImg"] = ITOrm.Utility.Const.Constant.CurrentApiHost+ userImageDao.GetUrl(user.AvatarImg);
            Logic.VipType vip = (Logic.VipType)user.VipType;
            decimal[] r = Constant.GetRate(0, vip);
            decimal[] r2 = Constant.GetRate(1, vip);

            data["Rate1"] =r[0].perCent(); 
            data["Rate3"] = r[1].ToString("F1");
            data["NoneRate1"] = r2[0].perCent();
            data["NoneRate3"] = r2[1].ToString("F1"); 


            var ubk = userBankCardDao.Single(" UserId=@UserId and TypeId=0 and state=1  ", new { UserId });
            data["BankCard"] = "";
            data["BankName"] = "";
            data["BankCode"] = "";
            if (ubk != null)
            {
                data["BankCard"] = ubk.BankCard;
                data["BankName"] = ubk.BankName;
                data["BankCode"] = ubk.BankCode;
            }

            data["BaseUserName"] = "--";
            if (user.BaseUserId > 0)
            {
                var baseUser = userDao.Single(user.BaseUserId);
                data["BaseUserName"] = baseUser.RealName;
            }
            return ApiReturnStr.getApiData(data);
        }
        #endregion

        #region 设置头像

        public string AvatarImg(int cid=0,int UserId=0,int ImgId=0)
        {
            if (cid == 0 || UserId == 0 || ImgId == 0)
            {
                return ApiReturnStr.getError(-100,"参数有误");
            }
            var user = userDao.Single(UserId);
            user.AvatarImg = ImgId;
            user.UTime = DateTime.Now;
            bool flag = userDao.Update(user);
            bool flag2= userImageDao.UpdateState(ImgId, 1);
            userEventDao.UserEventInit(cid, UserId, Ip.GetClientIp(), flag && flag2 ? 1 : 0, "Users", "AvatarImg", $"{{ImgId:{ImgId},flag:{flag},flag2:{flag2}}}");
            return ApiReturnStr.getError(flag && flag2 ? 0 : -100, flag && flag2 ? "设置成功" : "设置失败");
        }
        #endregion

        #region 查询支付卡

        public string GetBankCardList(int pageIndex = 1, int pageSize = 10, int UserId = 0)
        {


            int total = 0;

            var listUbk = userBankCardDao.GetPaged(pageSize, pageIndex, out total, " UserId=@UserId and TypeId=1  ", new { UserId }, " order by ID desc ");




            JArray list = new JArray();
            if (listUbk != null && listUbk.Count > 0)
            {
                foreach (var item in listUbk)
                {
                    JObject obj = new JObject();
                    obj["ID"] = item.ID;
                    obj["BankCard"] = item.BankCard;
                    obj["Mobile"] = item.Mobile;
                    obj["BankName"] = item.BankName;
                    obj["BankCode"] = item.BankCode;
                    obj["CVN2"] = item.CVN2;
                    obj["ExpiresYear"] = item.ExpiresYear;
                    obj["ExpiresMouth"] = item.ExpiresMouth;
                    obj["OpeningBank"] = item.OpeningBank;
                    obj["OpeningSerialBank"] = item.OpeningSerialBank;
                    obj["CTime"] = item.CTime.ToString("yyyy-MM-dd HH:mm:ss");
                    obj["State"] = item.State;
                   
                    list.Add(obj);
                }
            }
            return ApiReturnStr.getApiDataListByPage(list, total, pageIndex, pageSize);

        }

        #endregion

        #region 获得银行卡列表


        public string GetBankList()
        {
            //Logs.WriteLog($"1111", "d:\\Log\\", "GetBankList");
            List<Bank> listBank = MemcachHelper.Get<List<Bank>>(Constant.list_bank_key , DateTime.Now.AddHours(1), () =>
            {
                return bankDao.GetQuery(" State<>-1 "); 
            });

            JArray list = new JArray();
            if (listBank != null && listBank.Count > 0)
            {
                foreach (var item in listBank)
                {
                    JObject obj = new JObject();
                    obj["BankName"] = item.BankName;
                    obj["BankCode"] = item.BankCode;
                    obj["State"] = item.State;
                    obj["StateTxt"] = item.State==0?"可用":"不可用";
                    list.Add(obj);
                }
            }
            return ApiReturnStr.getApiDataList(list);
        }
        #endregion

        #region 绑定银行卡
        public string BankBind(int cid = 0, int UserId = 0, string mobile = "", string bankcard = "", string bankcode = "", int typeid = 0, string cvn2 = "", string expiresYear = "", string expiresMouth = "", string OpeningBank = "", string OpeningSerialBank = "", int BankID = 0)
        {
            Logs.WriteLog($"Action:User,Cmd:BankBind,UserId:{UserId},mobile：{mobile},bankcard:{bankcard},bankcode:{bankcode},typeid:{typeid},cvn2:{cvn2},expiresYear:{expiresYear},expiresMouth:{expiresMouth},OpeningBank:{OpeningBank},OpeningSerialBank:{OpeningSerialBank}", "d:\\Log\\ITOrm", "BankBind");
            userEventDao.UserBankBind(cid, UserId, Ip.GetClientIp(), mobile, bankcard, bankcode, typeid, cvn2, expiresYear, expiresMouth, OpeningBank, OpeningSerialBank, BankID);

            var version = TQuery.GetString("version");
            if (cid == 3 && version == "1.0.0")
            {
                string temp = expiresYear;
                expiresYear = expiresMouth;
                expiresMouth = temp;
            }
            #region 验证
            if (!TypeParse.IsMobile(mobile))
            {
                return ApiReturnStr.getError(-100, "预留手机号格式验证失败");
            }
            if (!(bankcard.Length > 13 && bankcard.Length < 21))
            {
                return ApiReturnStr.getError(-100, "银行卡参数错误");
            }
            if (string.IsNullOrEmpty(bankcode))
            {
                return ApiReturnStr.getError(-100, "银行编号不能为空");
            }
            Users user = userDao.Single(UserId);
            if (user == null || user.UserId <= 0)
            {
                return ApiReturnStr.getError(-100, "用户不存在");
            }
            if (user.IsRealState != 1)
            {
                return ApiReturnStr.getError(-100, "用户未实名认证，无法绑卡");
            }
            if (typeid == 1)//如果是结算卡
            {
                if (string.IsNullOrEmpty(cvn2) || string.IsNullOrEmpty(expiresYear) || string.IsNullOrEmpty(expiresMouth))
                {
                    return ApiReturnStr.getError(-100, "支付卡参数有误");
                }

                if (BankID == 0)
                { 
                    UserBankCard ubc = userBankCardDao.Single(" UserId=@UserId and BankCard=@bankcard and TypeId=1  ", new { UserId, bankcard });
                    if (ubc != null && ubc.ID > 0)
                    {
                        return ApiReturnStr.getError(-100, "该支付卡已经绑定过，不能重复绑定");
                    }
                }
            }
            else
            {
                UserBankCard ubc = userBankCardDao.Single(" UserId=@UserId and TypeId=0 and  State=1 ", new { UserId });
                if (ubc != null && ubc.ID > 0)
                {
                    return ApiReturnStr.getApiData(-100, "结算卡只能绑定一张");
                }
            }
            UserBankCard model = null;
            if (BankID > 0)
            {
                model = userBankCardDao.Single(BankID);
                if (model == null)
                {
                    return ApiReturnStr.getApiData(-100, "BankID记录不存在");
                }
                if (model.State == 1)
                {
                    return ApiReturnStr.getApiData(-100, "该银行卡已通过验证，不可修改信息");
                }
            }
            else
            {
                model = new UserBankCard();
            }
            #endregion

            #region 绑卡
            var result = BankCardBindHelper.Bind(typeid, user.RealName, user.IdCard, bankcard, mobile, cvn2, expiresYear, expiresMouth);

            model.BankCard = result.bankCard;
            model.BankCode = bankcode;
            model.BankName = bankDao.QueryBankName(bankcode);
            model.CVN2 = cvn2;
            model.ExpiresYear = expiresYear;
            model.ExpiresMouth = expiresMouth;
            model.UTime = DateTime.Now;
            model.Mobile = mobile;
            model.TypeId = typeid;
            model.OpeningBank = OpeningBank;
            model.OpeningSerialBank = OpeningSerialBank;
            model.State = 0;// result.backState ? 1 : 0;//默认为0
            model.RelationId = ",0,";
            bool flag = false;
            if (BankID == 0)
            {
                model.UserId = UserId;
                model.IP = Ip.GetClientIp();
                model.CTime = DateTime.Now;
                model.Platform = cid;
                int num = userBankCardDao.Insert(model);
                flag = num > 0;
                return ApiReturnStr.getError(flag ? 0 : -100, flag ? "绑定成功" : "绑定失败");
            }
            else
            {
                flag = userBankCardDao.Update(model);
                return ApiReturnStr.getError(flag ? 0 : -100, flag ? "修改成功" : "修改失败");
            }

            #endregion
        }

        #endregion

        #region 修改银行卡预留手机号
        public string UpdateBankMobile(int cid = 0, int UserId = 0, int BankID = 0, string mobile = "")
        {
            userEventDao.UserEventInit(cid, UserId, Ip.GetClientIp(), 0, "Users", "UpdateBankMobile", $"{{BankID:{BankID},mobile:{mobile},version:{TQuery.GetString("version")}}}");
            #region 验证
            if (!TypeParse.IsMobile(mobile))
            {
                return ApiReturnStr.getError(-100, "预留手机号格式验证失败");
            }
            var model = userBankCardDao.Single(BankID);
            if (model == null)
            {
                return ApiReturnStr.getApiData(-100, "BankID记录不存在");
            }
            if (model.UserId != UserId)
            {
                return ApiReturnStr.getApiData(-100, "该银行卡不属于此账户下");
            }
            #endregion
            model.Mobile = mobile;
            model.UTime = DateTime.Now;
            bool flag = userBankCardDao.Update(model);
            return ApiReturnStr.getError(flag ? 0 : -100, flag ? "修改成功" : "修改失败");
        }
        #endregion

        #region 请求激活银行卡快捷验证
        public string BankCardActivate(int cid = 0, int UserId = 0, int BankID = 0, int ChannelType = 0)
        {
            userEventDao.BankCardActivate(cid, UserId, Ip.GetClientIp(), 0, TQuery.GetString("version"), BankID, ChannelType);
            Logic.ChannelType ct = (Logic.ChannelType)ChannelType;
            var ubk = userBankCardDao.Single(BankID);

            if (ubk == null)
            {
                return ApiReturnStr.getError(-100, "银行卡不存在");
            }
            if (ubk.UserId != UserId)
            {
                return ApiReturnStr.getError(-100, "此卡不属于该用户");
            }
            switch (ct)
            {
                case Logic.ChannelType.易宝:
                    return ApiReturnStr.getError(-100, "易宝通道无需激活");
                case Logic.ChannelType.荣邦科技积分:
                case Logic.ChannelType.荣邦科技无积分:
                case Logic.ChannelType.荣邦3:
                    //进件
                    if (!masgetUserDao.QueryIsExist(UserId, ChannelType))
                    {
                        var resultSubcompany = MasgetDepository.SubcompanyAdd(UserId, cid, ct);
                        if (resultSubcompany.backState != 0)
                        {
                            return ApiReturnStr.getError(-100, $"开户失败({resultSubcompany.message},ct={ChannelType})");
                        }
                    }
                    //入驻
                    if (!masgetUserDao.QueryIsOpen(UserId, ChannelType))
                    {
                        var resultSamenameOpen = MasgetDepository.SamenameOpen(UserId, cid, ct);
                        if (resultSamenameOpen.backState != 0)
                        {
                            return ApiReturnStr.getError(-100, $"入驻失败({resultSamenameOpen.message},ct={ChannelType})");
                        }
                    }
                    if (bankTreatyApplyDao.QueryTreatycodeIsOpen(BankID, ChannelType))
                    {
                        return ApiReturnStr.getError(-100, "此通道已开通快捷协议");
                    }
                    //发送验证码
                    var resultTreatyApply = MasgetDepository.TreatyApply(BankID, cid, ct);
                    if (resultTreatyApply.backState != 0)
                    {
                        return ApiReturnStr.getError(-100, $"申请开通快捷协议失败({resultTreatyApply.message})");
                    }
                    else
                    {
                        return ApiReturnStr.getError(0, "验证码发送成功");
                    }
                default:
                    break;
            }
            return ApiReturnStr.getError(-100, "参数错误");
        }

        #endregion

        #region 确认开通快捷协议
        public string BankCardSubmitActivateCode(int cid = 0, int UserId = 0, int BankID = 0, int ChannelType = 0,string Code="")
        {

            var result= MasgetDepository.TreatyConfirm(BankID, Code,cid, (Logic.ChannelType)ChannelType);
            userEventDao.BankCardSubmitActivateCode(cid, UserId, Ip.GetClientIp(), result.backState==0?1:0, TQuery.GetString("version"), BankID, ChannelType, Code);
            return ApiReturnStr.getError(result.backState == 0 ? 0 : -100, result.message);
        }
        #endregion

        #region 查询支付记录
        public string QueryPayRecordList(int cid = 0, int UserId = 0, int pageIndex = 1, int pageSize = 10, int State = 0)
        {
            #region 验证参数
            if (UserId <= 0)
            {
                return ApiReturnStr.getError(-100, "用户ID不能为0");
            }
            if (pageIndex < 1)
            {
                return ApiReturnStr.getError(-100, "页码不能小于1");
            }
            if (pageSize < 1)
            {
                return ApiReturnStr.getError(-100, "每页数量不能小于1");
            }
            #endregion

            int totalCount = 0;
            object param = null;
            string where = "UserId=@UserId ";
            if (State != -200)
            {
                where += " and State=@State ";
                param = new { UserId, State };
            }
            else
            {
                where += " and State<>0 ";
                param = new { UserId };
            }
            var listpay = payRecordDao.GetPaged(pageSize, pageIndex, out totalCount, where, param, "order by CTime desc");

            JArray list = new JArray();
            if (listpay != null && listpay.Count > 0)
            {
                foreach (var item in listpay)
                {
                    //收款记录相关
                    JObject obj = new JObject();
                    obj["OrderNo"] = item.ID.ToString();
                    obj["Amount"] = item.Amount.ToString("F2");


                    obj["Fee"] = item.Fee.ToString("F2");
                    obj["Rate"] = item.Rate.perCent();
                    obj["CTime"] = item.CTime.ToString("yyyy-MM-dd HH:mm:ss");
                    obj["PayTime"] = item.State == 10 ? item.PayTime.ToString("yyyy-MM-dd HH:mm:ss") : "--";
                    obj["PayState"] = item.State;
                    obj["PayStateTxt"] = ((PayRecordState)item.State).ToString();
                    obj["Message"] = item.State == 10 ? "" : item.Message;
                    obj["BankCode"] = item.BankCode;
                    obj["BankCard"] = Util.GetHiddenString(item.BankCard, 6, 4);

                    //结算记录相关
                    obj["WithDrawAmount"] = item.WithDrawAmount.ToString("F2");
                    obj["ActualAmount"] = item.ActualAmount.ToString("F2");
                    obj["Fee3"] = item.Fee3.ToString("F2");
                    obj["DrawBankCard"] = item.DrawBankCard;
                    obj["DrawState"] = item.DrawState;
                    obj["DrawStateTxt"] = ((WithDrawState)item.DrawState).ToString();
                    obj["HandleTime"] = "--";
                    if ((WithDrawState)item.DrawState == WithDrawState.打款成功)
                    {
                        obj["HandleTime"] = item.HandleTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    list.Add(obj);
                }
            }
            return ApiReturnStr.getApiDataListByPage(list, totalCount, pageIndex, pageSize);
        }
        #endregion

        #region 查询结算记录 未使用
        public string QueryWithDrawList(int cid = 0, int UserId = 0, int pageIndex = 1, int pageSize = 10, int State = 0)
        {
            #region 验证参数
            if (UserId <= 0)
            {
                return ApiReturnStr.getError(-100, "用户ID不能为0");
            }
            if (pageIndex < 1)
            {
                return ApiReturnStr.getError(-100, "页码不能小于1");
            }
            if (pageSize < 1)
            {
                return ApiReturnStr.getError(-100, "每页数量不能小于1");
            }
            #endregion

            int totalCount = 0;
            object param = null;
            string where = "UserId=@UserId ";
            if (State != -200)
            {
                where += " and State=@State ";
                param = new { UserId, State };
            }
            else
            {
                where += " and State<>0 ";
                param = new { UserId };
            }
            var listpay = withDrawDao.GetPaged(pageSize, pageIndex, out totalCount, where, param, "order by CTime desc");

            JArray list = new JArray();
            if (listpay != null && listpay.Count > 0)
            {
                foreach (var item in listpay)
                {
                    JObject obj = new JObject();
                    obj["Amount"] = item.Amount.ToString("F2");
                    obj["Receiver"] = item.Receiver;
                    obj["Fee"] = item.Fee.ToString("F2");
                    obj["BasicFee"] = item.BasicFee.ToString("F2");
                    obj["ActualAmount"] = item.ActualAmount.ToString("F2");
                    obj["ReceiverBankCardNo"] = Util.GetHiddenString(item.ReceiverBankCardNo, 6, 4);
                    obj["ReceiverBank"] = item.ReceiverBank;
                    obj["CTime"] = item.CTime.ToString("yyyy-MM-dd HH:mm:ss");
                    obj["HandleTime"] = item.State != 0 ? item.HandleTime.ToString("yyyy-MM-dd HH:mm:ss") : "";
                    obj["State"] = ((WithDrawState)item.State).ToString();
                    obj["Message"] = item.State < 0 ? "" : item.Message;
                    list.Add(obj);
                }
            }
            return ApiReturnStr.getApiDataListByPage(list, totalCount, pageIndex, pageSize);
        }

        #endregion


        #endregion

        public string demo()
        {
            
            return ITOrm.Utility.StringHelper.Util.GetGUID;
        }
    }
}