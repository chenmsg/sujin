using ITOrm.Core.Helper;
using ITOrm.Utility.Const;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ITOrm.Host.BLL;
using ITOrm.Host.Models;
using Newtonsoft.Json;
using ITOrm.Utility.Log;
using ITOrm.Utility.Client;
using static ITOrm.Payment.Yeepay.Enums;

namespace ITOrm.Payment.Yeepay
{
    public class YeepayDepository
    {
        public static string YeepayDomain = ConfigHelper.GetAppSettings("YeepayDomain");
        public static string YeepayMainCustomerNumber = ConfigHelper.GetAppSettings("YeepayMainCustomerNumber");
        public static string YeepayHmacKey = ConfigHelper.GetAppSettings("YeepayHmacKey");
        public static string YeepayNoticeUrl = ConfigHelper.GetAppSettings("YeepayNoticeUrl");
        public static string YeepayLogDic = "d:\\Log\\Yeepay";//日志文件夹地址
        public static YeepayLogBLL yeepayLogDao = new YeepayLogBLL();
        public static YeepayLogParasBLL yeepayLogParasDao = new YeepayLogParasBLL();
        public static YeepayUserBLL yeepayUserDao = new YeepayUserBLL();
        public static UsersBLL usersDao = new UsersBLL();
        public static UserBankCardBLL userBankCardDao = new UserBankCardBLL();
        public static PayRecordBLL payRecordDao = new PayRecordBLL();
        public static WithDrawBLL withDrawDao = new WithDrawBLL();
        public static UserImageBLL userImageDao = new UserImageBLL();
        public static BankBLL bankDao = new BankBLL();

        #region 子商户注册 register
        public static respRegisterModel Register(reqRegisterModel model, int UserId, int Platform, int BankCardPhoto, int IdCardPhoto, int IdCardBackPhoto, int PersonPhoto)
        {
            string LogDic = "子商户注册";
            Users user = usersDao.Single(UserId);
            int num = 0;
            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Yeepay.Enums.YeepayType.子商户注册, UserId, Platform);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId}", YeepayLogDic, LogDic);
            model.mailStr = "";
            model.customerType = "PERSON";
            model.businessLicence = "";
            model.minSettleAmount = "100";
            model.riskReserveDay = "0";
            model.bankAccountType = "PrivateCash";
            model.bindMobile = user.Mobile;
            model.linkMan = model.signedName;//推荐人姓名(用来标记商户的推荐人，如无可与签约名相同)
            model.legalPerson = model.signedName;
            model.requestId = requestId.ToString();
            //model.bankAccountNumber = m.bankAccountNumber;//银行卡号
            //model.bankName = "";//银行名称
            model.accountName = model.signedName;//
            //model.areaCode = "";//
            model.manualSettle = "Y";
            //model.bankCardPhoto = "";
            model.businessLicensePhoto = "";//个体工商户正面照
            //model.idCardPhoto = "";//身份证正面照
            //model.idCardBackPhoto = "";//身份证背面照
            //model.personPhoto = "";//身份证+银行卡+本人合照
            model.electronicAgreement = "";//电子协议

            model.bankCardPhoto = userImageDao.GetUrlAndUpdateState(BankCardPhoto, 0);
            model.idCardPhoto = userImageDao.GetUrlAndUpdateState(IdCardPhoto, 0);
            model.idCardBackPhoto = userImageDao.GetUrlAndUpdateState(IdCardBackPhoto, 0);
            model.personPhoto = userImageDao.GetUrlAndUpdateState(PersonPhoto, 0); //三合一照片

