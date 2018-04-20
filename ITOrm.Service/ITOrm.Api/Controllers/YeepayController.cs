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
using ITOrm.Payment.Const;
using ITOrm.Payment.Teng;
namespace ITOrm.Api.Controllers
{
    public class YeepayController : Controller
    {

        public static UsersBLL userDao = new UsersBLL();
        public static SendMsgBLL sendMsgDao = new SendMsgBLL();
        public static AccountBLL accountDao = new AccountBLL();
        public static UserEventRecordBLL userEventDao = new UserEventRecordBLL();
        public static UserBankCardBLL userBankCardDao = new UserBankCardBLL();
        public static UserImageBLL userImageDao = new UserImageBLL();
        public static AreaCodeBLL areaCodeDao = new AreaCodeBLL();
        public static YeepayUserBLL yeepayUserDao = new YeepayUserBLL();
        public static BankQuotaBLL bankQuotaDao = new BankQuotaBLL();
        public static MasgetUserBLL masgetUserDao = new MasgetUserBLL();
        public static BankTreatyApplyBLL bankTreatyApplyDao = new BankTreatyApplyBLL();
        public static KeyValueBLL keyValueDao = new KeyValueBLL();
        public static YeepayLogBLL yeepayLogDao = new YeepayLogBLL();
        public readonly static string bankList = "工商银行、农业银行、招商银行、建设银行、交通银行、中信银行、光大银行、北京银行、平安银行、中国银行、兴业银行、民生银行、华夏银行、广发银行、浦发银行";
        public readonly static string[] bankLists = bankList.Split('、');


        #region 获得地区码

        public string GetAreaCode(int BaseId = 0)
        {
            List<AreaCode> listArea = MemcachHelper.Get<List<AreaCode>>(Constant.list_area_key + BaseId, DateTime.Now.AddDays(7), () =>
            {
                return areaCodeDao.GetQuery("BaseId=@BaseId", new { BaseId }, " order by sort asc ");
            });

            JArray list = new JArray();
            if (listArea != null && listArea.Count > 0)
            {
                foreach (var item in listArea)
                {
                    JObject obj = new JObject();
                    obj["Id"] = item.ID;
                    obj["Code"] = item.Code;
                    obj["Name"] = item.Name;
                    list.Add(obj);
                }
            }

            return ApiReturnStr.getApiDataList(list);
        }

        #endregion

        #region 获得收款银行列表

        public string GetBankList()
        {
            JArray list = new JArray();
            foreach (var item in bankLists)
            {
                list.Add(item);
            }
            return ApiReturnStr.getApiDataList(list);
        }

        #endregion

