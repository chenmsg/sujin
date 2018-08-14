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
	[Table("SendMsg")]
    public class SendMsg : BaseEntity
    {
        #region Entity Field
        private int _id = 0;
		/// <summary>
        /// 
        /// </summary>
        [Key]		[DataMember(Order = 0)]
		public int ID { get{return _id;} set{_id=value;} }
	    private int _typeid = 0;
		/// <summary>
        /// 类型ID  对应枚举 EnumSendMsg
        /// </summary>
        		[DataMember(Order = 0)]
		public int TypeId { get{return _typeid;} set{_typeid=value;} }
	    private string _context = string.Empty;
		/// <summary>
        /// 内容
        /// </summary>
        		[DataMember(Order = 0)]
		public string Context { get{return _context;} set{_context=value;} }
	    private DateTime _utime = DateTime.Now;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public DateTime UTime { get{return _utime;} set{_utime=value;} }
	    private DateTime _ctime = DateTime.Now;
		/// <summary>
        /// 创建时间
        /// </summary>
        		[DataMember(Order = 0)]
		public DateTime CTime { get{return _ctime;} set{_ctime=value;} }
	    private string _mobile = string.Empty;
		/// <summary>
        /// 手机号
        /// </summary>
        		[DataMember(Order = 0)]
		public string Mobile { get{return _mobile;} set{_mobile=value;} }
	    private int _userid = 0;
		/// <summary>
        /// 用户ID
        /// </summary>
        		[DataMember(Order = 0)]
		public int UserId { get{return _userid;} set{_userid=value;} }
	    private string _service = string.Empty;
		/// <summary>
        /// 业务
        /// </summary>
        		[DataMember(Order = 0)]
		public string Service { get{return _service;} set{_service=value;} }
	    private int _state = 0;
		/// <summary>
        /// 发送状态 0 发送中 1 失败 2成功
        /// </summary>
        		[DataMember(Order = 0)]
		public int State { get{return _state;} set{_state=value;} }
	    private int _merchant = 0;
		/// <summary>
        /// 短信通道
        /// </summary>
        		[DataMember(Order = 0)]
		public int Merchant { get{return _merchant;} set{_merchant=value;} }
	    private int _platform = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int Platform { get{return _platform;} set{_platform=value;} }
	    private string _ip = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string IP { get{return _ip;} set{_ip=value;} }
	    private string _relationid = string.Empty;
		/// <summary>
        /// 关联的业务ID
        /// </summary>
        		[DataMember(Order = 0)]
		public string RelationId { get{return _relationid;} set{_relationid=value;} }
	    
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
            public readonly static string TableName = "SendMsg";

            /// <summary>
            /// 所有字段
            /// </summary>
            public readonly static string All = "ID,TypeId,Context,UTime,CTime,Mobile,UserId,Service,State,Merchant,Platform,IP,RelationId";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string ID = "ID";
            
            /// <summary>
            /// 类型ID  对应枚举 EnumSendMsg
            /// </summary>
            public readonly static string TypeId = "TypeId";
            
            /// <summary>
            /// 内容
            /// </summary>
            public readonly static string Context = "Context";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string UTime = "UTime";
            
            /// <summary>
            /// 创建时间
            /// </summary>
            public readonly static string CTime = "CTime";
            
            /// <summary>
            /// 手机号
            /// </summary>
            public readonly static string Mobile = "Mobile";
            
            /// <summary>
            /// 用户ID
            /// </summary>
            public readonly static string UserId = "UserId";
            
            /// <summary>
            /// 业务
            /// </summary>
            public readonly static string Service = "Service";
            
            /// <summary>
            /// 发送状态 0 发送中 1 失败 2成功
            /// </summary>
            public readonly static string State = "State";
            
            /// <summary>
            /// 短信通道
            /// </summary>
            public readonly static string Merchant = "Merchant";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Platform = "Platform";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string IP = "IP";
            
            /// <summary>
            /// 关联的业务ID
            /// </summary>
            public readonly static string RelationId = "RelationId";
            
        }
        #endregion
    }
}

