//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的...
//    生成时间 2018-04-13 17:56:52 By CClump
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
	[Table("KeyValue")]
    public class KeyValue : BaseEntity
    {
        #region Entity Field
        private int _id = 0;
		/// <summary>
        /// 
        /// </summary>
        [Key]		[DataMember(Order = 0)]
		public int ID { get{return _id;} set{_id=value;} }
	    private int _keyid = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int KeyId { get{return _keyid;} set{_keyid=value;} }
	    private string _value = string.Empty;
		/// <summary>
        /// 数据
        /// </summary>
        		[DataMember(Order = 0)]
		public string Value { get{return _value;} set{_value=value;} }
	    private string _value2 = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string Value2 { get{return _value2;} set{_value2=value;} }
	    private int _typeid = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int TypeId { get{return _typeid;} set{_typeid=value;} }
	    private string _remark = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string Remark { get{return _remark;} set{_remark=value;} }
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
	    private int _sort = 0;
		/// <summary>
        /// 排序
        /// </summary>
        		[DataMember(Order = 0)]
		public int Sort { get{return _sort;} set{_sort=value;} }
	    private int _state = 0;
		/// <summary>
        /// 状态 0可用 -1 不可用
        /// </summary>
        		[DataMember(Order = 0)]
		public int State { get{return _state;} set{_state=value;} }
	    
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
            public readonly static string TableName = "KeyValue";

            /// <summary>
            /// 所有字段
            /// </summary>
            public readonly static string All = "ID,KeyId,Value,Value2,TypeId,Remark,CTime,UTime,Sort,State";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string ID = "ID";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string KeyId = "KeyId";
            
            /// <summary>
            /// 数据
            /// </summary>
            public readonly static string Value = "Value";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Value2 = "Value2";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string TypeId = "TypeId";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Remark = "Remark";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string CTime = "CTime";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string UTime = "UTime";
            
            /// <summary>
            /// 排序
            /// </summary>
            public readonly static string Sort = "Sort";
            
            /// <summary>
            /// 状态 0可用 -1 不可用
            /// </summary>
            public readonly static string State = "State";
            
        }
        #endregion
    }
}

