using ITOrm.Host.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Payment.Yeepay
{

    #region 子商户注册
    public class reqRegisterModel : reqModel
    {

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string mailStr { get; set; }
        /// <summary>
        /// 注册请求号
        /// </summary>
        public string requestId { get; set; }
        /// <summary>
        /// 商户类型
        /// </summary>
        public string customerType { get; set; }
        /// <summary>
        /// 营业执照号
        /// </summary>
        public string businessLicence { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string bindMobile { get; set; }
        /// <summary>
        /// 签约名
        /// </summary>
        public string signedName { get; set; }
        /// <summary>
        /// 推荐人
        /// </summary>
        public string linkMan { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string idCard { get; set; }
        /// <summary>
        /// 法人姓名
        /// </summary>
        public string legalPerson { get; set; }
        /// <summary>
        /// 起结金额
        /// </summary>
        public string minSettleAmount { get; set; }
        /// <summary>
        /// 结算周期
        /// </summary>
        public string riskReserveDay { get; set; }
        /// <summary>
        /// 银行卡类型
        /// </summary>
        public string bankAccountType { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string bankAccountNumber { get; set; }
        /// <summary>
        /// 银行卡开户行
        /// </summary>
        public string bankName { get; set; }
        /// <summary>
        /// 开户名
        /// </summary>
        public string accountName { get; set; }
        /// <summary>
        /// 地区码
        /// </summary>
        public string areaCode { get; set; }
        /// <summary>
        /// 是否自助结算
        /// </summary>
        public string manualSettle { get; set; }
        /// <summary>
        /// 银行卡正面照
        /// </summary>
        public string bankCardPhoto { get; set; }
        /// <summary>
        /// 个体工商户正面照
        /// </summary>
        public string businessLicensePhoto { get; set; }
        /// <summary>
        /// 身份证正面照
        /// </summary>
        public string idCardPhoto { get; set; }
        /// <summary>
        /// 身份证背面照
        /// </summary>
        public string idCardBackPhoto { get; set; }
        /// <summary>
        /// 身份证+银行卡+本人合照
        /// </summary>
        public string personPhoto { get; set; }
        /// <summary>
        /// 电子协议
        /// </summary>
        public string electronicAgreement { get; set; }
    }

    public class respRegisterModel : respModel
    {
        public string customerNumber { get; set; }
    }
    #endregion

    #region 子商户审核接口

    public class reqAuditMerchantModel : reqModel
    {
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 审核状态  审核状态，填写大写英文字母：
        /// FAILED：审核不通过SUCCESS：审核通过
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 审核原因
        /// （1）审核操作的备注；
        /// （2）审核状态为FAILED时该参数必填。
        /// </summary>
        public string reason { get; set; }
    }

    public class respAuditMerchantModel : respModel
    {
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 代理商编号
        /// </summary>
        public string mainCustomerNumber { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 审核原因
        /// </summary>
        public string reason { get; set; }
    }
    #endregion

    #region 子商户信息查询接口 customerInforQuery
    public class reqCustomerInforQueryModel : reqModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobilePhone { get; set; }
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 商户类型
        /// </summary>
        public string customerType { get; set; }
    }

    public class respCustomerInforQueryModel : respModel
    {
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerType { get; set; }
        /// <summary>
        /// 信息集合
        /// </summary>
        public List<RetList> retList { get; set; }
        
    }

    public class RetList
    {
        /// <summary>
        /// 审核状态
        /// </summary>
        public string auditStatus { get; set; }
        /// <summary>
        /// 审核原因
        /// </summary>
        public string auditMessage { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string bankAccountNumber { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string bankName { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public string createTime { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string idCard { get; set; }
        /// <summary>
        /// 商户编号
        /// </summary>
        public string ledgeCustomerNumber { get; set; }
        /// <summary>
        /// 推荐人
        /// </summary>
        public string linkMan { get; set; }
        /// <summary>
        /// 结算周期
        /// </summary>
        public string riskReserverDay { get; set; }
        /// <summary>
        /// 签约名
        /// </summary>
        public string signName { get; set; }
        /// <summary>
        /// 分润方
        /// </summary>
        public string splitter { get; set; }
        /// <summary>
        /// 分润比例
        /// </summary>
        public string splitterProfitFee { get; set; }
        /// <summary>
        /// 是否为白名单用户
        /// </summary>
        public string whiteList { get; set; }
        /// <summary>
        /// 冻结天数
        /// </summary>
        public string freezeDays { get; set; }
        /// <summary>
        /// 是否自助结算
        /// </summary>
        public string manualSettle { get; set; }
        /// <summary>
        /// 应收认证费
        /// </summary>
        public string certFee { get; set; }
        /// <summary>
        /// 未收认证费
        /// </summary>
        public string noPaidCertFee { get; set; }
        /// <summary>
        /// 银行卡审核状态
        /// </summary>
        public string bankCardUpdateStatus { get; set; }
        /// <summary>
        /// 银行卡审核原因
        /// </summary>
        public string bankCardUpdateReason { get; set; }
    }
    #endregion

    #region  子商户信息修改接口 customerInforUpdate
    public class reqCustomerInforUpdateModel : reqModel
    {

        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 修改类型
        /// 子商户修改的类型，传数字符号：
        /// 2：银行卡信息
        /// 3：结算信息
        /// 4：分润信息
        /// 5：开通扫码支付（2016年7月25日已关闭）
        /// 6：子商户基本信息
        /// </summary>
        public string modifyType { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string bankCardNumber { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string bankName { get; set; }
        /// <summary>
        /// 结算周期
        /// </summary>
        public string riskReserveDay { get; set; }
        /// <summary>
        /// 是否自助结算
        /// </summary>
        public string manualSettle { get; set; }
        /// <summary>
        /// 分润方
        /// </summary>
        public string splitter { get; set; }
        /// <summary>
        /// 分润比率
        /// </summary>
        public string splitterprofitfee { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string bindMobile { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string mailStr { get; set; }
        /// <summary>
        /// 地区码
        /// </summary>
        public string areaCode { get; set; }
    }

    public class respCustomerInforUpdate2Model : respModel
    {
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string bankCardNumber { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string bankName { get; set; }

    }
    public class respCustomerInforUpdate3Model : respModel
    {
        /// <summary>
        /// 是否自助结算
        /// </summary>
        public string manualSettle { get; set; }
        /// <summary>
        /// 结算周期
        /// </summary>
        public string riskReserveDay { get; set; }

    }

    public class respCustomerInforUpdate4Model : respModel
    {
        /// <summary>
        /// 分润方
        /// </summary>
        public string splitter { get; set; }
        /// <summary>
        /// 分润比率
        /// </summary>
        public string splitterprofitfee { get; set; }

    }

    public class respCustomerInforUpdate6Model : respModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string bindMobile { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string mailStr { get; set; }
        /// <summary>
        /// 地区码
        /// </summary>
        public string areaCode { get; set; }

    }

    #endregion

    #region 子商户费率查询接口 queryFeeSetApi
    public class reqQueryFeeSetApiModel : reqModel
    {

        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        public string productType { get; set; }
    }

    public class respQueryFeeSetApiModel : respModel
    {
        /// <summary>
        /// 费率
        /// </summary>
        public string rate { get; set; }
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        public string productType { get; set; }

    }
    #endregion

    #region 子商户限额查询接口 tradeLimitQuery

    public class reqTradeLimitQueryModel : reqModel
    {
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 限额类型
        /// </summary>
        public string tradeLimitConfigKey { get; set; }
        /// <summary>
        /// 银行卡类型
        /// </summary>
        public string bankCardType { get; set; }
        /// <summary>
        /// 银行卡卡号
        /// </summary>
        public string bankCardNo { get; set; }

    }

    public class respTradeLimitQueryModel : respModel
    {
        /// <summary>
        /// 单笔限额
        /// </summary>
        public string singleAmount { get; set; }
        /// <summary>
        /// 日限额
        /// </summary>
        public string dayAmount { get; set; }
        /// <summary>
        /// 月限额
        /// </summary>
        public string monthAmount { get; set; }
        /// <summary>
        /// 日累计次数
        /// </summary>
        public string dayCount { get; set; }
        /// <summary>
        /// 月累计次数
        /// </summary>
        public string monthCount { get; set; }
    }

    #endregion

    #region 子商户费率设置
    public class reqFeeSetApiModel : reqModel
    {
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        public string productType { get; set; }
        /// <summary>
        /// 费率
        /// </summary>
        public string rate { get; set; }
    }

    public class respFeeSetApiModel : respModel
    {
        /// <summary>
        /// 费率
        /// </summary>
        public string rate { get; set; }
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        public string productType { get; set; }
    }
    #endregion

    #region 收款接口（交易接口） receiveApi

    public class reqReceiveApiModel : reqModel
    {
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 收款订单号
        /// </summary>
        public string requestId { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 商品分类
        /// </summary>
        public string mcc { get; set; }
        /// <summary>
        /// 收款成功回调地址
        /// </summary>
        public string callBackUrl { get; set; }
        /// <summary>
        /// 页面回调地址
        /// </summary>
        public string webCallBackUrl { get; set; }
        /// <summary>
        /// 支付卡号
        /// </summary>
        public string payerBankAccountNo { get; set; }
        /// <summary>
        /// 是否逐笔结算
        /// </summary>
        public string autoWithdraw { get; set; }
        /// <summary>
        /// 提现卡号
        /// </summary>
        public string withdrawCardNo { get; set; }
        /// <summary>
        /// 逐笔结算回调地址
        /// </summary>
        public string withdrawCallBackUrl { get; set; }
    }

    public class respReceiveApiModel : respModel
    {
        /// <summary>
        /// 代理商编号
        /// </summary>
        public string mainCustomerNumber { get; set; }
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 收款URL地址
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 解密后的地址
        /// </summary>
        public string urlAES
        {
            get
            {
                return ITOrm.Utility.Encryption.AESEncrypter.AESDecrypt(url, YeepayDepository.YeepayHmacKey.Substring(0, 16));
            }
        }
        /// <summary>
        /// 收款订单号
        /// </summary>
        public string requestId { get; set; }
    }

    public class noticeReceiveApiModel : noticeModel
    {
        /// <summary>
        /// 收款订单号
        /// </summary>
        public string requestId { get; set; }
        /// <summary>
        /// 收款宝交易流水号
        /// </summary>
        public string externalld { get; set; }
        /// <summary>
        /// 收款请求时间
        /// </summary>
        public string createTime { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public string payTime { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public string fee { get; set; }
        /// <summary>
        /// 订单状态  订单状态：INIT：未支付SUCCESS：成功FAIL：失败FROZEN：冻结THAWED：解冻REVERSE：冲正
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 订单类型  COMMON ：普通交易ASSURE：担保交易（已废弃）
        /// </summary>
        public string busiType { get; set; }
        /// <summary>
        /// 银行编码
        /// </summary>
        public string bankCode { get; set; }
        /// <summary>
        /// 持卡人姓名
        /// </summary>
        public string payerName { get; set; }
        /// <summary>
        /// 银行预留手机号
        /// </summary>
        public string payerPhone { get; set; }
        /// <summary>
        /// 银行卡后四位
        /// </summary>
        public string lastNo { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string src { get; set; }
    }

    #endregion

    #region 结算接口

    public class reqWithDrawApiModel : reqModel
    {
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 结算请求号
        /// </summary>
        public string externalNo { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        public string transferWay { get; set; }
        /// <summary>
        /// 结算金额
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 结算回调地址
        /// </summary>
        public string callBackUrl { get; set; }
    }

    public class respWithDrawApiModel : respModel
    {
        /// <summary>
        /// 收款宝流水号
        /// </summary>
        public string serialNo { get; set; }
        /// <summary>
        /// 结算金额
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 代理商编号
        /// </summary>
        public string mainCustomerNumber { get; set; }
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 结算请求号
        /// </summary>
        public string externalNo { get; set; }
        /// <summary>
        /// 结算方式   结算方式：1：T0自助结算2：T1自助结算
        /// </summary>
        public string transferWay { get; set; }
    }

    public class noticeWithDrawApiModel : noticeModel
    {
        /// <summary>
        /// 结算请求号
        /// </summary>
        public string externalNo { get; set; }
        /// <summary>
        /// 收款宝流水号
        /// </summary>
        public string serialNo { get; set; }
        /// <summary>
        /// 结算状态 
        /// RECEIVED  ：已接受
        /// PROCESSING：处理中
        /// SUCCESSED：打款成功
        /// FAILED：打款失败
        /// REFUNED：已退款
        /// CANCELLED：已撤销
        /// </summary>
        public string transferStatus { get; set; }
        /// <summary>
        /// 请求时间
        /// </summary>
        public string requestTime { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public string handleTime { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        public string transferWay { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        public string receiver { get; set; }
        /// <summary>
        /// 收款卡号
        /// </summary>
        public string receiverBankCardNo { get; set; }
        /// <summary>
        /// 收款银行
        /// </summary>
        public string receiverBank { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public string fee { get; set; }
        /// <summary>
        /// 基本手续费
        /// </summary>
        public string basicFee { get; set; }
        /// <summary>
        /// 额外手续费
        /// </summary>
        public string exTargetFee { get; set; }
        /// <summary>
        /// 实收金额
        /// </summary>
        public string actualAmount { get; set; }
        /// <summary>
        /// 失败原因
        /// </summary>
        public string failReason { get; set; }
    }

    #endregion

    #region 交易查询接口 tradeReviceQuery
    public class reqTradeReviceQueryModel : reqModel
    {
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 收款订单号
        /// </summary>
        public string requestId { get; set; }
        /// <summary>
        /// 请求时间：开始时间
        /// </summary>
        public string createTimeBegin { get; set; }
        /// <summary>
        /// 请求时间：结束时间
        /// </summary>
        public string createTimeEnd { get; set; }
        /// <summary>
        /// 支付时间：开始时间
        /// </summary>
        public string payTimeBegin { get; set; }
        /// <summary>
        /// 支付时间：结束时间
        /// </summary>
        public string payTimeEnd { get; set; }
        /// <summary>
        /// 更新时间：开始时间
        /// </summary>
        public string lastUpdateTimeBegin { get; set; }
        /// <summary>
        /// 更新时间：结束时间
        /// </summary>
        public string lastUpdateTimeEnd { get; set; }
        /// <summary>
        /// 订单状态 
        /// INIT：未支付
        /// SUCCESS：成功
        /// FAIL：失败
        /// FROZEN：冻结
        /// THAWED：解冻
        /// REVERSE：冲正
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 订单类型
        /// COMMON：普通交易
        /// ASSURE：担保交易（已弃用）
        /// </summary>
        public string busiType { get; set; }
        /// <summary>
        /// 分页参数：页码  
        /// （1）订单列表的第几页，必须是正整数；
        /// （2）每页显示20条数据。
        /// </summary>
        public string pageNo { get; set; }
    }

    public class respTradeReviceQueryModel : respModel
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public string totalRecords { get; set; }
        /// <summary>
        /// 交易订单列表
        /// </summary>
        public List<tradeReceivesModel> tradeReceives { get; set; }
    }

    public class tradeReceivesModel
    {
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 收款订单号
        /// </summary>
        public string requestId { get; set; }
        /// <summary>
        /// 收款宝流水号
        /// </summary>
        public string externalId { get; set; }
        /// <summary>
        /// 请求时间
        /// </summary>
        public string createTime { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public string payTime { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public string fee { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public string busiType { get; set; }
        /// <summary>
        /// 银行编码
        /// </summary>
        public string bankCode { get; set; }
        /// <summary>
        /// 持卡人姓名
        /// </summary>
        public string payerName { get; set; }
        /// <summary>
        /// 银行预留手机号
        /// </summary>
        public string payerPhone { get; set; }
        /// <summary>
        /// 银行卡后四位
        /// </summary>
        public string lastNo { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string src { get; set; }
        /// <summary>
        /// 结算状态
        /// </summary>
        public string withdrawStatus { get; set; }
    }
    #endregion

    #region 可用余额查询接口 customerBalanceQuery

    public class reqCustomerBalanceQueryModel : reqModel
    {
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 可用余额类型
        /// </summary>
        public string balanceType { get; set; }
    }

    public class respCustomerBalanceQueryModel : respModel
    {
        /// <summary>
        /// 代理商编号
        /// </summary>
        public string mainCustomerNumber { get; set; }
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 可用余额类型
        /// </summary>
        public string balanceType { get; set; }
        /// <summary>
        /// 可用余额
        /// </summary>
        public string balance { get; set; }
    }

    #endregion

    #region 结算记录查询接口 transferQuery
    public class reqTransferQueryModel : reqModel
    {
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 结算请求号
        /// </summary>
        public string externalNo { get; set; }
        /// <summary>
        /// 收款宝流水号
        /// </summary>
        public string serialNo { get; set; }
        /// <summary>
        /// 请求时间：开始时间
        /// </summary>
        public string requestDateSectionBegin { get; set; }
        /// <summary>
        /// 请求时间：结束时间
        /// </summary>
        public string requestDateSectionEnd { get; set; }
        /// <summary>
        /// 结算状态
        /// RECEIVED：已接受
        /// PROCESSING：处理中
        /// SUCCESSED：打款成功
        /// FAILED：打款失败
        /// REFUNED：已退款
        /// CANCELLED：已撤销
        /// </summary>
        public string transferStatus { get; set; }
        /// <summary>
        /// 结算方式
        /// 1：T0自助结算
        /// 2：T1自助结算
        /// 3：T1自动结算
        /// </summary>
        public string transferWay { get; set; }
        /// <summary>
        /// 分页参数：页码  
        /// （1）订单列表的第几页，必须是正整数；
        /// （2）每页显示20条数据。
        /// </summary>
        public string pageNo { get; set; }
    }

    public class respTransferQueryModel : respModel
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public string totalRecords { get; set; }
        /// <summary>
        /// 结算记录列表
        /// </summary>transferRequests   
        ///           transferRequsts
        public List<transferRequstsModel> transferRequests { get; set; }
    }

    public class transferRequstsModel
    {
        /// <summary>
        /// 结算流水号
        /// </summary>
        public string batchNo { get; set; }
        /// <summary>
        /// 结算状态
        /// </summary>
        public string transferStatus { get; set; }
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 结算请求号
        /// </summary>
        public string requestNo { get; set; }
        /// <summary>
        /// 请求时间
        /// </summary>
        public string requestTime { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public string handleTime { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        public string receiver { get; set; }
        /// <summary>
        /// 收款卡号
        /// </summary>
        public string receiverBankCardNo { get; set; }
        /// <summary>
        /// 收款银行
        /// </summary>
        public string receiverBank { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public string fee { get; set; }
        /// <summary>
        /// 基本手续费
        /// </summary>
        public string basicFee { get; set; }
        /// <summary>
        /// 额外手续费
        /// </summary>
        public string extTargetFee { get; set; }
        /// <summary>
        /// 实收金额
        /// </summary>
        public string actualAmount { get; set; }
        /// <summary>
        /// 失败原因
        /// </summary>
        public string failReason { get; set; }
    }
    #endregion

    #region 结算手续费查询接口 lendTargetFeeQuery

    public class reqLendTargetFeeQueryModel : reqModel
    {
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        public string transType { get; set; }
        /// <summary>
        /// 结算金额
        /// </summary>
        public string transAmount { get; set; }
    }

    public class respLendTargetFeeQueryModel : respModel
    {
        /// <summary>
        /// 代理商商户编号
        /// </summary>
        public string mainCustomerNumber { get; set; }
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        public string transType { get; set; }
        /// <summary>
        /// 结算金额
        /// </summary>
        public string transAmount { get; set; }
        /// <summary>
        /// 查询时间
        /// </summary>
        public string queryTime { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public string targetFee { get; set; }
    }

    #endregion

    #region 垫资额度查询接口 queryRJTBalance
    public class reqQueryRJTBalanceModel : reqModel
    {

    }

    public class respQueryRJTBalanceModel : respModel
    {
        /// <summary>
        /// 代理商固定垫资额度
        /// </summary>
        public string fixedLimit { get; set; }
        /// <summary>
        /// 代理商临时垫资额度
        /// </summary>
        public string tempLimit { get; set; }
        /// <summary>
        /// T0可用总额度
        /// </summary>
        public string totalLimit { get; set; }
        /// <summary>
        /// 垫资配比
        /// </summary>
        public string percentage { get; set; }
        /// <summary>
        /// 剩余额度
        /// </summary>
        public string leftLimit { get; set; }
        /// <summary>
        /// 查询时间
        /// </summary>
        public string currentDate { get; set; }
    }

    #endregion


    #region 代理商返佣转账接口 transferToCustomer

    public class reqTransferToCustomerModel : reqModel
    {
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 转账订单号
        /// </summary>
        public string requestId { get; set; }
        /// <summary>
        /// 转账金额
        /// </summary>
        public string transAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

    }
    public class respTransferToCustomerModel : respModel
    {
        /// <summary>
        /// 代理商商户编号
        /// </summary>
        public string mainCustomerNumber { get; set; }
        /// <summary>
        /// 子商户编号
        /// </summary>
        public string customerNumber { get; set; }
        /// <summary>
        /// 转账订单号
        /// </summary>
        public string requestId { get; set; }
        /// <summary>
        /// 转账金额
        /// </summary>
        public string transAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
    }
    #endregion

    #region 通用基类


    /// <summary>
    /// 通用返回状态
    /// </summary>
    public class respModel
    {
        /// <summary>
        /// 返回码   0000 代表成功
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 说明信息  
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 签名信息
        /// </summary>
        public string hmac { get; set; }


        private int _backState;
        /// <summary>
        /// 是否成功状态
        /// </summary>
        public int backState { get {

                if (string.IsNullOrEmpty(code))
                {
                    return _backState;
                }
                return code == "0000" ? 0 : -100; }
            set { _backState = value; }
        }
    }

    public class reqModel
    {
        /// <summary>
        /// 代理商商户编号
        /// </summary>
        public string mainCustomerNumber { get { return YeepayDepository.YeepayMainCustomerNumber; } }
        /// <summary>
        /// 签名信息
        /// </summary>
        public string hmac { get; set; }
    }

    public class noticeModel
    {
        public string mainCustomerNumber { get; set; }
        public string customerNumber { get; set; }
        public string hmac { get; set; }
        /// <summary>
        /// 返回码   0000 代表成功
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 说明信息  
        /// </summary>
        public string message { get; set; }

        public int backState { get { return code == "0000" ? 0 : -100; } }

    }


    #endregion
}

