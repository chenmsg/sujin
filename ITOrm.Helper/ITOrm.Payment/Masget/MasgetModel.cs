using ITOrm.Core.Helper;
using ITOrm.Host.BLL;
using ITOrm.Host.Models;
using ITOrm.Utility.Const;
using ITOrm.Utility.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace ITOrm.Payment.Masget
{



    #region 公共调用类
    public class reqMasgetModel
    {
        public static MasgetUserBLL masgetUserDao = new MasgetUserBLL();
        /// <summary>
        /// 发送请求的公司id，由银联供应链综合服务平台统一分发
        /// </summary>
        public string appid
        {
            get
            {
                if (MasgetID > 0)
                {
                    return mUser.Appid;
                }
                switch (chanel)
                {
                  
                    case Logic.ChannelType.荣邦科技积分:
                        return MasgetDepository.MasgetAppid[0];
                    case Logic.ChannelType.荣邦科技无积分:
                        return MasgetDepository.MasgetAppid[1];
                    case Logic.ChannelType.荣邦3:
                        return MasgetDepository.MasgetAppid[2];
                    default:
                        break;
                }
                return "";
            }
        }
        /// <summary>
        /// API接口名称
        /// </summary>
        public string method { get; set; }
        /// <summary>
        /// 可选，指定响应格式。默认json,目前支持格式为json
        /// </summary>
        public string format { get { return "json"; } }
        /// <summary>
        /// 业务数据经过AES加密后，进行urlsafe base64编码
        /// </summary>
        public string data { get; set; }
        /// <summary>
        /// 明文
        /// </summary>
        public string dataExpress { get; set; }
        /// <summary>
        /// API协议版本，可选值：2.0
        /// </summary>
        public string v { get { return "2.0"; } }
        /// <summary>
        /// /// <summary>
        /// 时间戳，格式为yyyy-MM-dd HH:mm:ss，时区为GMT+8，例如：2016-01-01 12:00:00。API服务端允许客户端请求最大时间误差为5分钟
        /// </summary>
        /// </summary>
        public string timestamp
        {
            get
            {
                //return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                return ((int)(DateTime.Now - startTime).TotalSeconds).ToString();
            }
        }
        /// <summary>
        /// 用户的会话状态(根据API定义是否需要用户授权决定)
        /// </summary>
        public string session
        {
            get
            {
                if (MasgetID > 0)
                {
                    return mUser.Session;
                }

                switch (chanel)
                {

                    case Logic.ChannelType.荣邦科技积分:
                        return MasgetDepository.MasgetSession[0];
                    case Logic.ChannelType.荣邦科技无积分:
                        return MasgetDepository.MasgetSession[1];
                    case Logic.ChannelType.荣邦3:
                        return MasgetDepository.MasgetSession[2];
                    default:
                        break;
                }
                return "";


            }
        }
        /// <summary>
        /// 32位小写的md5(secretkey + appid + data + format + method + session + target_appid + timestamp + v + secretkey )
        /// secretkey 密钥有银联供应链综合服务平台分配 
        /// 根据参数名称的ASCII码表的顺序排序
        /// </summary>
        public string sign { get { return SecurityHelper.GetMD5String(secretkey + appid + data + format + method + session + target_appid + timestamp + v + secretkey); } }
        /// <summary>
        /// 目标公司id，仅当使用第三方或其他子系统时有效
        /// </summary>
        public string target_appid { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public string secretkey
        {
            get
            {
                if (MasgetID > 0)
                {
                    return mUser.Secretkey;
                }


                switch (chanel)
                {
                    case Logic.ChannelType.荣邦科技积分:
                        return MasgetDepository.MasgetSecretKey[0];
                    case Logic.ChannelType.荣邦科技无积分:
                        return MasgetDepository.MasgetSecretKey[1];
                    case Logic.ChannelType.荣邦3:
                        return MasgetDepository.MasgetSecretKey[2];
                    default:
                        break;
                }
                return "";
            }
        }

        public Logic.ChannelType chanel = Logic.ChannelType.荣邦科技积分;

        /// <summary>
        /// MasgetID
        /// </summary>
        public int MasgetID { get; set; }

        public MasgetUser mUser
        {
            get
            {
                if (MasgetID > 0)
                {
                    return masgetUserDao.Single(MasgetID);
                }
                return null;
            }
        }
    }


    public class respMasgetModel<T>
    {
        /// <summary>
        /// 返回码，ret=0表示请求成功,其他，则为异常
        /// </summary>
        public int ret { get; set; }
        /// <summary>
        /// 返回码说明
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 业务请求返回数据,具体格式由API决定
        /// </summary>
        public T data { get; set; }
        public int backState { get {
                if (ret != 0)
                {
                    return ret * -1;
                }
                return ret; } }
        /// <summary>
        /// 收银台URL
        /// </summary>
        public string url { get; set; }
    }

    public class noticeMasgetModel<T>
    {
        public static MasgetUserBLL masgetUserDao = new MasgetUserBLL();
        /// <summary>
        /// Appid
        /// </summary>
        public string Appid { get; set; }
        /// <summary>
        /// 明文实体
        /// </summary>
        public T dataExpress
        {
            get
            {
                if (mUser != null)
                {
                    string json = ITOrm.Payment.Masget.AES.Decrypt(Data, mUser.Secretkey, mUser.Secretkey);
                    return JsonConvert.DeserializeObject<T>(json);
                }
                return default(T);
            }
        }
        /// <summary>
        /// 接收的密文内容
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// Method 接口名称
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 接收的签名
        /// </summary>
        public string Sign { get; set; }
        /// <summary>
        /// 我方签名的结果
        /// </summary>
        public string sysSign
        {
            get
            {
                if (mUser != null)
                {
                    string lastSign = $"{Data}{mUser.Secretkey}";
                   return SecurityHelper.GetMD5String(lastSign);
                }
                return "";
            }
        }
        /// <summary>
        /// 签名对比结果
        /// </summary>
        public bool IsSign
        {
            get
            {
                return Sign == sysSign;
            }
        }
        /// <summary>
        /// 查询的荣邦商户
        /// </summary>
        public MasgetUser mUser
        {
            get
            {
                var mUser = masgetUserDao.Single(" CompanyId=@Appid", new { Appid });
                return mUser;
            }
        }
    }

    public class OptionFee
    {
        public decimal Rate1 { get; set; }
        public decimal Rate3 { get; set; }
        public string ratecode { get; set; }
    }
    #endregion

    #region 快速进件（相当于开户） masget.webapi.com.subcompany.add

    public class reqSubcompanyAddModel
    {
        /// <summary>
        /// 商户名称
        /// </summary>
        public string companyname { get; set; }
        /// <summary>
        /// 商户编码(由机构管理，保证唯一)
        /// </summary>
        public string companycode { get; set; }
        /// <summary>
        /// 账户名
        /// </summary>
        public string accountname { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string bankaccount { get; set; }
        /// <summary>
        /// 开户支行名称
        /// </summary>
        public string bank { get; set; }
        /// <summary>
        /// 联行号
        /// </summary>
        public string bankcode { get; set; }
        /// <summary>
        /// 账户类型
        /// 1=个人账户
        /// 0=企业账户
        /// </summary>
        public string accounttype { get { return "1"; } }
        /// <summary>
        /// 银行卡类型,默认1
        /// 1=储蓄卡
        /// 2=信用卡
        /// </summary>
        public string bankcardtype { get { return "1"; } }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobilephone { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string idcardno { get; set; }
        /// <summary>
        /// 商户地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 登录名（不传默认为手机号）
        /// </summary>
        public string loginname { get; set; }
        /// <summary>
        /// 费率套餐编码（不传默认费率套餐）
        /// </summary>
        public string ratecode { get; set; }
        /// <summary>
        /// 入账规则（默认为1）
        /// 1-入账到子商户银行卡
        /// 6-入账到子商户钱包
        /// 7-入账到机构
        /// </summary>
        public string accountrule { get { return "1"; } }
    }

    public class respSubcompanyAddModel
    {
        /// <summary>
        /// 子商户id
        /// </summary>
        public string companyid { get; set; }
        /// <summary>
        /// 子商户名称
        /// </summary>
        public string companyname { get; set; }
        /// <summary>
        /// 商户编码(由机构管理，保证唯一)
        /// </summary>
        public string companycode { get; set; }
        /// <summary>
        /// 账户名
        /// </summary>
        public string accountname { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string bankaccount { get; set; }
        /// <summary>
        /// 开户支行名称
        /// </summary>
        public string bank { get; set; }
        /// <summary>
        /// 联行号
        /// </summary>
        public string bankcode { get; set; }
        /// <summary>
        /// 账户类型
        /// 1=个人账户
        /// 0=企业账户
        /// </summary>
        public string accounttype { get; set; }
        /// <summary>
        /// 银行卡类型,默认1
        /// 1=储蓄卡
        /// 2=信用卡
        /// </summary>
        public string bankcardtype { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobilephone { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string idcardno { get; set; }
        /// <summary>
        /// 商户地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        public string openapiurl { get; set; }
        /// <summary>
        /// appid
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 会话
        /// </summary>
        public string session { get; set; }
        /// <summary>
        /// 动态密钥
        /// </summary>
        public string secretkey { get; set; }
    }
    #endregion

    #region 子商户秘钥下载  masget.webapi.com.subcompany.get
    public class reqSubcompanyGetModel
    {
        /// <summary>
        /// 商户编码
        /// </summary>
        public string companycode { get; set; }
        /// <summary>
        /// 手机号(说明：手机号和商户编码二选一，优先使用商户编号查询)
        /// </summary>
        public string mobilephone { get; set; }
    }

    public class respSubcompanyGetModel
    {
        /// <summary>
        /// 子商户id
        /// </summary>
        public string companyid { get; set; }
        /// <summary>
        /// 子商户名称
        /// </summary>
        public string companyname { get; set; }
        /// <summary>
        /// 商户编码(由机构管理，保证唯一)
        /// </summary>
        public string companycode { get; set; }
        /// <summary>
        /// 账户名
        /// </summary>
        public string accountname { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string bankaccount { get; set; }
        /// <summary>
        /// 开户支行名称
        /// </summary>
        public string bank { get; set; }
        /// <summary>
        /// 联行号
        /// </summary>
        public string bankcode { get; set; }
        /// <summary>
        /// 账户类型
        /// 1=个人账户
        /// 0=企业账户
        /// </summary>
        public string accounttype { get; set; }
        /// <summary>
        /// 银行卡类型,默认1
        /// 1=储蓄卡
        /// 2=信用卡
        /// </summary>
        public string bankcardtype { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobilephone { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string idcardno { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        public string openapiurl { get; set; }
        /// <summary>
        /// appid
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 会话
        /// </summary>
        public string session { get; set; }
        /// <summary>
        /// 动态密钥
        /// </summary>
        public string secretkey { get; set; }
    }
    #endregion

    #region 商户通道入驻接口 masget.pay.compay.router.samename.open

    public class reqSamenameOpenModel
    {
        public string companyid { get; set; }
    }
    #endregion

    #region 申请开通快捷协议 masget.pay.collect.router.treaty.apply

    public class reqTreatyApplyModel
    {
        /// <summary>
        /// 账户名
        /// </summary>
        public string accountname { get; set; }
        /// <summary>
        /// 账户类型,目前仅支持个人账户
        /// </summary>
        public string accounttype { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string bankaccount { get; set; }
        /// <summary>
        /// 证件类型1  身份证
        /// </summary>
        public string certificatetype { get; set; }
        /// <summary>
        /// 证件号
        /// </summary>
        public string certificateno { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobilephone { get; set; }
        /// <summary>
        /// 类型1-快捷支付
        /// </summary>
        public string collecttype { get; set; }
        /// <summary>
        /// 协议生效日期yyyyMMdd
        /// </summary>
        public string startdate { get; set; }
        /// <summary>
        /// 协议生效日期yyyyMMdd
        /// </summary>
        public string enddate { get; set; }
        /// <summary>
        /// 开户行联行号 如: 308581002353
        /// </summary>
        public string bankcode { get; set; }
        /// <summary>
        /// 开户行名称 如:招商银行股份有限公司广州丰兴支行
        /// </summary>
        public string bank { get; set; }
        /// <summary>
        /// 信用卡背面cvv2码后三位
        /// </summary>
        public string cvv2 { get; set; }
        /// <summary>
        /// 有效期，月年，四位数，例：2112
        /// </summary>
        public string expirationdate { get; set; }
        /// <summary>
        /// 开通代扣协议前台地址
        /// </summary>
        public string fronturl { get; set; }
        /// <summary>
        /// 开通代扣协议后台地址
        /// </summary>
        public string backurl { get; set; }
    }

    public class respTreatyApplyModel
    {
        public string html { get; set; }
        public string treatycode { get; set; }
        public string ishtml { get; set; }
        public string bankaccount { get; set; }
        public string smsseq { get; set; }
    }
    #endregion

    #region 确认开通快捷协议 masget.pay.collect.router.treaty.confirm
    public class reqTreatyConfirmModel
    {
        public string treatycode { get; set; }
        public string smsseq { get; set; }
        public string authcode { get; set; }
    }

    public class respTreatyConfirmModel
    {
        public string accountname { get; set; }
        public string bankaccount { get; set; }
        public string treatycode { get; set; }
    }
    #endregion

    #region 查询快捷协议 masget.pay.collect.router.treaty.query
    public class reqTreatyQueryModel
    {
        /// <summary>
        /// 上级商户id
        /// </summary>
        public string pcompanyid { get; set; }
        /// <summary>
        /// 商户id
        /// </summary>
        public string companyid { get; set; }
        /// <summary>
        /// 协议编号
        /// </summary>
        public string treatycode { get; set; }
        /// <summary>
        /// 卡号(协议号和卡号不能同时为空)
        /// </summary>
        public string bankaccount { get; set; }


    }

    public class respTreatyQueryModel
    {
        /// <summary>
        /// 协议编号
        /// </summary>
        public string treatycode { get; set; }
        /// <summary>
        /// 账户名
        /// </summary>
        public string accountname { get; set; }
        /// <summary>
        /// 账户类型,目前仅支持个人账户
        /// 0-企业账户
        /// 1-个人账户
        /// </summary>
        public string accounttype { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string bankaccount { get; set; }
        /// <summary>
        /// 银行卡类型,默认1
        /// 1=储蓄卡
        /// 2=信用卡
        /// </summary>
        public string bankcardtype { get; set; }
        /// <summary>
        /// 证件类型
        /// 可选，
        /// 1  身份证
        /// 2  军官证
        /// 3  护照
        /// 4  回乡证
        /// 5  台胞证
        /// 6  警官证
        /// 7  士兵证
        /// 99  其它证件
        /// </summary>
        public string certificatetype { get; set; }
        /// <summary>
        /// 证件号
        /// </summary>
        public string certificateno { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobilephone { get; set; }
        /// <summary>
        /// 协议生效日期yyyyMMdd
        /// </summary>
        public string startdate { get; set; }
        /// <summary>
        /// 协议生效日期yyyyMMdd
        /// </summary>
        public string enddate { get; set; }
        /// <summary>
        /// 银行编码
        /// </summary>
        public string hbankcode { get; set; }
        /// <summary>
        /// 银行名称，如:招商银行
        /// </summary>
        public string hbankname { get; set; }
        /// <summary>
        /// 开户行联行号 如: 308581002353
        /// </summary>
        public string bankcode { get; set; }
        /// <summary>
        /// 开户行名称 如:招商银行股份有限公司广州丰兴支行
        /// </summary>
        public string bank { get; set; }
    }
    #endregion

    #region 订单支付(后台) masget.pay.compay.router.back.pay

    public class reqBackPayModel
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string ordernumber { get; set; }
        /// <summary>
        /// 订单描述
        /// </summary>
        public string body { get; set; }
        /// <summary>
        /// 交易金额精确到分(100=1元)
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 交易类型1001-销售收款
        /// </summary>
        public string businesstype { get; set; }
        /// <summary>
        /// 支付方式 25-快捷支付
        /// </summary>
        public string paymenttypeid { get; set; }
        /// <summary>
        /// 前台通知地址
        /// </summary>
        public string fronturl { get; set; }
        /// <summary>
        /// 后台通知地址
        /// </summary>
        public string backurl { get; set; }
        /// <summary>
        /// 附加参数，支付报告中会原样返回
        /// </summary>
        public string extraparams { get; set; }
        /// <summary>
        /// 支付参数，根据具体支付方式决定
        /// </summary>
        public string payextraparams { get; set; }
    }
    public class PayExtraParamsModel
    {
        /// <summary>
        /// 支付密码（根据实际约定）
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 短信验证码（根据实际约定）
        /// </summary>
        public string authcode { get; set; }
        /// <summary>
        /// 代扣协议
        /// </summary>
        public string treatycode { get; set; }
    }

    public class respBackPayModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int ishtml { get; set; }
        /// <summary>
        /// 支付流水号
        /// </summary>
        public string payorderid { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        public long companyid { get; set; }
        /// <summary>
        /// 订单唯一码
        /// </summary>
        public string ordercode { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string ordernumber { get; set; }
        /// <summary>
        /// 订单描述
        /// </summary>
        public string body { get; set; }
        /// <summary>
        /// 发送短信确认标志
        /// 1-需要调取1.5.3
        /// 确认支付
        /// 2-不需要确认支付
        /// </summary>
        public string sendmessage { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public int paymenttypeid { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public string amount { get; set; }
    }


    #endregion

    #region 查询交易订单 masget.pay.compay.router.paymentjournal.get

    public class reqPaymentjournalGetModel
    {
        /// <summary>
        /// 公司id
        /// </summary>
        public string companyid { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string ordernumber { get; set; }
    }
    public class respPaymentjournalGetModel
    {
        /// <summary>
        /// 公司id
        /// </summary>
        public int companyid { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string companyname { get; set; }
        /// <summary>
        /// 子商户id
        /// </summary>
        public int subcompanyid { get; set; }
        /// <summary>
        /// 子商户名称
        /// </summary>
        public string subcompanyname { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string ordernumber { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public int businesstype { get; set; }
        /// <summary>
        /// 支付方式id
        /// </summary>
        public int paymenttypeid { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string paymenttypename { get; set; }
        /// <summary>
        /// 订单描述
        /// </summary>
        public string body { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public int amount { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public int refundamount { get; set; }
        /// <summary>
        /// 交易流水号
        /// </summary>
        public string payorderid { get; set; }
        /// <summary>
        /// 交易时间yyyy-MM-dd hh:mm:ss
        /// </summary>
        public string businesstime { get; set; }
        /// <summary>
        /// 通道订单号
        /// </summary>
        public string channelordernumber { get; set; }
        /// <summary>
        /// 通道流水号
        /// </summary>
        public string channelpayorderid { get; set; }
        /// <summary>
        /// 交易状态
        /// 1-待支付
        /// 2-支付完成
        /// 3-已关闭
        /// 4-交易撤销
        /// 5-交易已受理,待确认
        /// 6-交易失败7-已退款
        /// </summary>
        public int respcode { get; set; }
        /// <summary>
        /// 状态说明
        /// </summary>
        public string respmsg { get; set; }
    }

    #endregion

    #region 确认支付 masget.pay.compay.router.confirmpay

    public class reqPayConfirmpayModel
    {
        /// <summary>
        /// 短信验证码
        /// </summary>
        public string authcode { get; set; }
        /// <summary>
        /// 订单唯一码
        /// </summary>
        public string ordercode { get; set; }
    }

    public class respPayConfirmpayModel
    {
        /// <summary>
        /// 公司id
        /// </summary>
        public int companyid { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string companyname { get; set; }
        /// <summary>
        /// 子商户id
        /// </summary>
        public int subcompanyid { get; set; }
        /// <summary>
        /// 子商户名称
        /// </summary>
        public string subcompanyname { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string ordernumber { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public int businesstype { get; set; }
        /// <summary>
        /// 支付方式id
        /// </summary>
        public int paymenttypeid { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string paymenttypename { get; set; }
        /// <summary>
        /// 订单描述
        /// </summary>
        public string body { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public int amount { get; set; }
        /// <summary>
        /// 交易流水号
        /// </summary>
        public string payorderid { get; set; }
        /// <summary>
        /// 交易时间yyyy-MM-dd hh:mm:ss
        /// </summary>
        public string businesstime { get; set; }
        /// <summary>
        /// 交易状态
        /// 1-待支付
        /// 2-支付完成
        /// 3-已关闭
        /// 4-交易撤销
        /// 5-交易已受理,待确认
        /// 6-交易失败7-已退款
        /// </summary>
        public int respcode { get; set; }
        /// <summary>
        /// 状态说明
        /// </summary>
        public string respmsg { get; set; }
    }

    public class noticePayConfirmpayModel
    {
        /// <summary>
        /// 公司id
        /// </summary>
        public int companyid { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string companyname { get; set; }
        /// <summary>
        /// 子商户id
        /// </summary>
        public string subcompanyid { get; set; }
        /// <summary>
        /// 子商户名称
        /// </summary>
        public string subcompanyname { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string ordernumber { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string businesstype { get; set; }
        /// <summary>
        /// 支付方式id
        /// </summary>
        public string paymenttypeid { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string paymenttypename { get; set; }
        /// <summary>
        /// 订单描述
        /// </summary>
        public string body { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 交易流水号
        /// </summary>
        public string payorderid { get; set; }
        /// <summary>
        /// 交易时间yyyy-MM-dd hh:mm:ss
        /// </summary>
        public string businesstime { get; set; }
        /// <summary>
        /// 交易状态
        /// 1-待支付
        /// 2-支付完成
        /// 3-已关闭
        /// 4-交易撤销
        /// </summary>
        public string respcode { get; set; }
        /// <summary>
        /// 通道订单号
        /// </summary>
        public string channelordernumber { get; set; }
        /// <summary>
        /// 通道流水号
        /// </summary>
        public string channelpayorderid { get; set; }
        /// <summary>
        /// 当线下刷卡paymenttypeid=2时，刷卡卡号
        /// 采用前6后4中间用* 号屏蔽
        /// (刷卡交易成功时此字段必填)
        /// </summary>
        public string bankaccount { get; set; }
        /// <summary>
        /// 1-储蓄卡 2-信用卡
        /// (刷卡交易成功时此字段必填)
        /// </summary>
        public string bankcardtype { get; set; }
        /// <summary>
        /// 商户号
        /// (刷卡交易成功时此字段必填)
        /// </summary>
        public string merchantid { get; set; }
        /// <summary>
        /// 终端号(刷卡交易成功时此字段必填)
        /// </summary>
        public string terminalnumber { get; set; }
        /// <summary>
        /// 状态说明
        /// </summary>
        public string respmsg { get; set; }
    }
    #endregion

    #region 修改同名进出商户费率 masget.pay.compay.router.samename.update
    public class reqSamenameUpdateModel
    {
        /// <summary>
        /// 公司id
        /// </summary>
        public string companyid { get; set; }
        /// <summary>
        /// 费率套餐编码
        /// </summary>
        public string ratecode { get; set; }
        /// <summary>
        /// 支付方式(详情查看订单支付接口)
        /// </summary>
        public string paymenttypeid { get; set; }
        /// <summary>
        /// 子支付方式(详情查看订单支付接口)
        /// </summary>
        public string subpaymenttypeid { get; set; }
    }
    #endregion
}
