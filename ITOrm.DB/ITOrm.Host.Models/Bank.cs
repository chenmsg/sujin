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
	[Table("Bank")]
    public class Bank : BaseEntity
    {
        #region Entity Field
        private int _id = 0;
		/// <summary>
        /// 
        /// </summary>
        [Key]		[DataMember(Order = 0)]
		public int ID { get{return _id;} set{_id=value;} }
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
	    private DateTime _ctime = DateTime.Now;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public DateTime CTime { get{return _ctime;} set{_ctime=value;} }
	    private int _state = 0;
		/// <summary>
        /// 
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
            public readonly static string TableName = "Bank";

            /// <summary>
            /// 所有字段
            /// </summary>
            public readonly static string All = "ID,BankName,BankCode,CTime,State";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string ID = "ID";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string BankName = "BankName";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string BankCode = "BankCode";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string CTime = "CTime";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string State = "State";
            
        }
        #endregion
    }
}