            StringBuilder sb = new StringBuilder();
            sb.Append(model.mainCustomerNumber);
            sb.Append(model.requestId);
            sb.Append(model.customerType);
            sb.Append(model.businessLicence);
            sb.Append(model.bindMobile);
            sb.Append(model.signedName);
            sb.Append(model.linkMan);
            sb.Append(model.idCard);
            sb.Append(model.legalPerson);
            sb.Append(model.minSettleAmount);
            sb.Append(model.riskReserveDay);
            sb.Append(model.bankAccountNumber);
            sb.Append(model.bankName);
            sb.Append(model.accountName);
            sb.Append(model.areaCode);
            sb.Append(model.manualSettle);
            string sn = sb.ToString();
            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            NameValueCollection objNVC = new NameValueCollection();
            PropertyInfo[] pis = typeof(reqRegisterModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()) && keyName.ToString() != "bankCardPhoto" && keyName.ToString() != "idCardPhoto" && keyName.ToString() != "idCardBackPhoto" && keyName.ToString() != "personPhoto")
                {
                    objNVC.Add(pi.Name, pi.GetValue(model, null).ToString());
                }
            }

            string action = YeepayDomain + "register.action";
            //action = "http://api.itorm.com/itapi/server/register";
            string result = "";
            //请求前日志记录
            Logs.WriteLog("提交参数：" + JsonConvert.SerializeObject(model), YeepayLogDic, LogDic);
            yeepayLogParasDao.Init(requestId, JsonConvert.SerializeObject(model), 0);
            string[] filePath = new string[] { Constant.StaticDic + model.bankCardPhoto, Constant.StaticDic + model.idCardPhoto, Constant.StaticDic + model.idCardBackPhoto, Constant.StaticDic + model.personPhoto };
            string[] fileKeyName = new string[] { "bankCardPhoto", "idCardPhoto", "idCardBackPhoto", "personPhoto" };
            //执行请求
            int responseState = ITOrm.Utility.Client.HttpHelper.HttpPostData(action, Encoding.UTF8, filePath, fileKeyName, objNVC, out result);
            //返回后日志记录
            Logs.WriteLog("返回参数：" + result, YeepayLogDic, LogDic);
            yeepayLogParasDao.Init(requestId, result, 1);

            respRegisterModel resp = JsonConvert.DeserializeObject<respRegisterModel>(result);
            if (resp.backState == 0)
            {
                decimal[] r = Constant.GetRate(0, (Logic.VipType)user.VipType);
                #region 开户成功后的操作
                //初始化易宝用户
                YeepayUser yUser = new YeepayUser();
                yUser.UserId = UserId;
                yUser.Rate1 = r[0];
                yUser.Rate3 = r[1];
                yUser.RateState3 = 1;
                yUser.Rate4 = 0.002M;
                yUser.Rate5 = 0.002M;
                yUser.CTime = DateTime.Now;
                yUser.UTime = DateTime.Now;
                yUser.CustomerNumber = resp.customerNumber;
                num = yeepayUserDao.Insert(yUser);
                Logs.WriteLog($"初始化易宝用户：UserId:{UserId},YeepayUser:{num}", YeepayLogDic, LogDic);
                //设置已实名认证
                user.RealName = model.signedName;
                user.IdCard = model.idCard;
                user.IsRealState = 1;
                user.BankCardPhoto = BankCardPhoto;
                user.IdCardPhoto = IdCardPhoto;
                user.IdCardBackPhoto = IdCardBackPhoto;
                user.PersonPhoto = PersonPhoto;
                user.RealTime = DateTime.Now;
                flag = usersDao.Update(user);
                Logs.WriteLog($"设置已实名认证：UserId:{UserId},修改数据:{flag},RealName:{user.RealName},IdCard:{user.IdCard},IsRealState:{user.IsRealState},BankCardPhoto:{user.BankCardPhoto},IdCardPhoto:{user.IdCardPhoto},IdCardBackPhoto:{user.IdCardBackPhoto}.PersonPhoto:{user.PersonPhoto},", YeepayLogDic, LogDic);
                //设置图片使用状态
                userImageDao.UpdateState(BankCardPhoto, 1);
                userImageDao.UpdateState(IdCardPhoto, 1);
                userImageDao.UpdateState(IdCardBackPhoto, 1);
                userImageDao.UpdateState(PersonPhoto, 1);
                //初始化提现卡
                UserBankCard ubc = new UserBankCard();
                ubc.BankCard = model.bankAccountNumber;
                ubc.BankCode = bankDao.QueryBankCode(model.bankName.BankToFour());//查询BankCode
                ubc.BankName = model.bankName;
                ubc.CTime = DateTime.Now;
                ubc.UTime = DateTime.Now;
                ubc.CVN2 = "";
                ubc.ExpiresYear = "";
                ubc.ExpiresMouth = "";
                ubc.IP = Ip.GetClientIp();
                ubc.Mobile = model.bindMobile;
                ubc.Platform = Platform;
                ubc.TypeId = 0;
                ubc.UserId = UserId;
                ubc.State = 1;
                num = userBankCardDao.Insert(ubc);
                Logs.WriteLog($"初始化提现卡：UserId:{UserId},UserBankCard:{num}", YeepayLogDic, LogDic);
                #endregion

            }
            //易宝日志状态更新
            flag = yeepayLogDao.UpdateState(requestId, resp.code, resp.message, resp.code == "0000" ? 10 : -1);
            Logs.WriteLog($"易宝日志状态更新：requestId:{requestId},code:{resp.code},message:{ resp.message},State:{flag}", YeepayLogDic, LogDic);

            return resp;
        }
        #endregion

        #region 子商户审核 auditMerchant
        public static respAuditMerchantModel AuditMerchant(int UserId, int Platform, AuditMerchant enums, string reason = "")
        {
            string LogDic = "子商户审核";
            YeepayUser yeepayUser = yeepayUserDao.Single(" UserId=@UserId", new { UserId });
            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Yeepay.Enums.YeepayType.子商户审核, UserId, Platform);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId}", YeepayLogDic, LogDic);

            reqAuditMerchantModel model = new reqAuditMerchantModel();
            model.customerNumber = yeepayUser.CustomerNumber;
            model.status = enums.ToString();
            model.reason = string.IsNullOrEmpty(reason) ? "审核拒绝" : reason;

            StringBuilder sb = new StringBuilder();
            sb.Append(model.customerNumber);
            sb.Append(model.mainCustomerNumber);
            sb.Append(model.status);
            sb.Append(model.reason);
            string sn = sb.ToString();
            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqAuditMerchantModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "auditMerchant.action";
            //action = "http://api.itorm.com/itapi/server/auditMerchant";

            //执行请求
            respAuditMerchantModel resp = PostUrl<respAuditMerchantModel>(requestId, action, strPostPara.ToString(), model, LogDic);


            if (resp.backState == 0)
            {
                yeepayUser.IsAudit = (int)enums;
                yeepayUser.UTime = DateTime.Now;
                flag = yeepayUserDao.Update(yeepayUser);
                Logs.WriteLog($"更新审核状态：UserId:{UserId},Platform:{Platform},requestId:{requestId},IsAudit:{yeepayUser.IsAudit},flag:{flag}", YeepayLogDic, LogDic);
            }
            return resp;
        }
        #endregion

        #region 子商户信息查询接口 customerInforQuery
        public static respCustomerInforQueryModel CustomerInforQuery(int UserId, int Platform)
        {
            string LogDic = "子商户信息查询接口";
            YeepayUser yeepayUser = yeepayUserDao.Single(" UserId=@UserId", new { UserId });
            Users users = usersDao.Single(UserId);

            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Yeepay.Enums.YeepayType.子商户信息查询, UserId, Platform);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId}", YeepayLogDic, LogDic);

            reqCustomerInforQueryModel model = new reqCustomerInforQueryModel();
            model.customerNumber = yeepayUser != null ? yeepayUser.CustomerNumber : "";
            model.mobilePhone = users.Mobile;
            model.customerType = "CUSTOMER";

            StringBuilder sb = new StringBuilder();
            sb.Append(model.mainCustomerNumber);
            sb.Append(model.mobilePhone);
            sb.Append(model.customerNumber);
            sb.Append(model.customerType);
            string sn = sb.ToString();

            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqCustomerInforQueryModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "customerInforQuery.action";
            //action = "http://api.itorm.com/itapi/server/customerInforQuery";

            //执行请求
            respCustomerInforQueryModel resp = PostUrl<respCustomerInforQueryModel>(requestId, action, strPostPara.ToString(), model, LogDic);


            if (resp.backState == 0)
            {

            }
            return resp;
        }

        public static respCustomerInforQueryModel CustomerInforQuery(string Mobile, int Platform)
        {
            var user = usersDao.Single(" Mobile=@Mobile ", new { Mobile });
            if (user == null)
            {
                return new respCustomerInforQueryModel() { code = "8000", message = "用户不存在" };
            }
            return CustomerInforQuery(user.UserId, Platform);
        }
        #endregion

        #region 子商户信息修改接口 customerInforUpdate
        /// <summary>
        /// 修改银行卡
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Platform"></param>
        /// <param name="bankCardNumber"></param>
        /// <param name="bankName"></param>
        /// <returns></returns>
        public static respCustomerInforUpdate2Model CustomerInforUpdate(int UserId, int Platform, string bankCardNumber, string bankName)
        {
            string LogDic = "修改银行卡";
            YeepayUser yeepayUser = yeepayUserDao.Single(" UserId=@UserId", new { UserId });
            Users users = usersDao.Single(UserId);

            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Yeepay.Enums.YeepayType.修改银行卡信息, UserId, Platform);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId}", YeepayLogDic, LogDic);

            reqCustomerInforUpdateModel model = new reqCustomerInforUpdateModel();
            model.customerNumber = yeepayUser != null ? yeepayUser.CustomerNumber : "";
            model.modifyType = "2";
            model.bankCardNumber = bankCardNumber;
            model.bankName = bankName;

            StringBuilder sb = new StringBuilder();
            sb.Append(model.mainCustomerNumber);
            sb.Append(model.customerNumber);
            sb.Append(model.bankCardNumber);
            sb.Append(model.bankName);
            string sn = sb.ToString();

            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqCustomerInforUpdateModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "customerInforUpdate.action";
            //action = "http://api.itorm.com/itapi/server/customerInforUpdate";

            //执行请求
            respCustomerInforUpdate2Model resp = PostUrl<respCustomerInforUpdate2Model>(requestId, action, strPostPara.ToString(), model, LogDic);


            if (resp.backState == 0)
            {
                Bank bank = bankDao.Single("BankName=@bankName", new { bankName });
                UserBankCard ubk = userBankCardDao.Single("UserId=@UserId and TypeId=0", new { UserId });
                ubk.BankCard = bankCardNumber;
                ubk.BankName = bankName;
                ubk.BankCode = bank.BankCode;
                ubk.UTime = DateTime.Now;
                flag = userBankCardDao.Update(ubk);
                Logs.WriteLog($"修改银行卡号：UserId:{UserId},Platform:{Platform},requestId:{requestId},bankCardNumber:{bankCardNumber},bankName:{bankName}", YeepayLogDic, LogDic);
            }
            return resp;
        }

        /// <summary>
        /// 修改基本信息
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Platform"></param>
        /// <param name="bindMobile"></param>
        /// <param name="mailStr"></param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public static respCustomerInforUpdate6Model CustomerInforUpdate(int UserId, int Platform, string bindMobile, string mailStr, string areaCode)
        {
            string LogDic = "修改基本信息";
            YeepayUser yeepayUser = yeepayUserDao.Single(" UserId=@UserId", new { UserId });
            Users users = usersDao.Single(UserId);

            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Yeepay.Enums.YeepayType.修改商户基本信息, UserId, Platform);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId}", YeepayLogDic, LogDic);

            reqCustomerInforUpdateModel model = new reqCustomerInforUpdateModel();
            model.customerNumber = yeepayUser != null ? yeepayUser.CustomerNumber : "";
            model.modifyType = "6";
            model.bindMobile = bindMobile;
            model.mailStr = mailStr;
            model.areaCode = areaCode;

            StringBuilder sb = new StringBuilder();
            sb.Append(model.mainCustomerNumber);
            sb.Append(model.customerNumber);
            sb.Append(model.bindMobile);
            sb.Append(model.mailStr);
            sb.Append(model.areaCode);
            string sn = sb.ToString();

            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqCustomerInforUpdateModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "customerInforUpdate.action";
            action = "http://api.itorm.com/itapi/server/customerInforUpdate";

            //执行请求
            respCustomerInforUpdate6Model resp = PostUrl<respCustomerInforUpdate6Model>(requestId, action, strPostPara.ToString(), model, LogDic);


            if (resp.backState == 0)
            {

            }
            return resp;
        }


        #endregion

        #region 子商户费率查询接口 queryFeeSetApi
        public static respQueryFeeSetApiModel QueryFeeSetApi(int UserId, int Platform, int productType)
        {
            string LogDic = "子商户费率查询接口";
            YeepayUser yeepayUser = yeepayUserDao.Single(" UserId=@UserId", new { UserId });
            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Enums.YeepayType.子商户费率查询, UserId, Platform);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId},productType:{productType}", YeepayLogDic, LogDic);

            reqQueryFeeSetApiModel model = new reqQueryFeeSetApiModel();
            model.customerNumber = yeepayUser.CustomerNumber;
            model.productType = productType.ToString();

            StringBuilder sb = new StringBuilder();
            sb.Append(model.customerNumber);
            sb.Append(model.mainCustomerNumber);
            sb.Append(model.productType);

            string sn = sb.ToString();
            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqQueryFeeSetApiModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "queryFeeSetApi.action";
            //action = "http://api.itorm.com/itapi/server/queryFeeSetApi";

            //执行请求
            respQueryFeeSetApiModel resp = PostUrl<respQueryFeeSetApiModel>(requestId, action, strPostPara.ToString(), model, LogDic);

            if (resp.backState == 0)
            {

            }
            return resp;
        }
        #endregion

        #region 子商户费率设置 feeSetApi
        public static respFeeSetApiModel FeeSetApi(int UserId, int Platform, Yeepay.Enums.YeepayType ytEnum, string rate = "")
        {
            string LogDic = "子商户费率设置";
            YeepayUser yeepayUser = yeepayUserDao.Single(" UserId=@UserId", new { UserId });
            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)ytEnum, UserId, Platform);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId}", YeepayLogDic, LogDic);

            reqFeeSetApiModel model = new reqFeeSetApiModel();
            model.customerNumber = yeepayUser.CustomerNumber;
            model.productType = ((int)ytEnum - 200).ToString();
            model.rate = rate;

            StringBuilder sb = new StringBuilder();
            sb.Append(model.customerNumber);
            sb.Append(model.mainCustomerNumber);
            sb.Append(model.productType);
            sb.Append(model.rate);
            string sn = sb.ToString();
            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqFeeSetApiModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "feeSetApi.action";
            //action = "http://api.itorm.com/itapi/server/feeSetApi";

            //执行请求
            respFeeSetApiModel resp = PostUrl<respFeeSetApiModel>(requestId, action, strPostPara.ToString(), model, LogDic);

            if (resp.backState == 0)
            {
                #region 设置费率
                switch (resp.productType)
                {
                    case "1":
                        yeepayUser.Rate1 = Convert.ToDecimal(resp.rate);
                        yeepayUser.RateState1 = 1;
                        break;
                    case "2":
                        yeepayUser.Rate2 = Convert.ToDecimal(resp.rate);
                        yeepayUser.RateState2 = 1;
                        break;
                    case "3":
                        yeepayUser.Rate3 = Convert.ToDecimal(resp.rate);
                        yeepayUser.RateState3 = 1;
                        break;
                    case "4":
                        yeepayUser.Rate4 = Convert.ToDecimal(resp.rate);
                        yeepayUser.RateState4 = 1;
                        break;
                    case "5":
                        yeepayUser.Rate5 = Convert.ToDecimal(resp.rate);
                        yeepayUser.RateState5 = 1;
                        break;
                    default:
                        break;
                }
                yeepayUser.UTime = DateTime.Now;
                flag = yeepayUserDao.Update(yeepayUser);
                Logs.WriteLog($"设置费率：Platform:{Platform},requestId:{requestId},productType:{resp.productType},rate:{resp.rate},flag={flag}", YeepayLogDic, LogDic);
                #endregion
            }
            return resp;
        }
        
        #endregion

        #region 子商户限额查询接口 tradeLimitQuery
        /// <summary>
        /// 子商户限额查询接口
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Platform"></param>
        /// <param name="bankCardType">卡类型</param>
        /// <param name="bankCardNo">卡号</param>
        /// <returns></returns>
        public static respTradeLimitQueryModel TradeLimitQuery(int UserId, int Platform, BankCardType bankCardType, string bankCardNo = "")
        {
            string LogDic = "子商户限额查询接口";
            YeepayUser yeepayUser = yeepayUserDao.Single(" UserId=@UserId", new { UserId });
            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Enums.YeepayType.子商户限额查询, UserId, Platform);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId}", YeepayLogDic, LogDic);

            reqTradeLimitQueryModel model = new reqTradeLimitQueryModel();
            model.customerNumber = yeepayUser != null ? yeepayUser.CustomerNumber : "";
            model.bankCardNo = bankCardNo;
            model.bankCardType = bankCardType == BankCardType.信用卡 ? "CREDIT" : "DEBIT";
            model.tradeLimitConfigKey = "1";

            StringBuilder sb = new StringBuilder();
            sb.Append(model.customerNumber);
            sb.Append(model.mainCustomerNumber);
            sb.Append(model.bankCardType);
            sb.Append(model.bankCardNo);
            sb.Append(model.tradeLimitConfigKey);

            string sn = sb.ToString();
            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqQueryFeeSetApiModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "tradeLimitQuery.action";
            //action = "http://api.itorm.com/itapi/server/tradeLimitQuery";

            //执行请求
            respTradeLimitQueryModel resp = PostUrl<respTradeLimitQueryModel>(requestId, action, strPostPara.ToString(), model, LogDic);

            if (resp.backState == 0)
            {

            }
            return resp;
        }
        #endregion

        #region 收款接口（交易接口） receiveApi

        /// <summary>
        /// 收款
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Amount"></param>
        /// <param name="Platform"></param>
        /// <param name="payerBankAccountNo"></param>
        /// <returns></returns>
        public static respReceiveApiModel ReceiveApi(int UserId = 0, decimal Amount = 0, int Platform = 0, string payerBankAccountNo = "")
        {
            string LogDic = "收款接口";
            YeepayUser yeepayUser = yeepayUserDao.Single(" UserId=@UserId", new { UserId });

            //获得支付ID
            int keyId = payRecordDao.Init(UserId, Amount, Platform, Ip.GetClientIp(), payerBankAccountNo);
            Logs.WriteLog($"创建支付记录：UserId:{UserId},Platform:{Platform},keyId:{keyId},Amount={Amount}", YeepayLogDic, LogDic);


            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)YeepayType.收款接口, UserId, Platform, keyId);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId},Amount={Amount}", YeepayLogDic, LogDic);

            reqReceiveApiModel model = new reqReceiveApiModel();
            model.customerNumber = yeepayUser.CustomerNumber;
            model.requestId = requestId.ToString();
            model.source = "B";
            model.amount = Amount.ToString();
            string[] mccs = new string[] { "5311", "4511", "4733" };
            Random r = new Random();
            model.mcc = mccs[r.Next(0, 3)];
            model.webCallBackUrl = YeepayNoticeUrl;
            model.payerBankAccountNo = payerBankAccountNo;//支付卡号
            model.autoWithdraw = "";
            model.withdrawCardNo = "";
            model.callBackUrl = YeepayNoticeUrl + "receiveApi";
            model.withdrawCallBackUrl = "";// YeepayNoticeUrl + "withDrawApi";


            StringBuilder sb = new StringBuilder();
            sb.Append(model.source);
            sb.Append(model.mainCustomerNumber);
            sb.Append(model.customerNumber);
            sb.Append(model.amount);
            sb.Append(model.mcc);
            sb.Append(model.requestId);
            //sb.Append(model.mobileNumber);
            sb.Append(model.callBackUrl);
            sb.Append(model.webCallBackUrl);
            sb.Append(model.payerBankAccountNo);

            string sn = sb.ToString();
            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqReceiveApiModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "receiveApi.action";
            //action = "http://api.itorm.com/itapi/server/receiveApi";

            //执行请求
            respReceiveApiModel resp = PostUrl<respReceiveApiModel>(requestId, action, strPostPara.ToString(), model, LogDic, 5);

            if (resp.code == "0000")
            {
                flag = payRecordDao.UpdateState(keyId, 1, resp.message);
                Logs.WriteLog($"支付记录发起成功状态更新：keyId:{keyId},code:{resp.code},message:{ resp.message},State:{flag}", YeepayLogDic, LogDic);

            }
            else
            {
                //如果失败
                flag = payRecordDao.UpdateState(keyId, -1, resp.message);
                Logs.WriteLog($"支付记录失败状态更新：keyId:{keyId},code:{resp.code},message:{ resp.message},State:{flag}", YeepayLogDic, LogDic);
            }

            return resp;
        }


        //新版
        public static respReceiveApiModel ReceiveApi(int UserId = 0, decimal Amount = 0, int Platform = 0, int UbkId=0)
        {
            string LogDic = "收款接口";
            Logs.WriteLog($"初始记录：UserId:{UserId},Platform:{Platform},UbkId:{UbkId},Amount={Amount}", YeepayLogDic, LogDic);
            YeepayUser yeepayUser = yeepayUserDao.Single(" UserId=@UserId", new { UserId });

            UserBankCard ukb = userBankCardDao.Single(UbkId);
            //获得支付ID
            int keyId = payRecordDao.Init(UbkId,Amount,Platform,Ip.GetClientIp(),(int)Logic.ChannelType.易宝);
            Logs.WriteLog($"创建支付记录：UserId:{UserId},Platform:{Platform},keyId:{keyId},Amount={Amount}", YeepayLogDic, LogDic);


            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)YeepayType.收款接口, UserId, Platform, keyId);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId},Amount={Amount}", YeepayLogDic, LogDic);

            reqReceiveApiModel model = new reqReceiveApiModel();
            model.customerNumber = yeepayUser.CustomerNumber;
            model.requestId = requestId.ToString();
            model.source = "B";
            model.amount = Amount.ToString();
            string[] mccs = new string[] { "5311", "4511", "4733" };
            Random r = new Random();
            model.mcc = mccs[r.Next(0, 3)];
            model.webCallBackUrl = YeepayNoticeUrl;
            model.payerBankAccountNo = ukb.BankCard;//支付卡号
            model.autoWithdraw = "";
            model.withdrawCardNo = "";
            model.callBackUrl = YeepayNoticeUrl + "receiveApi";
            model.withdrawCallBackUrl = "";// YeepayNoticeUrl + "withDrawApi";


            StringBuilder sb = new StringBuilder();
            sb.Append(model.source);
            sb.Append(model.mainCustomerNumber);
            sb.Append(model.customerNumber);
            sb.Append(model.amount);
            sb.Append(model.mcc);
            sb.Append(model.requestId);
            //sb.Append(model.mobileNumber);
            sb.Append(model.callBackUrl);
            sb.Append(model.webCallBackUrl);
            sb.Append(model.payerBankAccountNo);

            string sn = sb.ToString();
            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqReceiveApiModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "receiveApi.action";
            //action = "http://api.itorm.com/itapi/server/receiveApi";

            //执行请求
            respReceiveApiModel resp = PostUrl<respReceiveApiModel>(requestId, action, strPostPara.ToString(), model, LogDic, 5);

            if (resp.code == "0000")
            {
                flag = payRecordDao.UpdateState(keyId, 1, resp.message);
                Logs.WriteLog($"支付记录发起成功状态更新：keyId:{keyId},code:{resp.code},message:{ resp.message},State:{flag}", YeepayLogDic, LogDic);

            }
            else
            {
                //如果失败
                flag = payRecordDao.UpdateState(keyId, -1, resp.message);
                Logs.WriteLog($"支付记录失败状态更新：keyId:{keyId},code:{resp.code},message:{ resp.message},State:{flag}", YeepayLogDic, LogDic);
            }

            return resp;
        }



        #endregion

        #region 结算接口 withDrawApi


        public static respWithDrawApiModel WithDrawApi(int PayId = 0, int Platform = 0)
        {
            bool flag = false;
            string LogDic = "结算接口";
            Logs.WriteLog($"结算请求开始：Platform:{Platform},PayId:{PayId}", YeepayLogDic, LogDic);

            PayRecord pay = payRecordDao.Single(PayId);
            if (pay != null && pay.State != 10)
            {
                return new respWithDrawApiModel() { code = "9999", message = "订单未完成收款功能" };
            }
            if (pay != null && pay.DrawState > 0 && pay.DrawState < 10)
            {
                return new respWithDrawApiModel() { code = "9999", message = "订单已发起结算请求" };
            }
            if (pay != null && pay.DrawState == 10)
            {
                return new respWithDrawApiModel() { code = "9999", message = "订单已结算成功" };
            }
            int UserId = pay.UserId;

            //获得支付ID

            int keyId = withDrawDao.Init(UserId, PayId, pay.WithDrawAmount, Platform, "");
            pay.WithDrawId = keyId;
            pay.UTime = DateTime.Now;
            flag = payRecordDao.Update(pay);
            Logs.WriteLog($"支付记录更新WithDrawId：UserId:{UserId},Platform:{Platform},PayId={PayId},WithDrawId={keyId},State:{flag}", YeepayLogDic, LogDic);

            YeepayUser yeepayUser = yeepayUserDao.Single(" UserId=@UserId", new { UserId });

            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)YeepayType.结算接口, UserId, Platform, keyId);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId},Amount={pay.Amount},PayId={PayId},DrawId={keyId}", YeepayLogDic, LogDic);

            reqWithDrawApiModel model = new reqWithDrawApiModel();
            model.customerNumber = yeepayUser.CustomerNumber;
            model.externalNo = requestId.ToString();
            model.transferWay = "1";
            model.amount = pay.WithDrawAmount.ToString("F2");
            model.callBackUrl = YeepayNoticeUrl + "withDrawApi";

            StringBuilder sb = new StringBuilder();
            sb.Append(model.amount);
            sb.Append(model.customerNumber);
            sb.Append(model.externalNo);
            sb.Append(model.mainCustomerNumber);
            sb.Append(model.transferWay);
            sb.Append(model.callBackUrl);
            string sn = sb.ToString();
            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqWithDrawApiModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "withDrawApi.action";
            //action = "http://api.itorm.com/itapi/server/withDrawApi";

            //执行请求
            respWithDrawApiModel resp = PostUrl<respWithDrawApiModel>(requestId, action, strPostPara.ToString(), model, LogDic, 5);

            if (resp.backState == 0)
            {
                flag = withDrawDao.UpdateState(keyId, (int)WithDrawState.已接受, resp.message);
                Logs.WriteLog($"结算记录发起成功状态更新：keyId:{keyId},code:{resp.code},message:{ resp.message},State:{flag}", YeepayLogDic, LogDic);
                flag = payRecordDao.UpdateDrawState(PayId, (int)WithDrawState.已接受);
                Logs.WriteLog($"支付记录发起成功状态更新：PayId:{PayId},code:{resp.code},message:{ resp.message},State:{flag}", YeepayLogDic, LogDic);
            }
            else
            {
                //如果失败
                flag = withDrawDao.UpdateState(keyId, (int)WithDrawState.结算失败, resp.message);
                Logs.WriteLog($"结算记录失败状态更新：keyId:{keyId},code:{resp.code},message:{ resp.message},State:{flag}", YeepayLogDic, LogDic);
                flag = payRecordDao.UpdateDrawState(PayId, (int)WithDrawState.结算失败);
                Logs.WriteLog($"支付记录失败状态更新：PayId:{PayId},code:{resp.code},message:{ resp.message},State:{flag}", YeepayLogDic, LogDic);
            }

            return resp;
        }


        #endregion

        #region 结算手续费查询接口 lendTargetFeeQuery
        public static respLendTargetFeeQueryModel LendTargetFeeQuery(int UserId = 0, int Platform = 0, decimal transAmount = 0m)
        {
            string LogDic = "结算手续费查询接口";
            YeepayUser yeepayUser = yeepayUserDao.Single(" UserId=@UserId", new { UserId });
            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Enums.YeepayType.结算手续费查询, UserId, Platform);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId},transAmount:{transAmount}", YeepayLogDic, LogDic);

            reqLendTargetFeeQueryModel model = new reqLendTargetFeeQueryModel();
            model.customerNumber = yeepayUser != null ? yeepayUser.CustomerNumber : "";
            model.transType = "1";
            model.transAmount = transAmount.ToString("F2");

            StringBuilder sb = new StringBuilder();
            sb.Append(model.mainCustomerNumber);
            sb.Append(model.customerNumber);
            sb.Append(model.transType);
            sb.Append(model.transAmount);
            string sn = sb.ToString();
            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqLendTargetFeeQueryModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "lendTargetFeeQuery.action";
            //action = "http://api.itorm.com/itapi/server/lendTargetFeeQuery";

            //执行请求
            respLendTargetFeeQueryModel resp = PostUrl<respLendTargetFeeQueryModel>(requestId, action, strPostPara.ToString(), model, LogDic);

            if (resp.backState == 0)
            {

            }
            return resp;
        }

        #endregion

        #region 交易查询 tradeReviceQuery
        public static respTradeReviceQueryModel TradeReviceQuery(reqTradeReviceQueryModel model, int Platform = 0, int UserId = 0)
        {

            string LogDic = "交易查询";
            YeepayUser yeepayUser = yeepayUserDao.Single(" UserId=@UserId", new { UserId });
            model.customerNumber = UserId > 0 ? yeepayUser.CustomerNumber : "";
            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Yeepay.Enums.YeepayType.交易查询, UserId, Platform);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId},model={JsonConvert.SerializeObject(model)}", YeepayLogDic, LogDic);


            StringBuilder sb = new StringBuilder();
            sb.Append(model.mainCustomerNumber);
            sb.Append(model.customerNumber);
            sb.Append(model.requestId);
            sb.Append(model.createTimeBegin);
            sb.Append(model.createTimeEnd);
            sb.Append(model.payTimeBegin);
            sb.Append(model.payTimeEnd);
            sb.Append(model.lastUpdateTimeBegin);
            sb.Append(model.lastUpdateTimeEnd);
            sb.Append(model.status);
            sb.Append(model.busiType);
            sb.Append(model.pageNo);
            string sn = sb.ToString();
            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqTradeReviceQueryModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "tradeReviceQuery.action";
            //action = "http://api.itorm.com/itapi/server/tradeReviceQuery";

            //执行请求
            respTradeReviceQueryModel resp = PostUrl<respTradeReviceQueryModel>(requestId, action, strPostPara.ToString(), model, LogDic);

            if (resp.backState == 0)
            {

            }
            return resp;
        }

        /// <summary>
        /// 根据收款订单号查询 交易记录
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="Platform"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static respTradeReviceQueryModel TradeReviceQuery(string requestId = "", int Platform = 0)
        {
            reqTradeReviceQueryModel model = new reqTradeReviceQueryModel();
            model.requestId = requestId;
            model.createTimeBegin = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");
            model.createTimeEnd = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            model.pageNo = "1";
            return TradeReviceQuery(model, Platform, 0);
        }

        /// <summary>
        /// 根据用户查询
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <param name="status">状态</param>
        /// <param name="pageNo">页码</param>
        /// <param name="Platform"></param>
        /// <returns></returns>
        public static respTradeReviceQueryModel TradeReviceQuery(int UserId = 0, string status = "", int pageNo = 1, int Platform = 0)
        {
            reqTradeReviceQueryModel model = new reqTradeReviceQueryModel();
            model.createTimeBegin = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");
            model.createTimeEnd = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            model.status = status;
            model.pageNo = pageNo.ToString();
            return TradeReviceQuery(model, Platform, UserId);
        }

        /// <summary>
        /// 查询全平台交易记录
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="pageNo"></param>
        /// <param name="Platform"></param>
        /// <returns></returns>
        public static respTradeReviceQueryModel TradeReviceQuery(string status = "", int pageNo = 1, int Platform = 0)
        {
            reqTradeReviceQueryModel model = new reqTradeReviceQueryModel();
            model.createTimeBegin = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");
            model.createTimeEnd = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            model.status = status;
            model.pageNo = pageNo.ToString();
            return TradeReviceQuery(model, Platform, 0);
        }


        #endregion

        #region 可用余额查询 customerBalanceQuery

        public static respCustomerBalanceQueryModel CustomerBalanceQuery(int UserId, int Platform, int balanceType = 1)
        {

            string LogDic = "可用余额查询";
            YeepayUser yeepayUser = yeepayUserDao.Single(" UserId=@UserId", new { UserId });
            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Yeepay.Enums.YeepayType.可用余额查询, UserId, Platform);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId},balanceType={balanceType}", YeepayLogDic, LogDic);

            reqCustomerBalanceQueryModel model = new reqCustomerBalanceQueryModel();
            model.customerNumber = yeepayUser.CustomerNumber;
            model.balanceType = balanceType.ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append(model.mainCustomerNumber);
            sb.Append(model.customerNumber);
            sb.Append(model.balanceType);
            string sn = sb.ToString();
            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqCustomerBalanceQueryModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "customerBalanceQuery.action";
            //action = "http://api.itorm.com/itapi/server/customerBalanceQuery";

            //执行请求
            respCustomerBalanceQueryModel resp = PostUrl<respCustomerBalanceQueryModel>(requestId, action, strPostPara.ToString(), model, LogDic);

            if (resp.backState == 0)
            {

            }
            return resp;
        }

        #endregion

        #region 结算记录查询接口  transferQuery
        public static respTransferQueryModel TransferQuery(reqTransferQueryModel model, int UserId, int Platform)
        {

            string LogDic = "结算记录查询";
            YeepayUser yeepayUser = yeepayUserDao.Single(" UserId=@UserId", new { UserId });
            model.customerNumber = UserId > 0 ? yeepayUser.CustomerNumber : "";
            model.transferWay = "1";
            bool flag = false;
            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Yeepay.Enums.YeepayType.结算记录查询, UserId, Platform);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId},model={JsonConvert.SerializeObject(model)}", YeepayLogDic, LogDic);


            StringBuilder sb = new StringBuilder();
            sb.Append(model.customerNumber);
            sb.Append(model.externalNo);
            sb.Append(model.mainCustomerNumber);
            sb.Append(model.pageNo);
            sb.Append(model.requestDateSectionBegin);
            sb.Append(model.requestDateSectionEnd);
            sb.Append(model.serialNo);
            sb.Append(model.transferStatus);
            sb.Append(model.transferWay);
            string sn = sb.ToString();
            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;



            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqTransferQueryModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "transferQuery.action";
            //action = "http://api.itorm.com/itapi/server/transferQuery";

            //执行请求
            respTransferQueryModel resp = PostUrl<respTransferQueryModel>(requestId, action, strPostPara.ToString(), model, LogDic);

            if (resp.backState == 0)
            {

            }
            return resp;
        }

        /// <summary>
        /// 根据结算请求号查询 结算记录
        /// </summary>
        /// <param name="externalNo">结算请求号</param>
        /// <param name="Platform"></param>
        /// <returns></returns>
        public static respTransferQueryModel TransferQuery(string externalNo = "", int Platform = 0)
        {
            YeepayLog yeepayLog = yeepayLogDao.Single(Convert.ToInt32(externalNo));
            int UserId = yeepayLog.UserId;
            reqTransferQueryModel model = new reqTransferQueryModel();
            model.externalNo = externalNo;
            model.requestDateSectionBegin = yeepayLog.CTime.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
            model.requestDateSectionEnd = yeepayLog.CTime.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            model.transferWay = "1";
            model.pageNo = "1";
            return TransferQuery(model, UserId, Platform);
        }

        /// <summary>
        /// 根据用户查询 结算记录
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <param name="status">状态</param>
        /// <param name="pageNo">页码</param>
        /// <param name="Platform"></param>
        /// <returns></returns>
        public static respTransferQueryModel TransferQuery(int UserId = 0, string transferStatus = "", int pageNo = 1, int Platform = 0)
        {
            reqTransferQueryModel model = new reqTransferQueryModel();
            model.externalNo = "";
            model.requestDateSectionBegin = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss");
            model.requestDateSectionEnd = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            model.transferWay = "1";
            model.pageNo = "1";
            model.transferStatus = transferStatus;
            return TransferQuery(model, UserId, Platform);
        }
        #endregion

        #region 垫资额度查询接口 queryRJTBalance

        public static respQueryRJTBalanceModel QueryRJTBalance(int Platform)
        {

            string LogDic = "垫资额度查询";

            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Yeepay.Enums.YeepayType.垫资额度查询, 0, Platform);
            Logs.WriteLog($"获取请求流水号：Platform:{Platform},requestId:{requestId}", YeepayLogDic, LogDic);

            reqQueryRJTBalanceModel model = new reqQueryRJTBalanceModel();
            StringBuilder sb = new StringBuilder();
            sb.Append(model.mainCustomerNumber);
            string sn = sb.ToString();
            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqQueryRJTBalanceModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "queryRJTBalance.action";
            //action = "http://api.itorm.com/itapi/server/queryRJTBalance";

            //执行请求
            respQueryRJTBalanceModel resp = PostUrl<respQueryRJTBalanceModel>(requestId, action, strPostPara.ToString(), model, LogDic);

            if (resp.backState == 0)
            {

            }
            return resp;
        }


        #endregion

        #region 代理商返佣转账接口 transferToCustomer

        public static respTransferToCustomerModel TransferToCustomer(int UserId, int Platform, decimal transAmount, string remark = "")
        {

            string LogDic = "代理商返佣转账接口";

            //获取请求流水号
            int requestId = yeepayLogDao.Init((int)Yeepay.Enums.YeepayType.代理商返佣转账, 0, Platform);
            Logs.WriteLog($"获取请求流水号：UserId:{UserId},Platform:{Platform},requestId:{requestId},transAmount:{transAmount},remark:{remark}", YeepayLogDic, LogDic);

            reqTransferToCustomerModel model = new reqTransferToCustomerModel();
            StringBuilder sb = new StringBuilder();
            sb.Append(model.mainCustomerNumber);
            string sn = sb.ToString();
            var hmac = ITOrm.Utility.Encryption.EncryptionHelper.HMACMD5(YeepayHmacKey, sn);
            model.hmac = hmac;

            //拼接所有传递参数
            //公共字段
            StringBuilder strPostPara = new StringBuilder();
            PropertyInfo[] pis = typeof(reqTransferToCustomerModel).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                object keyName = pi.GetValue(model, null);
                if (keyName != null && !string.IsNullOrEmpty(keyName.ToString()))
                {
                    strPostPara.Append("&").Append(pi.Name).Append("=").Append(pi.GetValue(model, null));
                }
            }
            string action = YeepayDomain + "transferToCustomer.action";
            //action = "http://api.itorm.com/itapi/server/transferToCustomer";

            //执行请求
            respTransferToCustomerModel resp = PostUrl<respTransferToCustomerModel>(requestId, action, strPostPara.ToString(), model, LogDic);

            if (resp.backState == 0)
            {

            }
            return resp;
        }
        #endregion

        private static T PostUrl<T>(int requestId, string action, string data, object json, string logPath, int state = 10)
        {

            try
            {
                bool flag = false;
                string result = string.Empty;
                //请求前日志记录
                Logs.WriteLog("提交参数：" + JsonConvert.SerializeObject(json), YeepayLogDic, logPath);
                yeepayLogParasDao.Init(requestId, JsonConvert.SerializeObject(json), 0);
                //执行请求
                int responseState = ITOrm.Utility.Client.HttpHelper.HttpPost(action, data, Encoding.UTF8, out result);
                if (responseState != 200)
                {
                    result = $"{{ \"code\":\"{responseState}\", \"message\":\"{result}\"  }}";
                }
                //返回后日志记录
                Logs.WriteLog("返回参数：" + result, YeepayLogDic, logPath);
                yeepayLogParasDao.Init(requestId, result, 1);

                //易宝日志状态更新
                respModel resp = JsonConvert.DeserializeObject<respModel>(result);
                flag = yeepayLogDao.UpdateState(requestId, resp.code, resp.message, resp.code == "0000" ? state : -1);

                Logs.WriteLog($"易宝日志状态更新：requestId:{requestId},code:{resp.code},message:{ resp.message},State:{flag}", YeepayLogDic, logPath);
                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception e)
            {
                return JsonConvert.DeserializeObject<T>($"{{ \"code\":\"-1000\", \"message\":\"{e.Message}\"  }}");
            }


        }



    }
}
