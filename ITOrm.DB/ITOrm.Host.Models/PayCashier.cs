//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的...
//    生成时间 2018-04-23 13:46:39 By CClump
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
	[Table("PayCashier")]
    public class PayCashier : BaseEntity
    {
        #region Entity Field
        private int _id = 0;
		/// <summary>
        /// 
        /// </summary>
        [Key]		[DataMember(Order = 0)]
		public int ID { get{return _id;} set{_id=value;} }
	    private int _channeltype = 0;
		/// <summary>
        /// 渠道类型 1 荣邦积分 2荣邦无积分
        /// </summary>
        		[DataMember(Order = 0)]
		public int ChannelType { get{return _channeltype;} set{_channeltype=value;} }
	    private string _value = string.Empty;
		/// <summary>
        /// 报文json
        /// </summary>
        		[DataMember(Order = 0)]
		public string Value { get{return _value;} set{_value=value;} }
	    private DateTime _ctime = DateTime.Now;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public DateTime CTime { get{return _ctime;} set{_ctime=value;} }
	    private DateTime _utime = DateTime.Now;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public DateTime UTime { get{return _utime;} set{_utime=value;} }
	    private int _state = 0;
		/// <summary>
        /// 状态 0待支付 -1支付失败 2支付成功
        /// </summary>
        		[DataMember(Order = 0)]
		public int State { get{return _state;} set{_state=value;} }
	    private int _logid = 0;
		/// <summary>
        /// 日志ID
        /// </summary>
        		[DataMember(Order = 0)]
		public int LogId { get{return _logid;} set{_logid=value;} }
	    private int _ubkid = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int UbkId { get{return _ubkid;} set{_ubkid=value;} }
	    private int _userid = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int UserId { get{return _userid;} set{_userid=value;} }
	    private int _payrecordid = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int PayRecordId { get{return _payrecordid;} set{_payrecordid=value;} }
	    
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
            public readonly static string TableName = "PayCashier";

            /// <summary>
            /// 所有字段
            /// </summary>
            public readonly static string All = "ID,ChannelType,Value,CTime,UTime,State,LogId,UbkId,UserId,PayRecordId";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string ID = "ID";
            
            /// <summary>
            /// 渠道类型 1 荣邦积分 2荣邦无积分
            /// </summary>
            public readonly static string ChannelType = "ChannelType";
            
            /// <summary>
            /// 报文json
            /// </summary>
            public readonly static string Value = "Value";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string CTime = "CTime";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string UTime = "UTime";
            
            /// <summary>
            /// 状态 0待支付 -1支付失败 2支付成功
            /// </summary>
            public readonly static string State = "State";
            
            /// <summary>
            /// 日志ID
            /// </summary>
            public readonly static string LogId = "LogId";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string UbkId = "UbkId";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string UserId = "UserId";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string PayRecordId = "PayRecordId";
            
        }
        #endregion
    }
}

