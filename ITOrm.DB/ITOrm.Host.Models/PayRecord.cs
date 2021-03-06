//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的...
//    生成时间 2018-08-14 11:29:10 By CClump
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using ITOrm.Core.Dapper;
using System.Runtime.Serialization;
namespace ITOrm.Host.Models
{
    /// <summary>
    /// 备注
    /// </summary>
    [Serializable]
	[Table("PayRecord")]
    public class PayRecord : BaseEntity
    {
        #region Entity Field
        private int _id = 0;
		/// <summary>
        /// 
        /// </summary>
        [Key]		[DataMember(Order = 0)]
		public int ID { get{return _id;} set{_id=value;} }
	    private int _userid = 0;
		/// <summary>
        /// 用户ID
        /// </summary>
        		[DataMember(Order = 0)]
		public int UserId { get{return _userid;} set{_userid=value;} }
	    private int _withdrawid = 0;
		/// <summary>
        /// 结算ID
        /// </summary>
        		[DataMember(Order = 0)]
		public int WithDrawId { get{return _withdrawid;} set{_withdrawid=value;} }
	    private decimal _amount = 0M;
		/// <summary>
        /// 订单金额
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Amount { get{return _amount;} set{_amount=value;} }
	    private decimal _withdrawamount = 0M;
		/// <summary>
        /// 结算金额
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal WithDrawAmount { get{return _withdrawamount;} set{_withdrawamount=value;} }
	    private decimal _actualamount = 0M;
		/// <summary>
        /// 实际到账
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal ActualAmount { get{return _actualamount;} set{_actualamount=value;} }
	    private decimal _fee = 0M;
		/// <summary>
        /// 手续费
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Fee { get{return _fee;} set{_fee=value;} }
	    private decimal _fee3 = 0M;
		/// <summary>
        /// 结算手续费
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Fee3 { get{return _fee3;} set{_fee3=value;} }
	    private decimal _rate = 0M;
		/// <summary>
        /// 交易费率
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Rate { get{return _rate;} set{_rate=value;} }
	    private int _platform = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int Platform { get{return _platform;} set{_platform=value;} }
	    private string _ip = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string Ip { get{return _ip;} set{_ip=value;} }
	    private DateTime _ctime = DateTime.Now;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public DateTime CTime { get{return _ctime;} set{_ctime=value;} }
	    private int _state = 0;
		/// <summary>
        /// 订单状态 0发起 1成功 10支付成功  -1失败
        /// </summary>
        		[DataMember(Order = 0)]
		public int State { get{return _state;} set{_state=value;} }
	    private int _drawstate = 0;
		/// <summary>
        /// 结算状态
        /// </summary>
        		[DataMember(Order = 0)]
		public int DrawState { get{return _drawstate;} set{_drawstate=value;} }
	    private string _drawbankcard = string.Empty;
		/// <summary>
        /// 收款卡号
        /// </summary>
        		[DataMember(Order = 0)]
		public string DrawBankCard { get{return _drawbankcard;} set{_drawbankcard=value;} }
	    private string _message = string.Empty;
		/// <summary>
        /// 文字说明
        /// </summary>
        		[DataMember(Order = 0)]
		public string Message { get{return _message;} set{_message=value;} }
	    private DateTime _utime = DateTime.Now;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public DateTime UTime { get{return _utime;} set{_utime=value;} }
	    private DateTime _paytime = DateTime.Now;
		/// <summary>
        /// 支付成功时间
        /// </summary>
        		[DataMember(Order = 0)]
		public DateTime PayTime { get{return _paytime;} set{_paytime=value;} }
	    private DateTime _handletime = DateTime.Now;
		/// <summary>
        /// 到账时间
        /// </summary>
        		[DataMember(Order = 0)]
		public DateTime HandleTime { get{return _handletime;} set{_handletime=value;} }
	    private string _bankcode = string.Empty;
		/// <summary>
        /// 银行编码
        /// </summary>
        		[DataMember(Order = 0)]
		public string BankCode { get{return _bankcode;} set{_bankcode=value;} }
	    private string _payername = string.Empty;
		/// <summary>
        /// 持卡人姓名
        /// </summary>
        		[DataMember(Order = 0)]
		public string PayerName { get{return _payername;} set{_payername=value;} }
	    private string _payerphone = string.Empty;
		/// <summary>
        /// 银行预留手机号
        /// </summary>
        		[DataMember(Order = 0)]
		public string PayerPhone { get{return _payerphone;} set{_payerphone=value;} }
	    private string _bankcard = string.Empty;
		/// <summary>
        /// 银行卡号
        /// </summary>
        		[DataMember(Order = 0)]
		public string BankCard { get{return _bankcard;} set{_bankcard=value;} }
	    private string _lastno = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string LastNo { get{return _lastno;} set{_lastno=value;} }
	    private string _src = string.Empty;
		/// <summary>
        /// 支付方式
        /// </summary>
        		[DataMember(Order = 0)]
		public string Src { get{return _src;} set{_src=value;} }
	    private int _channeltype = 0;
		/// <summary>
        /// 支付渠道ID
        /// </summary>
        		[DataMember(Order = 0)]
		public int ChannelType { get{return _channeltype;} set{_channeltype=value;} }
	    private decimal _income = 0M;
		/// <summary>
        /// 收益
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Income { get{return _income;} set{_income=value;} }
	    private decimal _drawincome = 0M;
		/// <summary>
        /// 结算收益
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal DrawIncome { get{return _drawincome;} set{_drawincome=value;} }
	    
