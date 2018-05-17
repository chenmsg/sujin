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
	[Table("MasgetUser")]
    public class MasgetUser : BaseEntity
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
        /// 用户ID
        /// </summary>
        		[DataMember(Order = 0)]
		public int UserId { get{return _userid;} set{_userid=value;} }
	    private decimal _rate1 = 0M;
		/// <summary>
        /// 交易费率
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Rate1 { get{return _rate1;} set{_rate1=value;} }
	    private int _ratestate1 = 0;
		/// <summary>
        /// 状态
        /// </summary>
        		[DataMember(Order = 0)]
		public int RateState1 { get{return _ratestate1;} set{_ratestate1=value;} }
	    private decimal _rate3 = 0M;
		/// <summary>
        /// 结算费率
        /// </summary>
        		[DataMember(Order = 0)]
		public decimal Rate3 { get{return _rate3;} set{_rate3=value;} }
	    private int _ratestate3 = 0;
		/// <summary>
        /// 状态
        /// </summary>
        		[DataMember(Order = 0)]
		public int RateState3 { get{return _ratestate3;} set{_ratestate3=value;} }
	    private string _appid = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string Appid { get{return _appid;} set{_appid=value;} }
	    private string _session = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string Session { get{return _session;} set{_session=value;} }
	    private string _secretkey = string.Empty;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public string Secretkey { get{return _secretkey;} set{_secretkey=value;} }
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
	    private string _companyid = string.Empty;
		/// <summary>
        /// 子商户id
        /// </summary>
        		[DataMember(Order = 0)]
		public string CompanyId { get{return _companyid;} set{_companyid=value;} }
	    private int _platform = 0;
		/// <summary>
        /// 
        /// </summary>
        		[DataMember(Order = 0)]
		public int Platform { get{return _platform;} set{_platform=value;} }
	    private int _state = 0;
		/// <summary>
        /// 0未入驻  1已入驻 
        /// </summary>
        		[DataMember(Order = 0)]
		public int State { get{return _state;} set{_state=value;} }
	    private int _apply = 0;
		/// <summary>
        /// 快捷协议 0未开通 1已开通
        /// </summary>
        		[DataMember(Order = 0)]
		public int Apply { get{return _apply;} set{_apply=value;} }
	    private int _typeid = 0;
		/// <summary>
        /// 用户类型 1积分用户  2 无积分用户
        /// </summary>
        		[DataMember(Order = 0)]
		public int TypeId { get{return _typeid;} set{_typeid=value;} }
	    private string _treatycode = string.Empty;
		/// <summary>
        /// 代扣协议
        /// </summary>
        		[DataMember(Order = 0)]
		public string Treatycode { get{return _treatycode;} set{_treatycode=value;} }
	    
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
            public readonly static string TableName = "MasgetUser";

            /// <summary>
            /// 所有字段
            /// </summary>
            public readonly static string All = "ID,UserId,Rate1,RateState1,Rate3,RateState3,Appid,Session,Secretkey,CTime,UTime,CompanyId,Platform,State,Apply,TypeId,Treatycode";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string ID = "ID";
            
            /// <summary>
            /// 用户ID
            /// </summary>
            public readonly static string UserId = "UserId";
            
            /// <summary>
            /// 交易费率
            /// </summary>
            public readonly static string Rate1 = "Rate1";
            
            /// <summary>
            /// 状态
            /// </summary>
            public readonly static string RateState1 = "RateState1";
            
            /// <summary>
            /// 结算费率
            /// </summary>
            public readonly static string Rate3 = "Rate3";
            
            /// <summary>
            /// 状态
            /// </summary>
            public readonly static string RateState3 = "RateState3";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Appid = "Appid";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Session = "Session";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Secretkey = "Secretkey";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string CTime = "CTime";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string UTime = "UTime";
            
            /// <summary>
            /// 子商户id
            /// </summary>
            public readonly static string CompanyId = "CompanyId";
            
            /// <summary>
            /// 
            /// </summary>
            public readonly static string Platform = "Platform";
            
            /// <summary>
            /// 0未入驻  1已入驻 
            /// </summary>
            public readonly static string State = "State";
            
            /// <summary>
            /// 快捷协议 0未开通 1已开通
            /// </summary>
            public readonly static string Apply = "Apply";
            
            /// <summary>
            /// 用户类型 1积分用户  2 无积分用户
            /// </summary>
            public readonly static string TypeId = "TypeId";
            
            /// <summary>
            /// 代扣协议
            /// </summary>
            public readonly static string Treatycode = "Treatycode";
            
        }
        #endregion
    }
}

