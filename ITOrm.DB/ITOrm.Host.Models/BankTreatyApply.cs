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
	[Table("BankTreatyApply")]
    public class BankTreatyApply : BaseEntity
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
	    private int _ubkid = 0;
		/// <summary>
        /// UserBankCard 外键Id
        /// </summary>
        		[DataMember(Order = 0)]
		public int UbkID { get{return _ubkid;} set{_ubkid=value;} }
	    private string _bankcard = string.Empty;
		/// <summary>
        /// 认证卡号
        /// </summary>
        		[DataMember(Order = 0)]
		public string BankCard { get{return _bankcard;} set{_bankcard=value;} }
	    private string _mobile = string.Empty;
		/// <summary>
        /// 认证手机号
        /// </summary>
        		[DataMember(Order = 0)]
		public string Mobile { get{return _mobile;} set{_mobile=value;} }
	    private string _smsseq = string.Empty;
		/// <summary>
        /// 验证码
        /// </summary>
        		[DataMember(Order = 0)]
		public string Smsseq { get{return _smsseq;} set{_smsseq=value;} }
	    private string _treatycode = string.Empty;
		/// <summary>
        /// 协议号
        /// </summary>
        		[DataMember(Order = 0)]
		public string Treatycode { get{return _treatycode;} set{_treatycode=value;} }
	    private int _state = 0;
		/// <summary>
        /// 状态 0未签订  1等待确认  2确认成功  -1失败
        /// </summary>
        		[DataMember(Order = 0)]
		public int State { get{return _state;} set{_state=value;} }
	    private int _channeltype = 0;
		/// <summary>
        /// 渠道 0 易宝  1荣邦积分  2荣邦无积分
        /// </summary>
        		[DataMember(Order = 0)]
		public int ChannelType { get{return _channeltype;} set{_channeltype=value;} }
	    private int _platform = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int Platform { get{return _platform;} set{_platform=value;} }
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
            public readonly static string TableName = "BankTreatyApply";

            /// <summary>
            /// 所有字段
            /// </summary>
            public readonly static string All = "ID,UserId,UbkID,BankCard,Mobile,Smsseq,Treatycode,State,ChannelType,Platform,CTime,UTime";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string ID = "ID";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string UserId = "UserId";
            
            /// <summary>
            /// UserBankCard 外键Id
            /// </summary>
            public readonly static string UbkID = "UbkID";
            
            /// <summary>
            /// 认证卡号
            /// </summary>
            public readonly static string BankCard = "BankCard";
            
            /// <summary>
            /// 认证手机号
            /// </summary>
            public readonly static string Mobile = "Mobile";
            
            /// <summary>
            /// 验证码
            /// </summary>
            public readonly static string Smsseq = "Smsseq";
            
            /// <summary>
            /// 协议号
            /// </summary>
            public readonly static string Treatycode = "Treatycode";
            
            /// <summary>
            /// 状态 0未签订  1等待确认  2确认成功  -1失败
            /// </summary>
            public readonly static string State = "State";
            
            /// <summary>
            /// 渠道 0 易宝  1荣邦积分  2荣邦无积分
            /// </summary>
            public readonly static string ChannelType = "ChannelType";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Platform = "Platform";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string CTime = "CTime";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string UTime = "UTime";
            
        }
        #endregion
    }
}

