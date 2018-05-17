//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的...
//    生成时间 2018-05-16 21:24:16 By CClump
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
	[Table("AccountQueue")]
    public class AccountQueue : BaseEntity
    {
        #region Entity Field
        private int _id = 0;
		/// <summary>
        /// 
        /// </summary>
        [Key]		[DataMember(Order = 0)]
		public int ID { get{return _id;} set{_id=value;} }
	    private decimal _amount = 0M;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Amount { get{return _amount;} set{_amount=value;} }
	    private int _userid = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int UserId { get{return _userid;} set{_userid=value;} }
	    private int _typeid = 0;
		/// <summary>
        /// 类型
        /// </summary>
        		[DataMember(Order = 0)]
		public int TypeId { get{return _typeid;} set{_typeid=value;} }
	    private int _platform = 0;
		/// <summary>
        /// 平台
        /// </summary>
        		[DataMember(Order = 0)]
		public int Platform { get{return _platform;} set{_platform=value;} }
	    private int _keyid = 0;
		/// <summary>
        /// 关联ID
        /// </summary>
        		[DataMember(Order = 0)]
		public int KeyId { get{return _keyid;} set{_keyid=value;} }
	    private int _inorout = 0;
		/// <summary>
        /// 0冻结 1增加 -1扣减
        /// </summary>
        		[DataMember(Order = 0)]
		public int InOrOut { get{return _inorout;} set{_inorout=value;} }
	    private int _state = 0;
		/// <summary>
        /// 0待处理 1已处理 -1  处理失败
        /// </summary>
        		[DataMember(Order = 0)]
		public int State { get{return _state;} set{_state=value;} }
	    private DateTime _ctime = DateTime.Now;
		/// <summary>
        /// 创建时间
        /// </summary>
        		[DataMember(Order = 0)]
		public DateTime CTime { get{return _ctime;} set{_ctime=value;} }
	    private DateTime _utime = DateTime.Now;
		/// <summary>
        /// 修改时间
        /// </summary>
        		[DataMember(Order = 0)]
		public DateTime UTime { get{return _utime;} set{_utime=value;} }
	    private string _remark = string.Empty;
		/// <summary>
        /// 备注
        /// </summary>
        		[DataMember(Order = 0)]
		public string Remark { get{return _remark;} set{_remark=value;} }
	    
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
            public readonly static string TableName = "AccountQueue";

            /// <summary>
            /// 所有字段
            /// </summary>
            public readonly static string All = "ID,Amount,UserId,TypeId,Platform,KeyId,InOrOut,State,CTime,UTime,Remark";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string ID = "ID";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Amount = "Amount";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string UserId = "UserId";
            
            /// <summary>
            /// 类型
            /// </summary>
            public readonly static string TypeId = "TypeId";
            
            /// <summary>
            /// 平台
            /// </summary>
            public readonly static string Platform = "Platform";
            
            /// <summary>
            /// 关联ID
            /// </summary>
            public readonly static string KeyId = "KeyId";
            
            /// <summary>
            /// 0冻结 1增加 -1扣减
            /// </summary>
            public readonly static string InOrOut = "InOrOut";
            
            /// <summary>
            /// 0待处理 1已处理 -1  处理失败
            /// </summary>
            public readonly static string State = "State";
            
            /// <summary>
            /// 创建时间
            /// </summary>
            public readonly static string CTime = "CTime";
            
            /// <summary>
            /// 修改时间
            /// </summary>
            public readonly static string UTime = "UTime";
            
            /// <summary>
            /// 备注
            /// </summary>
            public readonly static string Remark = "Remark";
            
        }
        #endregion
    }
}

