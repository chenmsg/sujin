//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的...
//    生成时间 2018-05-22 10:54:04 By CClump
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
	[Table("Account")]
    public class Account : BaseEntity
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
	    private decimal _total = 0M;
		/// <summary>
        /// 总金额
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Total { get{return _total;} set{_total=value;} }
	    private decimal _available = 0M;
		/// <summary>
        /// 可用余额
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Available { get{return _available;} set{_available=value;} }
	    private decimal _frozen = 0M;
		/// <summary>
        /// 冻结金额
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Frozen { get{return _frozen;} set{_frozen=value;} }
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
            public readonly static string TableName = "Account";

            /// <summary>
            /// 所有字段
            /// </summary>
            public readonly static string All = "ID,UserId,Total,Available,Frozen,CTime,UTime";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string ID = "ID";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string UserId = "UserId";
            
            /// <summary>
            /// 总金额
            /// </summary>
            public readonly static string Total = "Total";
            
            /// <summary>
            /// 可用余额
            /// </summary>
            public readonly static string Available = "Available";
            
            /// <summary>
            /// 冻结金额
            /// </summary>
            public readonly static string Frozen = "Frozen";
            
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

