//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的...
//    生成时间 2018-08-14 11:29:11 By CClump
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
	[Table("ShareProfit")]
    public class ShareProfit : BaseEntity
    {
        #region Entity Field
        private int _id = 0;
		/// <summary>
        /// 
        /// </summary>
        [Key]		[DataMember(Order = 0)]
		public int ID { get{return _id;} set{_id=value;} }
	    private int _payid = 0;
		/// <summary>
        /// 收款记录ID
        /// </summary>
        		[DataMember(Order = 0)]
		public int PayId { get{return _payid;} set{_payid=value;} }
	    private int _userid = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int UserId { get{return _userid;} set{_userid=value;} }
	    private int _baseuserid = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int BaseUserId { get{return _baseuserid;} set{_baseuserid=value;} }
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
	    private decimal _amount = 0M;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Amount { get{return _amount;} set{_amount=value;} }
	    private decimal _income = 0M;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Income { get{return _income;} set{_income=value;} }
	    private int _viptype = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int VipType { get{return _viptype;} set{_viptype=value;} }
	    private int _baseviptype = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int BaseVipType { get{return _baseviptype;} set{_baseviptype=value;} }
	    private decimal _maxrate = 0M;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal MaxRate { get{return _maxrate;} set{_maxrate=value;} }
	    private decimal _minrate = 0M;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal MinRate { get{return _minrate;} set{_minrate=value;} }
	    private decimal _profitlevel = 0M;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal ProfitLevel { get{return _profitlevel;} set{_profitlevel=value;} }
	    
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
            public readonly static string TableName = "ShareProfit";

            /// <summary>
            /// 所有字段
            /// </summary>
            public readonly static string All = "ID,PayId,UserId,BaseUserId,CTime,UTime,Amount,Income,VipType,BaseVipType,MaxRate,MinRate,ProfitLevel";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string ID = "ID";
            
            /// <summary>
            /// 收款记录ID
            /// </summary>
            public readonly static string PayId = "PayId";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string UserId = "UserId";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string BaseUserId = "BaseUserId";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string CTime = "CTime";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string UTime = "UTime";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Amount = "Amount";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Income = "Income";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string VipType = "VipType";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string BaseVipType = "BaseVipType";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string MaxRate = "MaxRate";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string MinRate = "MinRate";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string ProfitLevel = "ProfitLevel";
            
        }
        #endregion
    }
}

