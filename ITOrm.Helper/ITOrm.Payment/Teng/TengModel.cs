using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Payment.Teng
{

    #region 公共对象
    public class reqTengModel
    {
        /// <summary>
        /// 接口地址
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 版本号 
        /// </summary>
        public string version { get { return "1.0.0"; } }
        /// <summary>
        /// 代理商号
        /// </summary>
        public string agentId { get { return "A1000000031"; } }
        /// <summary>
        /// 商户代码 
        /// </summary>
        public string merId { get { return "1000000031"; } }
        /// <summary>
        /// 商户订单号 
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 提交时间 
        /// </summary>
        public string txnTime
        {
            get
            {

                return DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

    }

    public class respTengModel
    {
        /// <summary>
        /// 版本号 
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// 代理商号
        /// </summary>
        public string agentId { get; set; }
        /// <summary>
        /// 商户代码 
        /// </summary>
        public string merId { get; set; }
        /// <summary>
        /// 商户订单号 
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// 应答码 
        /// </summary>
        public string respCode { get; set; }
        /// <summary>
        /// 应答信息 
        /// </summary>
        public string respMsg { get; set; }
        /// <summary>
        /// 签名  
        /// </summary>
        public string sign { get; set; }


        private int _backState;
        /// <summary>
        /// 系统签名
        /// </summary>

        public int backState
        {
            get
            {
                if (!string.IsNullOrEmpty(respCode))
                {
                    return respCode == "00" ? 0 : -100;
                }

                return _backState; }
            set
            {
                _backState = value;
            }
        }
    }
    #endregion



    #region 支付结果查询
    public class respPayDebitQueryModel : respTengModel
    {
        public string status { get; set; }
    }
    #endregion

    #region 代付结果查询
    public class respDebitWithdrawQueryModel : respTengModel
    {
        public string status { get; set; }
    }
    #endregion



    #region 支付接口
    public class reqTengDebitPaymentModel : reqTengModel
    {

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string idCardNo { get; set; }
        /// <summary>
        /// 交易卡预留手机号
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public string txnAmt { get; set; }
        /// <summary>
        /// 清算金额
        /// </summary>
        public string settleAmt { get; set; }
        /// <summary>
        /// 支付通知地址
        /// </summary>
        public string notifyUrl { get; set; }
        /// <summary>
        /// 交易卡卡号
        /// </summary>
        public string bankCardNo { get; set; }
        /// <summary>
        /// cvv
        /// </summary>
        public string lastThreeNo { get; set; }
        /// <summary>
        /// /有效期年
        /// </summary>
        public string cardExpYear { get; set; }
        /// <summary>
        /// 有效期月
        /// </summary>
        public string cardExpMonth { get; set; }
        /// <summary>
        /// 代付通知地址
        /// </summary>
        public string withdrawUrl { get; set; }
        /// <summary>
        /// 结算卡卡号
        /// </summary>
        public string settleCardNo { get; set; }
        /// <summary>
        /// 结算卡预留手机号
        /// </summary>
        public string settlePhone { get; set; }
    }
    #endregion




}
