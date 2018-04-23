//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的...
//    生成时间 2018-04-23 13:46:40 By CClump
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
	[Table("YeepayLogParas")]
    public class YeepayLogParas : BaseEntity
    {
        #region Entity Field
        private int _id = 0;
		/// <summary>
        /// 
        /// </summary>
        [Key]		[DataMember(Order = 0)]
		public int ID { get{return _id;} set{_id=value;} }
	    private int _logid = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int LogID { get{return _logid;} set{_logid=value;} }
	    private int _inorout = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int InOrOut { get{return _inorout;} set{_inorout=value;} }
	    private string _msg = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string Msg { get{return _msg;} set{_msg=value;} }
	    private DateTime _ctime = DateTime.Now;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public DateTime CTime { get{return _ctime;} set{_ctime=value;} }
	    
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
            public readonly static string TableName = "YeepayLogParas";

            /// <summary>
            /// 所有字段
            /// </summary>
            public readonly static string All = "ID,LogID,InOrOut,Msg,CTime";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string ID = "ID";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string LogID = "LogID";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string InOrOut = "InOrOut";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Msg = "Msg";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string CTime = "CTime";
            
        }
        #endregion
    }
}

