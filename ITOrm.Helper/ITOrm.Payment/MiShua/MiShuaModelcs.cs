using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace ITOrm.Payment.MiShua
{

    #region 公共基类
    public class reqModel
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string mchNo { get; set; }
        /// <summary>
        /// 具体报文体
        /// </summary>
        public string payload { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }
    }

    public class respModel<T>
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string mchNo { get; set; }
        /// <summary>
        /// 请求状态
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 错误描述
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 错误码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 具体报文体
        /// </summary>
        public string payload { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }

        public T Data { get {
                if (code == "0")
                {
                    string json = ITOrm.Payment.MiShua.AES.Decrypt(payload,MiShuaDepository.AESkey, "0102030405060708");
                    return JsonConvert.DeserializeObject<T>(json);
                }
                return default(T);
            } }

        private int _backState;
        public int backState { get {

                if (!string.IsNullOrEmpty(code))
                {
                    return Convert.ToInt32(code);
                }
                else
                {
                    return _backState;
                }
            }

            set { value = _backState; }
        }
    }

    public class noticeMiShuaModel
    {
        /// <summary>
        /// 接口版本号
        /// </summary>
        public string versionNo { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string mchNo { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 订单描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 订单日期
        /// </summary>
        public string orderDate { get; set; }
        /// <summary>
        /// 商户流水号
        /// </summary>
        public string tradeNo { get; set; }
        /// <summary>
        /// 米刷流水号
        /// </summary>
        public string transNo { get; set; }
        /// <summary>
        /// 交易结果
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 交易结果描述
        /// </summary>
        public string statusDesc { get; set; }
        /// <summary>
        /// 代付结果
        /// </summary>
        public string qfStatus { get; set; }
    }
    #endregion

    #region 支付接口
    public class reqPayDzeroModel
    {
        /// <summary>
        /// 接口版本号
        /// </summary>
        public string versionNo { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string mchNo { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 订单描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 订单日期
        /// </summary>
        public string orderDate { get; set; }
        /// <summary>
        /// 商户流水号
        /// </summary>
        public string tradeNo { get; set; }
        /// <summary>
        /// 异步通知URL
        /// </summary>
        public string notifyUrl { get; set; }
        /// <summary>
        /// 为成功支付后页面会跳地址
        /// </summary>
        public string callbackUrl { get; set; }
        /// <summary>
        /// 支付卡卡号
        /// </summary>
        public string payCardNo { get; set; }
        /// <summary>
        /// 支付卡手机号
        /// </summary>
        public string userToken { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string accTel { get; set; }
        /// <summary>
        /// 持卡人姓名
        /// </summary>
        public string accName { get; set; }
        /// <summary>
        /// 持卡人身份证
        /// </summary>
        public string accIdCard { get; set; }
        /// <summary>
        /// 结算卡开户行
        /// </summary>
        public string bankName { get; set; }
        /// <summary>
        /// 结算卡开户行联行号
        /// </summary>
        public string bankCode { get; set; }
        /// <summary>
        /// 结算卡卡号
        /// </summary>
        public string cardNo { get; set; }
        /// <summary>
        /// 结算费率
        /// </summary>
        public string downPayFee { get; set; }
        /// <summary>
        /// 代付费
        /// </summary>
        public string downDrawFee { get; set; }
    }


    public class respPayDzeroModel
    {
        /// <summary>
        /// 接口版本号
        /// </summary>
        public string versionNo { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string mchNo { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 订单描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 订单日期
        /// </summary>
        public string orderDate { get; set; }
        /// <summary>
        /// 商户流水号
        /// </summary>
        public string tradeNo { get; set; }
        /// <summary>
        /// 米刷流水号
        /// </summary>
        public string transNo { get; set; }
        /// <summary>
        /// 返回结果
        /// </summary>
        public string tranStr { get; set; }
        /// <summary>
        /// 下单结果
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 下单结果描述
        /// </summary>
        public string statusDesc { get; set; }
        /// <summary>
        /// 结算金额
        /// </summary>
        public string downRealPrice { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public string profit { get; set; }
    }
    #endregion

    #region 无卡查询接口

    public class reqCheckDzeroModel
    {
        /// <summary>
        /// 接口版本号
        /// </summary>
        public string versionNo { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string mchNo { get; set; }
        /// <summary>
        /// 商户流水号
        /// </summary>
        public string tradeNo { get; set; }
        /// <summary>
        /// 米刷流水号
        /// </summary>
        public string transNo { get; set; }
    }

    public class respCheckDzeroModel
    {
        /// <summary>
        /// 接口版本号
        /// </summary>
        public string versionNo { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string mchNo { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 订单描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 订单日期
        /// </summary>
        public string orderDate { get; set; }
        /// <summary>
        /// 商户流水号
        /// </summary>
        public string tradeNo { get; set; }
        /// <summary>
        /// 米刷流水号
        /// </summary>
        public string transNo { get; set; }
        /// <summary>
        /// 返回结果
        /// </summary>
        public string tranStr { get; set; }
        /// <summary>
        /// 下单结果
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 下单结果描述
        /// </summary>
        public string statusDesc { get; set; }
        /// <summary>
        /// 结算金额
        /// </summary>
        public string downRealPrice { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public string profit { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public string qfStatus { get; set; }
    }
    #endregion
}