		#endregion

        #region 字段名信息 方便调用
        /// <summary>
        /// 数据表“WS_Log”的相关信息[数据库名、表名及字段名]
        /// </summary>
        [Serializable]
        public struct _
        {
            /// <summary>
            /// 数据库名
            /// </summary>
            public readonly static string DataBaseName = "SujinDB";

            /// <summary>
            /// 表名
            /// </summary>
            public readonly static string TableName = "PayRecord";

            /// <summary>
            /// 所有字段
            /// </summary>
            public readonly static string All = "ID,UserId,WithDrawId,Amount,WithDrawAmount,ActualAmount,Fee,Fee3,Rate,Platform,Ip,CTime,State,DrawState,DrawBankCard,Message,UTime,PayTime,HandleTime,BankCode,PayerName,PayerPhone,BankCard,LastNo,Src,ChannelType,Income,DrawIncome";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string ID = "ID";
            
            /// <summary>
            /// 用户ID
            /// </summary>
            public readonly static string UserId = "UserId";
            
            /// <summary>
            /// 结算ID
            /// </summary>
            public readonly static string WithDrawId = "WithDrawId";
            
            /// <summary>
            /// 订单金额
            /// </summary>
            public readonly static string Amount = "Amount";
            
            /// <summary>
            /// 结算金额
            /// </summary>
            public readonly static string WithDrawAmount = "WithDrawAmount";
            
            /// <summary>
            /// 实际到账
            /// </summary>
            public readonly static string ActualAmount = "ActualAmount";
            
            /// <summary>
            /// 手续费
            /// </summary>
            public readonly static string Fee = "Fee";
            
            /// <summary>
            /// 结算手续费
            /// </summary>
            public readonly static string Fee3 = "Fee3";
            
            /// <summary>
            /// 交易费率
            /// </summary>
            public readonly static string Rate = "Rate";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Platform = "Platform";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Ip = "Ip";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string CTime = "CTime";
            
            /// <summary>
            /// 订单状态 0发起 1成功 10支付成功  -1失败
            /// </summary>
            public readonly static string State = "State";
            
            /// <summary>
            /// 结算状态
            /// </summary>
            public readonly static string DrawState = "DrawState";
            
            /// <summary>
            /// 收款卡号
            /// </summary>
            public readonly static string DrawBankCard = "DrawBankCard";
            
            /// <summary>
            /// 文字说明
            /// </summary>
            public readonly static string Message = "Message";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string UTime = "UTime";
            
            /// <summary>
            /// 支付成功时间
            /// </summary>
            public readonly static string PayTime = "PayTime";
            
            /// <summary>
            /// 到账时间
            /// </summary>
            public readonly static string HandleTime = "HandleTime";
            
            /// <summary>
            /// 银行编码
            /// </summary>
            public readonly static string BankCode = "BankCode";
            
            /// <summary>
            /// 持卡人姓名
            /// </summary>
            public readonly static string PayerName = "PayerName";
            
            /// <summary>
            /// 银行预留手机号
            /// </summary>
            public readonly static string PayerPhone = "PayerPhone";
            
            /// <summary>
            /// 银行卡号
            /// </summary>
            public readonly static string BankCard = "BankCard";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string LastNo = "LastNo";
            
            /// <summary>
            /// 支付方式
            /// </summary>
            public readonly static string Src = "Src";
            
            /// <summary>
            /// 支付渠道ID
            /// </summary>
            public readonly static string ChannelType = "ChannelType";
            
            /// <summary>
            /// 收益
            /// </summary>
            public readonly static string Income = "Income";
            
            /// <summary>
            /// 结算收益
            /// </summary>
            public readonly static string DrawIncome = "DrawIncome";
            
        }
        #endregion
    }
}

