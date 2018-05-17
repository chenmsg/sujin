//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的...
//    生成时间 2018-05-17 16:41:18 By CClump
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using ITOrm.Core.Utility.PagerWebdiyer;
using ITOrm.Host.Models;
using ITOrm.Core.Dapper.Context;
namespace ITOrm.Host.DAL
{
    /// <summary>
    /// 备注接口
    /// </summary>
    public interface IAccountRecordDAL
    {
		#region ==========增删改数据
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <returns>返回,成功:true,失败:false</returns>
        int Insert(AccountRecord entity);

        /// <summary>
        /// Update的对象，必须通过Single()获取重置属性后操作！传入实体修改，根据传入的实体主健修改，如果是new出来的实体，则要把Single()的对象赋给他才可更新
        /// </summary>
        /// <returns>返回,成功:true,失败:false</returns>
        bool Update(AccountRecord entity);

        /// <summary>
        /// Delete，根据实体对象删除
        /// </summary>
        /// <returns>返回,成功:true,失败:false</returns>
        bool Delete(AccountRecord entity);

        /// <summary>
        /// 根据主键ID删除数据
        /// </summary>
        /// <param name="id">主键ID的值</param>
        /// <param name="name">主键ID的数据库字段名称，默认是ID</param>
        /// <returns></returns>
        bool Delete(int id);

        #endregion

		#region ==========缓存操作
				/// <summary>
        /// 清理缓存
        /// </summary>
        /// <param name="Id"></param>
        void RemoveCache(long Id);

		#endregion
		
        #region ==========查询单一实体

        /// <summary>
        /// 根据主键ID取得单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="id">实体的主键ID</param>
        /// <returns>AccountRecord实体对象</returns>
        AccountRecord Single(int id);

        /// <summary>
        /// 根据where条件取得单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <returns>AccountRecord实体对象</returns>
        AccountRecord Single(string where, object param = null);

        #endregion

        #region ==========查询列表集合

        /// <summary>
        /// 查询List集合
        /// </summary>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <param name="orderBy">排序语句[例子：order by id desc ]</param>
        /// <returns>List集合</returns>
        List<AccountRecord> GetQuery(string where, object param = null, string orderBy = null);



        /// <summary>
        /// 查询List集合的前几条
        /// </summary>
        /// <param name="top">前多少条</param>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <param name="orderBy">排序语句[例子：order by id desc ]</param>
        /// <returns>List集合</returns>
        List<AccountRecord> GetQuery(int top, string where, object param = null, string orderBy = null);


        /// <summary>
        /// 根据条件获取条数
        /// </summary>
        /// <param name="where">sql条件</param>
        /// <param name="param">参数化</param>
        /// <returns></returns>
        int Count(string where, object param = null);


		/// <summary>
        /// 根据sql条件查询单一字段内容
        /// </summary>
        /// <param name="where">sql条件</param>
        /// <param name="param">参数化</param>
        /// <returns></returns>
        object Scalar(string sql, object param = null);

        
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
        List<AccountRecord> GetPaged(int pageSize, int pageIndex, out int totalCount, string where, object param = null, string orderBy = null);

        


        #endregion


    }

