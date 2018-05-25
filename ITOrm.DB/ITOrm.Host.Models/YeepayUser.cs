//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的...
//    生成时间 2018-05-22 10:54:05 By CClump
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
	[Table("YeepayUser")]
    public class YeepayUser : BaseEntity
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
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int UserId { get{return _userid;} set{_userid=value;} }
	    private int _isaudit = 0;
		/// <summary>
        /// 审核状态 0未审核  1审核通过  -1  审核失败
        /// </summary>
        		[DataMember(Order = 0)]
		public int IsAudit { get{return _isaudit;} set{_isaudit=value;} }
	    private string _customernumber = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string CustomerNumber { get{return _customernumber;} set{_customernumber=value;} }
	    private decimal _rate1 = 0M;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Rate1 { get{return _rate1;} set{_rate1=value;} }
	    private int _ratestate1 = 0;
		/// <summary>
        /// 费率设置状态 0未设置 1已设置
        /// </summary>
        		[DataMember(Order = 0)]
		public int RateState1 { get{return _ratestate1;} set{_ratestate1=value;} }
	    private decimal _rate2 = 0M;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Rate2 { get{return _rate2;} set{_rate2=value;} }
	    private int _ratestate2 = 0;
		/// <summary>
        /// 费率设置状态 0未设置 1已设置
        /// </summary>
        		[DataMember(Order = 0)]
		public int RateState2 { get{return _ratestate2;} set{_ratestate2=value;} }
	    private decimal _rate3 = 0M;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Rate3 { get{return _rate3;} set{_rate3=value;} }
	    private int _ratestate3 = 0;
		/// <summary>
        /// 费率设置状态 0未设置 1已设置
        /// </summary>
        		[DataMember(Order = 0)]
		public int RateState3 { get{return _ratestate3;} set{_ratestate3=value;} }
	    private decimal _rate4 = 0M;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Rate4 { get{return _rate4;} set{_rate4=value;} }
	    private int _ratestate4 = 0;
		/// <summary>
        /// 费率设置状态 0未设置 1已设置
        /// </summary>
        		[DataMember(Order = 0)]
		public int RateState4 { get{return _ratestate4;} set{_ratestate4=value;} }
	    private decimal _rate5 = 0M;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Rate5 { get{return _rate5;} set{_rate5=value;} }
	    private int _ratestate5 = 0;
		/// <summary>
        /// 费率设置状态 0未设置 1已设置
        /// </summary>
        		[DataMember(Order = 0)]
		public int RateState5 { get{return _ratestate5;} set{_ratestate5=value;} }
	    private DateTime _ctime = DateTime.Now;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public DateTime CTime { get{return _ctime;} set{_ctime=value;} }
	    private DateTime _utime = DateTime.Now;
		/// <summary>
        /// 更新时间
        /// </summary>
        		[DataMember(Order = 0)]
		public DateTime UTime { get{return _utime;} set{_utime=value;} }
	    
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
            public readonly static string TableName = "YeepayUser";

            /// <summary>
            /// 所有字段
            /// </summary>
            public readonly static string All = "ID,UserId,IsAudit,CustomerNumber,Rate1,RateState1,Rate2,RateState2,Rate3,RateState3,Rate4,RateState4,Rate5,RateState5,CTime,UTime";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string ID = "ID";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string UserId = "UserId";
            
            /// <summary>
            /// 审核状态 0未审核  1审核通过  -1  审核失败
            /// </summary>
            public readonly static string IsAudit = "IsAudit";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string CustomerNumber = "CustomerNumber";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Rate1 = "Rate1";
            
            /// <summary>
            /// 费率设置状态 0未设置 1已设置
            /// </summary>
            public readonly static string RateState1 = "RateState1";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Rate2 = "Rate2";
            
            /// <summary>
            /// 费率设置状态 0未设置 1已设置
            /// </summary>
            public readonly static string RateState2 = "RateState2";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Rate3 = "Rate3";
            
            /// <summary>
            /// 费率设置状态 0未设置 1已设置
            /// </summary>
            public readonly static string RateState3 = "RateState3";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Rate4 = "Rate4";
            
            /// <summary>
            /// 费率设置状态 0未设置 1已设置
            /// </summary>
            public readonly static string RateState4 = "RateState4";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Rate5 = "Rate5";
            
            /// <summary>
            /// 费率设置状态 0未设置 1已设置
            /// </summary>
            public readonly static string RateState5 = "RateState5";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string CTime = "CTime";
            
            /// <summary>
            /// 更新时间
            /// </summary>
            public readonly static string UTime = "UTime";
            
        }
        #endregion
    }
}