        #region 子商户注册
        /// <summary>
        /// 子商户注册
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="UserId"></param>
        /// <param name="IdCard">身份证号码</param>
        /// <param name="RealName">真实姓名</param>
        /// <param name="BankName">银行名称</param>
        /// <param name="BankAccountNumber">银行卡号码</param>
        /// <param name="AreaCode">地区编码</param>
        /// <param name="BankCardPhoto">银行卡照片</param>
        /// <param name="IdCardPhoto">身份证照片</param>
        /// <param name="IdCardBackPhoto">身份证背面照片</param>
        /// <param name="PersonPhoto">三合一照片</param>
        /// <returns></returns>
        public string Register(int cid = 0, int UserId = 0, string IdCard = "", string RealName = "", string BankName = "", string BankAccountNumber = "", string AreaCode = "", int BankCardPhoto = 0, int IdCardPhoto = 0, int IdCardBackPhoto = 0, int PersonPhoto = 0)
        {
            //if (cid == 2)
            //{
            //    return ApiReturnStr.getError(0,"开户成功");
            //}
            Logs.WriteLog($"Register,cid:{cid},UserId:{UserId},IdCard:{IdCard},RealName:{RealName},BankName:{BankName},BankAccountNumber:{BankAccountNumber},AreaCode:{AreaCode},BankCardPhoto:{BankCardPhoto},IdCardPhoto:{IdCardPhoto},IdCardBackPhoto:{IdCardBackPhoto},PersonPhoto:{PersonPhoto}", "d:\\Log\\Yeepay", "Register");
            #region 参数验证
            if (UserId <= 0)
            {
                return ApiReturnStr.getError(-100, "UserId参数错误");
            }
            if (!RegexHelper.IsMatch(RealName, @"^\s*[\u4e00-\u9fa5]{1,}[\u4e00-\u9fa5.·]{0,15}[\u4e00-\u9fa5]{1,}\s*$"))
            {
                return ApiReturnStr.getError(-100, "请输入真实的姓名！");
            }

            if (!TypeParse.IsIdentity(IdCard))
            {
                return ApiReturnStr.getError(-100, "身份证号格式错误！");
            }
            if (!TypeParse.IsChinese(BankName))
            {
                return ApiReturnStr.getError(-100, "银行卡开户行有误");
            }
            if (string.IsNullOrEmpty(BankAccountNumber) || !(BankAccountNumber.Length > 13 && BankAccountNumber.Length < 21))
            {
                return ApiReturnStr.getError(-100, "银行卡卡号有误");
            }
            if (!BankCardBindHelper.ValidateBank(BankName, BankAccountNumber))
            {
                return ApiReturnStr.getError(-100, "银行卡卡bin识别失败");
            }
            if (string.IsNullOrEmpty(AreaCode) || AreaCode.Length != 4) return ApiReturnStr.getError(-100, "请选择地区码");
            if (BankCardPhoto == 0) return ApiReturnStr.getError(-100, "银行卡正面照未上传");
            if (IdCardPhoto == 0) return ApiReturnStr.getError(-100, "身份证正面照未上传");
            if (IdCardBackPhoto == 0) return ApiReturnStr.getError(-100, "身份证背面照未上传");
            if (PersonPhoto == 0) PersonPhoto= IdCardPhoto;//取消手持三合一

            var user = userDao.Single(" IdCard=@IdCard and RealName=@RealName and IsRealState=1", new { IdCard, RealName, });
            if (user != null && user.UserId > 0)
            {
                return ApiReturnStr.getError(-100, "该身份证号码已注册");
            }

            var model = userDao.Single(UserId);
            if (model == null || model.UserId <= 0)
            {
                return ApiReturnStr.getError(-100, "用户不存在");
            }

            var cnt = yeepayLogDao.Count(string.Format("typeId={0} and UserId={1} and DateDiff(dd,CTime,getdate())=0 ", (int)YeepayType.子商户注册, UserId));
            if (cnt > 4)
            {
                return ApiReturnStr.getError(-100, "当日开户次数超过5次，请次日再操作！");
            }
            #endregion

            #region 组装子商户报文实体
            reqRegisterModel yeepayRegModel = new reqRegisterModel();
            yeepayRegModel.signedName = RealName;
            yeepayRegModel.idCard = IdCard;
            yeepayRegModel.bankAccountNumber = BankAccountNumber;
            yeepayRegModel.bankName = BankName.ConvertBank();//转换易宝可识别的银行
            yeepayRegModel.areaCode = AreaCode;

            #endregion
            //易宝子商户注册
            var result = YeepayDepository.Register(yeepayRegModel, UserId, cid, BankCardPhoto, IdCardPhoto, IdCardBackPhoto, PersonPhoto);
            //事件日志
            userEventDao.RealNameAuthentication(cid, UserId, Ip.GetClientIp(), IdCard, RealName, result.backState == 0 ? 1 : 0,TQuery.GetString("version"));
            return ApiReturnStr.getError(result.backState, result.backState == 0 ? "开户成功" : result.message);
        }
        #endregion

        #region 提前计算实际到卡金额