    /// <summary>
    /// 备注
    /// </summary>
    public class AccountRecordDAL : BaseEntityAction<AccountRecord>, IAccountRecordDAL
    {
	    #region ==========增删改数据

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <returns>返回,成功:true,失败:false</returns>
        public new  int Insert(AccountRecord entity)
        {
			if (entity == null) throw new ArgumentNullException("obj");
            var Id = base.Insert(entity);
            if (Id > 0)
            {
                Single(entity.ID);
                return Id;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Update的对象，必须通过Single()获取重置属性后操作！传入实体修改，根据传入的实体主健修改，如果是new出来的实体，则要把Single()的对象赋给他才可更新
        /// </summary>
        /// <returns>返回,成功:true,失败:false</returns>
        public new bool Update(AccountRecord entity)
        {
			if (entity == null) throw new ArgumentNullException("obj");
            var result = base.Update(entity);
            if (result)
            {
                RemoveCache(entity.ID);
				Single(entity.ID);
                return result;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Delete，根据实体对象删除
        /// </summary>
        /// <returns>返回,成功:true,失败:false</returns>
        public new  bool Delete(AccountRecord entity)
        {
            if (entity == null) throw new ArgumentNullException("obj");
            var result = base.Delete(entity);
            if (result)
            {
                RemoveCache(entity.ID);
                return result;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据主键ID删除数据
        /// </summary>
        /// <param name="id">主键ID的值</param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            if (id>0) throw new ArgumentNullException("obj");
            var result = base.Delete(id, "ID");
            if (result)
            {
                RemoveCache(id);
                return result;
            }
            else
            {
                return false;
            }
        }

        #endregion
		#region ==========查询单一实体


        /// <summary>
        /// 根据主键ID取得单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="id">实体的主键ID</param>
        /// <returns>AccountRecord实体对象</returns>
        public AccountRecord Single(int id)
        {
					return base.Single(id, "ID");
		
		}


        /// <summary>
        /// 根据where条件取得单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <returns>AccountRecord实体对象</returns>
        public   AccountRecord Single(string where, object param = null)
        {
			
			var	result = base.Single(where, param,"ID");
            return Single(result);

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
        public List<AccountRecord> GetQuery(string where, object param = null, string orderBy = null)
        {
			            var result = base.GetQuery(string.Format("SELECT ID FROM AccountRecord WITH(NOLOCK) WHERE {0} {1}", where, orderBy), param);
            return MappingCacheEntity(result);
			        }


        /// <summary>
        /// 查询List集合的前几条
        /// </summary>
        /// <param name="top">前多少条</param>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <param name="orderBy">排序语句[例子：order by id desc ]</param>
        /// <returns>List集合</returns>
        public List<AccountRecord> GetQuery(int top, string where, object param = null, string orderBy = null)
        {
			            var result = base.GetQuery(string.Format("SELECT top {2} ID FROM AccountRecord WITH(NOLOCK) WHERE {0} {1} ", where, orderBy, top), param);
			return MappingCacheEntity(result);
			        }


        /// <summary>
        /// 根据条件获取条数
        /// </summary>
        /// <param name="where">sql条件</param>
        /// <param name="param">参数化</param>
        /// <returns></returns>
        public new int Count(string where, object param = null)
        {
            return base.Count("SELECT COUNT(0) FROM AccountRecord WITH(NOLOCK) WHERE " + where, param);
        }



		/// <summary>
        /// 根据sql条件查询单一字段内容
        /// </summary>
        /// <param name="where">sql条件</param>
        /// <param name="param">参数化</param>
        /// <returns></returns>
        public object Scalar(string sql, object param = null)
        {
            return base.GetScalar(sql, param);
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
        public List<AccountRecord> GetPaged(int pageSize, int pageIndex, out int totalCount, string where, object param = null, string orderBy = null)
        {
            string orderByNow = " ORDER BY ID DESC ";
            if (!string.IsNullOrEmpty(orderBy))
            {
                orderByNow = orderBy;
            }
            totalCount = 0;
            int topNum = pageSize * (pageIndex - 1) + 1;
			string field="ID";
            string sql = string.Format("SELECT {0} FROM AccountRecord ", field);
            if (topNum <= 1)
            {
				sql = string.Format("SELECT {0} FROM (SELECT ROW_NUMBER() OVER({1}) as 'RowNumber', {0} from AccountRecord WITH(NOLOCK) {3} ) as temp WHERE (RowNumber between 0 and {2} ) ", field, orderByNow, pageSize, (!string.IsNullOrEmpty(where) ? " WHERE " + where : ""));
            }
            else
            {
                sql = string.Format("SELECT {0} FROM (SELECT ROW_NUMBER() OVER({1}) as 'RowNumber', {0} from AccountRecord WITH(NOLOCK) {4} ) as temp WHERE (RowNumber between {2} and {3} ) ", field, orderByNow, topNum, (topNum + pageSize-1), (!string.IsNullOrEmpty(where) ? " WHERE " + where : ""));
            }
            try
            {
                totalCount = Count(where, param);
								var result = base.GetQuery(sql, param);
                return MappingCacheEntity(result);
				                
            }
            catch (Exception e)
            {
                log.Error("获取分页数据失败", e);
                return null;
            }
        }








        #endregion

		
		#region ==========缓存操作
		/// <summary>
        /// Memcache Key编码
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        private string EncodeKey(string paramName, object paramValue)
        {
            return Memcache.EncodeKey("SujinDB", "AccountRecord", paramName, paramValue);
        }

		/// <summary>
        /// 清理缓存
        /// </summary>
        /// <param name="Id"></param>
        public void RemoveCache(long Id)
        {
            var key = EncodeKey("ID", Id);
            Memcache.Remove(key);
        }

		/// <summary>
        /// 批量操作缓存
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>        
        private List<AccountRecord> MappingCacheEntity(List<int> result)
        {
            //exists result is null(empty)
            if (result != null && result.Count > 0)
            {
                String key = string.Empty;
                AccountRecord cache = null;
                List<AccountRecord> result_cache = new List<AccountRecord>(result.Count);

                foreach (var item in result)
                {
                    key = EncodeKey("ID", item);
                    cache = Memcache.Get(key) as AccountRecord;
                    if (cache == null)
                    {
                        cache = Single(item);
                        Memcache.Store(key, cache, DurationCache);
                    }

                    if (cache != null)
                        result_cache.Add(cache);
                }
                return result_cache;
            }
            return null;
        }
		#endregion

		    }
}

