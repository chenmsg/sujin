//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的...
//    生成时间 2018-08-14 11:29:12 By CClump
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
	[Table("View_BankQuota")]
    public class ViewBankQuota : BaseEntity
    {
        #region Entity Field
        private int _id = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int ID { get{return _id;} set{_id=value;} }
	    private int _bankid = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int BankId { get{return _bankid;} set{_bankid=value;} }
	    private decimal _singlequota = 0M;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal SingleQuota { get{return _singlequota;} set{_singlequota=value;} }
	    private decimal _dayquota = 0M;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal DayQuota { get{return _dayquota;} set{_dayquota=value;} }
	    private decimal _mouthquota = 0M;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal MouthQuota { get{return _mouthquota;} set{_mouthquota=value;} }
	    private string _source = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string Source { get{return _source;} set{_source=value;} }
	    private int _channeltype = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int ChannelType { get{return _channeltype;} set{_channeltype=value;} }
	    private int _state = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int State { get{return _state;} set{_state=value;} }
	    private string _bankname = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string BankName { get{return _bankname;} set{_bankname=value;} }
	    private string _bankcode = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string BankCode { get{return _bankcode;} set{_bankcode=value;} }
	    
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
            public readonly static string TableName = "View_BankQuota";

            /// <summary>
            /// 所有字段
            /// </summary>
            public readonly static string All = "ID,BankId,SingleQuota,DayQuota,MouthQuota,Source,ChannelType,State,BankName,BankCode";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string ID = "ID";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string BankId = "BankId";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string SingleQuota = "SingleQuota";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string DayQuota = "DayQuota";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string MouthQuota = "MouthQuota";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Source = "Source";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string ChannelType = "ChannelType";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string State = "State";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string BankName = "BankName";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string BankCode = "BankCode";
            
        }
        #endregion
    }
}