        public string PayFeeTool(int UserId = 0, decimal Amount = 0M,int PayType=0)
        {
            
            if (UserId <= 0)
            {
                return ApiReturnStr.getError(-100, "用户ID不能为0");
            }
            if (Amount < 100M)
            {
                return ApiReturnStr.getError(-100, "支付金额不能小于100");
            }
            ToolPay model = null;
            Users user = userDao.Single(UserId);
            Logic.VipType vip = (Logic.VipType)user.VipType;

            if (PayType == 0)
            {
                switch (vip)
                {
                    case Logic.VipType.顶级用户:
                        model = new ToolPay(Amount, 0.0041M, 0, 1, 0, 0);
                        break;
                    case Logic.VipType.普通用户:
                        model = new ToolPay(Amount, 0.0050M, 0, 2, 0, 0);
                        break;
                    case Logic.VipType.Vip用户:
                        model = new ToolPay(Amount, 0.0043M, 0, 2, 0, 0);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (vip)
                {
                    case Logic.VipType.顶级用户:
                        model = new ToolPay(Amount, 0.0030M, 0, 0.5M, 0, 0);
                        break;
                    case Logic.VipType.普通用户:
                        model = new ToolPay(Amount, 0.0047M, 0, 2, 0, 0);
                        break;
                    case Logic.VipType.Vip用户:
                        model = new ToolPay(Amount, 0.0039M, 0, 2, 0, 0);
                        break;
                    default:
                        break;
                }

            }

            //Logic.ChannelType ct = (Logic.ChannelType)ChannelType;
            //switch (ct)
            //{
            //    case Logic.ChannelType.易宝:
            //        YeepayUser yUser = yeepayUserDao.Single(" UserId=@UserId", new { UserId });
            //        if (yUser == null)
            //        {
            //            return ApiReturnStr.getError(-100, "未开通商户");
            //        }
            //        if (yUser.RateState1 == 0 || yUser.RateState3 == 0)//|| yUser.RateState4 == 0 || yUser.RateState5 == 0
            //        {
            //            return ApiReturnStr.getError(-100, "费率未设置");
            //        }
            //        model= new ToolPay(Amount, yUser.Rate1, 0, yUser.Rate3, 0, 0);
            //        break;
            //    case Logic.ChannelType.荣邦科技积分:
            //    case Logic.ChannelType.荣邦科技无积分:
            //        MasgetUser mUser = masgetUserDao.Single(" TypeId=@ChannelType and UserId=@UserId ",new { ChannelType, UserId });
            //        if (mUser == null) return ApiReturnStr.getError(-100, "商户未进件");
            //        if (mUser.State == 0) return ApiReturnStr.getError(-100, "商户未入驻");
            //        model = new ToolPay(Amount, mUser.Rate1, 0, mUser.Rate3, 0, 0);
            //        break;
            //    default:
            //        break;
            //}


            JObject data = new JObject();
            data["Amount"] = model.Amount.ToString("F2");
            data["Rate1"] = model.Rate1.perCent();
            data["Rate2"] = model.Rate2.ToString("F2");
            data["Rate3"] = model.Rate3.ToString("F2");
            data["Rate4"] = model.Rate4.ToString("F2");
            data["Rate5"] = model.Rate5.ToString("F2");
            data["PayFee"] = model.PayFee.ToString("F2");
            data["BasicFee"] = model.BasicFee.ToString("F2");
            data["ExTargetFee"] = model.ExTargetFee.ToString("F2");
            data["ActualAmount"] = model.ActualAmount.ToString("F2");
            return ApiReturnStr.getApiData(data);
        }

        #endregion

        #region 获取支付卡限额表

        public string GetPayBankQuotaList()
        {
            List<BankQuota> listBank = MemcachHelper.Get<List<BankQuota>>(Constant.list_bank_quota_key, DateTime.Now.AddDays(7), () =>
           {
               return bankQuotaDao.GetQuery("1=1");
           });

            JArray list = new JArray();
            if (listBank != null && listBank.Count > 0)
            {
                foreach (var item in listBank)
                {
                    JObject obj = new JObject();
                    obj["Id"] = item.ID;
      
                    obj["SingleQuota"] = item.SingleQuota.ToString("F0");
                    obj["DayQuota"] = item.DayQuota.ToString("F0");
                    obj["MouthQuota"] = item.MouthQuota.ToString("F0");
                    list.Add(obj);
                }
            }
            return ApiReturnStr.getApiDataList(list);
        }

        #endregion

        #region 收款接口

        //初始版 透传卡号
        public string ReceiveApi(int cid = 0, int UserId = 0, decimal Amount = 0m, string BankCard = "")
        {
            return ApiReturnStr.getError(-100,"请使用最新版APP");
            Logs.WriteLog($"ReceiveApi,cid:{cid},UserId:{UserId},Amount:{Amount},BankCard:{BankCard}", "d:\\Log\\Yeepay", "ReceiveApi");
            #region 参数验证
            if (UserId <= 0) return ApiReturnStr.getError(-100, "UserId参数错误");
            if (Amount < 100) return ApiReturnStr.getError(-100, "收款金额不能小于100元");
            if (string.IsNullOrEmpty(BankCard) || !(BankCard.Length >13 && BankCard.Length<21))
                return ApiReturnStr.getError(-100, "收款银行卡有误");
            var model = userDao.Single(UserId);
            if (model == null || model.UserId <= 0)
            {
                return ApiReturnStr.getError(-100, "用户不存在");
            }
            var yeepayUser = yeepayUserDao.Single(" UserId=@UserId ", new { UserId });
            if (yeepayUser == null || model.UserId <= 0)
            {
                return ApiReturnStr.getError(-100, "未开通子商户功能");
            }
            if (yeepayUser.RateState1 == 0 || yeepayUser.RateState3 == 0 || yeepayUser.RateState4 == 0 || yeepayUser.RateState5 == 0)
            {
                return ApiReturnStr.getError(-100, "费率未设置");
            }
            if (yeepayUser.IsAudit == 0)
            {
                return ApiReturnStr.getError(-100, "子商户未审核通过");
            }
            #endregion
            var result = YeepayDepository.ReceiveApi(UserId, Amount, cid, BankCard);
            if (result.backState == 0)
            {
                JObject data = new JObject();
                data["PayUrl"] = result.urlAES;
                return ApiReturnStr.getApiData(data);
            }
            return ApiReturnStr.getError(-100, result.message);
        }

        //1.0.0
        public string ReceiveApi2(int cid = 0, int UserId = 0, decimal Amount = 0m, int BankID =0,int PayType=0)
        {
            Logs.WriteLog($"ReceiveApi,cid:{cid},UserId:{UserId},Amount:{Amount},BankID:{BankID},PayType:{PayType}", "d:\\Log\\Yeepay", "ReceiveApi2");
            userEventDao.UserReceiveApi2(cid, UserId, Ip.GetClientIp(), 0, TQuery.GetString("version"), Amount, BankID, PayType);
            #region 参数验证
            if (UserId <= 0) return ApiReturnStr.getError(-100, "UserId参数错误");
            if (Amount < 500) return ApiReturnStr.getError(-100, "收款金额不能小于500元");
            var ubk = userBankCardDao.Single(BankID);
            if (ubk == null) return ApiReturnStr.getError(-100, "卡记录不存在");

            //卡数据验证
            var validateBankResult = userBankCardDao.ValidateBank(ubk);
            if (validateBankResult.backState != 0) return ApiReturnStr.getError(-100, validateBankResult.message);

            var model = userDao.Single(UserId);
            if (model == null || model.UserId <= 0) return ApiReturnStr.getError(-100, "用户不存在");

            JObject data = new JObject();
            data["PayUrl"] = "";

            string msg = "";



            //int ChannelType = 0;
            //data["ChannelType"] = 0;
            //data["BankID"] = BankID;



            int ChannelType = 0;
            //data["ChannelType"] = ChannelType;
            data["BankID"] = BankID;


            var option = SelectOptionChannel.Optimal(Amount, BankID, PayType);
            if (option.backState == 0)
            {
                ChannelType = option.Data;
                data["ChannelType"] = ChannelType;
            }
            else
            {
                return ApiReturnStr.getError(-100, option.message);
            }
            data["ChannelType"] = option.Data;

            Logic.ChannelType ct = (Logic.ChannelType)ChannelType;
            switch (ct)
            {
                case Logic.ChannelType.易宝:
                    #region 易宝逻辑
                    var yeepayUser = yeepayUserDao.Single(" UserId=@UserId ", new { UserId });
                    if (yeepayUser == null || model.UserId <= 0)
                    {
                        return ApiReturnStr.getError(-100, "未开通子商户功能");
                    }
                    if (yeepayUser.RateState1 == 0 || yeepayUser.RateState3 == 0 || yeepayUser.RateState4 == 0 || yeepayUser.RateState5 == 0)
                    {
                        return ApiReturnStr.getError(-100, "费率未设置");
                    }
                    if (yeepayUser.IsAudit == 0)
                    {
                        return ApiReturnStr.getError(-100, "子商户未审核通过");
                    }
                    var result = YeepayDepository.ReceiveApi(UserId, Amount, cid, BankID);
                    if (result.backState == 0)
                    {
                        data["PayUrl"] = result.urlAES;
                        return ApiReturnStr.getApiData(data);
                    }
                    msg = result.message;
                    #endregion
                    break;
                case Logic.ChannelType.荣邦科技积分:
                case Logic.ChannelType.荣邦科技无积分:
                    if (ct == Logic.ChannelType.荣邦科技无积分)
                    {
                        return ApiReturnStr.getError(-100, "通道升级中，敬请期待");
                    }
                    #region 荣邦逻辑
                    #region 验证
                    if (Amount < 500)
                    {
                        return ApiReturnStr.getError(-100, "此通道要求支付金额不得小于500元！");
                    }
                    //验证是否开户
                    if (!masgetUserDao.QueryIsExist(UserId, ChannelType)) return ApiReturnStr.getApiData(-200, $"快捷协议未开通(01-{ChannelType})", data);//通道未开户
                    if (!masgetUserDao.QueryIsOpen(UserId, ChannelType)) return ApiReturnStr.getApiData(-200, $"快捷协议未开通(02-{ChannelType})", data);//通道未入驻
                    //验证快捷协议是否开通
                    if (!bankTreatyApplyDao.QueryTreatycodeIsOpen(BankID, ChannelType)) return ApiReturnStr.getApiData(-200, $"快捷协议未开通(03-{ChannelType})", data);
                    #endregion
                    //执行请求
                    var resultBackPay = MasgetDepository.BackPay(BankID, Amount, cid, ct);
                    if (resultBackPay.backState == 0)
                    {
                        data["PayUrl"] = resultBackPay.url;
                        return ApiReturnStr.getApiData(0, "请求成功，待确认支付", data);
                    }
                    else if (resultBackPay.backState == 8401)
                    {
                        return ApiReturnStr.getError(-8401, "通道暂无额度");
                    }
                    else
                    {
                        return ApiReturnStr.getError(-100, resultBackPay.message);
                    }
                #endregion
                case Logic.ChannelType.腾付通:
                    var resultTeng = TengDepository.CreatePayCashier(UserId, cid, Amount, BankID);
                    msg = resultTeng.message;
                    if (resultTeng.backState == 0)
                    {
                        data["PayUrl"] = resultTeng.Data["url"];
                        return ApiReturnStr.getApiData(data);
                    }
                    break;
                default:
                    break;
            }
            
            #endregion
            
            return ApiReturnStr.getError(-100, msg);
        }


        #endregion


    }
}