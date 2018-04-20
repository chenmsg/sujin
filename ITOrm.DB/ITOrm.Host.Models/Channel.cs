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
	[Table("Channel")]
    public class Channel : BaseEntity
    {
        #region Entity Field
        private int _cid = 0;
		/// <summary>
        /// 
        /// </summary>
        [Key]		[DataMember(Order = 0)]
		public int CId { get{return _cid;} set{_cid=value;} }
	    private string _cname = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string CName { get{return _cname;} set{_cname=value;} }
	    private string _cpwd = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string CPwd { get{return _cpwd;} set{_cpwd=value;} }
	    private string _cmd5key = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string CMd5Key { get{return _cmd5key;} set{_cmd5key=value;} }
	    private DateTime _ccreatetime = DateTime.Now;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public DateTime CCreateTime { get{return _ccreatetime;} set{_ccreatetime=value;} }
	    
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
            public readonly static string TableName = "Channel";

            /// <summary>
            /// 所有字段
            /// </summary>
            public readonly static string All = "CId,CName,CPwd,CMd5Key,CCreateTime";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string CId = "CId";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string CName = "CName";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string CPwd = "CPwd";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string CMd5Key = "CMd5Key";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string CCreateTime = "CCreateTime";
            
        }
        #endregion
    }
}

