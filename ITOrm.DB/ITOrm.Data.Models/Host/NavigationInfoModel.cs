//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的...
//    生成时间 2015-12-22 11:53:19 By CClump
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Clump.Data.Models.Host.Context;
using Clump.Core.Dapper;
using Clump.Utility.UI.PagerWebdiyer;

namespace Clump.Data.Models.Host
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
	[Table("navigation_info")]
    public class NavigationInfoModel : BaseEntityAction<NavigationInfoModel>
    {
        #region Entity Field
                
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int ID { get; set; }
                
        /// <summary>
        /// 分类
        /// </summary>
        public int CategoryID { get; set; }
        
                        
        /// <summary>
        /// 网站名称
        /// </summary>
        public string SiteName { get; set; }
                        
        /// <summary>
        /// 网站地址
        /// </summary>
        public string Url { get; set; }
                        
        /// <summary>
        /// 网站描述
        /// </summary>
        public string Description { get; set; }
                        
        /// <summary>
        /// 是否删除[0:正常][1:删除]
        /// </summary>
        public bool IsDel { get; set; }

		  
                
        /// <summary>
        /// 状态[0:未审核][审核通过][2:审核未通过]
        /// </summary>
        public int Status { get; set; }
        
                
        private DateTime _CreateTime;
        ///<summary>
        ///添加时间
        ///</summary>
        public DateTime CreateTime
        {
            get
            {
                if (_CreateTime != null)
                {
                    if (_CreateTime == Convert.ToDateTime("0001/1/1 0:00:00"))
                        _CreateTime = DateTime.Now;
                }
                else
                {
                    _CreateTime = DateTime.Now;
                }
                return _CreateTime;
            }
            set
            {
                if (value != null)
                {
                    if (value == Convert.ToDateTime("0001/1/1 0:00:00"))
                        value = DateTime.Now;
                }
                else
                {
                    value = DateTime.Now;
                }
                _CreateTime = value;
            }
        }
        #endregion

        #region ==========查询单一实体

        /// <summary>
        /// 根据主键ID取得单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="id">实体的主键ID</param>
        /// <returns>NavigationInfoModel实体对象</returns>
        public static NavigationInfoModel Single(int id)
        {
            return new NavigationInfoModel().Get(id);
        }

        /// <summary>
        /// 根据where条件取得单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <returns>NavigationInfoModel实体对象</returns>
        public static NavigationInfoModel Single(string where, object param = null)
        {
            return new NavigationInfoModel().Get(where, param);
        }
        #endregion

        #region ==========查询列表集合

        /// <summary>
        /// 查询List集合
        /// </summary>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <param name="orderBy">排序语句[例子：order by id desc ]</param>
        /// <returns>List集合</returns>
        public static List<NavigationInfoModel> GetQuery(string where, object param = null, string orderBy = null)
        {
            return new NavigationInfoModel().GetList(string.Format("SELECT ID,CategoryID,SiteName,Url,Description,IsDel,Status,CreateTime FROM navigation_info WHERE {0} {1}", where, orderBy), param);
        }

        /// <summary>
        /// 根据指定的字段名，查询List集合
        /// </summary>
        /// <param name="field">查询的字段名[英文逗号分隔]</param>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <param name="orderBy">排序语句[例子：order by id desc ]</param>
        /// <returns></returns>
        public static List<NavigationInfoModel> GetQuery(string field, string where, object param = null, string orderBy = null)
        {
            return new NavigationInfoModel().GetList(string.Format("SELECT {0} FROM navigation_info WHERE {1} {2}", field, where, orderBy), param);
        }

        /// <summary>
        /// 查询List集合的前几条
        /// </summary>
        /// <param name="top">前多少条</param>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <param name="orderBy">排序语句[例子：order by id desc ]</param>
        /// <returns>List集合</returns>
        public static List<NavigationInfoModel> GetQuery(int top, string where, object param = null, string orderBy = null)
        {
            return new NavigationInfoModel().GetList(string.Format("SELECT ID,CategoryID,SiteName,Url,Description,IsDel,Status,CreateTime FROM navigation_info WHERE {0} {1} Limit 0,{2}", where, orderBy, top), param);
        }

        /// <summary>
        /// 根据指定的字段名，查询List集合的前几条
        /// </summary>
        /// <param name="top">前多少条</param>
        /// <param name="field">查询的字段名[英文逗号分隔]</param>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <param name="orderBy">排序语句[例子：order by id desc ]</param>
        /// <returns></returns>
        public static List<NavigationInfoModel> GetQuery(int top, string field, string where, object param = null, string orderBy = null)
        {
            return new NavigationInfoModel().GetList(string.Format("SELECT {0} FROM navigation_info WHERE {1} {2} Limit 0,{3}", field, where, orderBy, top), param);
        }

        /// <summary>
        /// 根据条件获取条数
        /// </summary>
        /// <param name="where">sql条件</param>
        /// <param name="param">参数化</param>
        /// <returns></returns>
        public static int Count(string where, object param = null)
        {
            return new NavigationInfoModel().GetCount("SELECT COUNT(*) FROM navigation_info WHERE " + where, param);
        }

        /// <summary>
        /// 根据条件获取表中某一字段非空的条数
        /// </summary>
        /// <param name="fieldname">表的某一个字段</param>
        /// <param name="where">sql条件</param>
        /// <param name="param">参数化</param>
        /// <returns></returns>
        public static int Count(string fieldname, string where, object param = null)
        {
            return new NavigationInfoModel().GetCount(string.Format("SELECT COUNT({0}) FROM navigation_info WHERE {1}", fieldname, where), param);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageSize">每页条数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="where">查询条件</param>
        /// <param name="param">条件语句参数化</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public static PagedList<NavigationInfoModel> GetPaged(int pageSize, int pageIndex, out int totalCount, string where, object param = null, string orderBy = null)
        {
            string orderByNow = " ORDER BY ID DESC ";
            if (orderBy != null && orderBy != "")
            {
                orderByNow = orderBy;
            }
            totalCount = 0;
            int topNum = pageSize * (pageIndex - 1);
			string field="ID,CategoryID,SiteName,Url,Description,IsDel,Status,CreateTime";
            string sql = string.Format("SELECT {0} FROM navigation_info ", field);
            if (topNum <= 0)
            {
                sql = string.Format("SELECT {0} FROM navigation_info WHERE {1} {2} LIMIT 0,{3}", field, where, orderByNow, pageSize);
            }
            else
            {
                sql = string.Format("SELECT {0} FROM navigation_info WHERE {1} {2} LIMIT {3},{4}", field, where, orderByNow, topNum, pageSize);
            }
            try
            {
                totalCount = new NavigationInfoModel().GetCount(string.Format("SELECT COUNT(*) FROM navigation_info WHERE {0}", where), param);
                return new PagedList<NavigationInfoModel>(new NavigationInfoModel().GetList(sql, param), pageIndex, pageSize, totalCount);
            }
            catch (Exception e)
            {
                log.Error("获取分页数据失败", e);
                return null;
            }
        }

        /// <summary>
        /// 分页查询,指定查询的字段名
        /// </summary>
        /// <param name="pageSize">每页条数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="field">查询的字段名[英文逗号分隔]</param>
        /// <param name="where">查询条件</param>
        /// <param name="param">条件语句参数化</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public static PagedList<NavigationInfoModel> GetPaged(int pageSize, int pageIndex, out int totalCount, string field, string where, object param = null, string orderBy = null)
        {
            string orderByNow = " ORDER BY ID DESC ";
            if (orderBy != null && orderBy != "")
            {
                orderByNow = orderBy;
            }
            totalCount = 0;
            int topNum = pageSize * (pageIndex - 1);
            string sql = string.Format("SELECT {0} FROM navigation_info ", field);
            if (topNum <= 0)
            {
                sql = string.Format("SELECT {0} FROM navigation_info WHERE {1} {2} LIMIT 0,{3}", field, where, orderByNow, pageSize);
            }
            else
            {
                sql = string.Format("SELECT {0} FROM navigation_info WHERE {1} {2} LIMIT {3},{4}", field, where, orderByNow, topNum, pageSize);
            }
            try
            {
                totalCount = new NavigationInfoModel().GetCount(string.Format("SELECT COUNT(*) FROM navigation_info WHERE {0}", where), param);
                return new PagedList<NavigationInfoModel>(new NavigationInfoModel().GetList(sql, param), pageIndex, pageSize, totalCount);
            }
            catch (Exception e)
            {
                log.Error("获取分页数据失败", e);
                return null;
            }
        }

        #endregion

        #region Hyperemia
        public override BaseEntity Clone()
        {
            return this as BaseEntity;
        }
        #endregion
    }
}

