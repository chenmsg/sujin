using Clump.Core.Dapper;
using Clump.Core.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clump.Data.Models.Host.Context
{
    /// <summary>
    /// 实体操作 这是个泛型,T必须是BaseEntity或者他的子类,而且必须有个无参构造函数.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseViewAction<T> : BaseEntity where T : BaseEntity, new()
    {
        public static ILogger log = LogManager.GetCurrentClassLogger();

        #region ==========查询单一实体

        /// <summary>
        /// 根据主键ID查询单一实体数据
        /// </summary>
        /// <param name="id">主键ID的值</param>
        /// <param name="name">主键ID的数据库字段名称，默认是ID</param>
        /// <returns></returns>
        public T Get(int id, string name = "ID")
        {
            T entity = Clone() as T;
            if (id > 0)
            {
                try
                {
                    string tableName = (typeof(T).GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic).Name;
                    using (MySqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            entity = connection.Query<T>(string.Format("select * from {0} where {1} = @{1}", tableName, name), new { id }).FirstOrDefault();
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error("查询单一数据失败", e);
                }
            }
            return entity;
        }

        /// <summary>
        /// 根据主键ID查询单一实体数据
        /// </summary>
        /// <param name="id">主键ID的值</param>
        /// <param name="name">主键ID的数据库字段名称，默认是ID</param>
        /// <returns></returns>
        public T Get(string where, object param = null)
        {
            T entity = Clone() as T;
            if (!string.IsNullOrEmpty(where))
            {
                try
                {
                    string tableName = (typeof(T).GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic).Name;
                    using (MySqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        entity = connection.Query<T>(string.Format("select * from {0} where {1} ", tableName, where), param).FirstOrDefault();
                    }
                }
                catch (Exception e)
                {
                    log.Error("查询单一数据失败", e);
                }
            }
            return entity;
        }

        #endregion

        #region ==========查询数据

        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="specification">满足表达式规范的组合</param>
        /// <returns>DemoInfo实体对象的List集合</returns>
        public int GetCount(string sql, object param = null)
        {
            int totalCount = 0;
            if (!string.IsNullOrEmpty(sql))
            {
                try
                {
                    using (MySqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        totalCount = connection.ExecuteScalar<int>(sql, param, null, null, null);
                    }
                }
                catch (Exception e)
                {
                    log.Error("获取数据条数失败", e);
                }
            }
            return totalCount;
        }

        /// <summary>
        /// 根据SQL语句查询一个结果值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public object GetScalar(string sql, object param = null)
        {
            object totalCount = null;
            if (!string.IsNullOrEmpty(sql))
            {
                try
                {
                    using (MySqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        totalCount = connection.ExecuteScalar<object>(sql, param, null, null, null);
                    }
                }
                catch (Exception e)
                {
                    log.Error("获取结果值失败", e);
                }
            }
            return totalCount;
        }

        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="specification">满足表达式规范的组合</param>
        /// <returns>DemoInfo实体对象的List集合</returns>
        public List<T> GetList(string sql, object param = null)
        {
            T entity = Clone() as T;
            List<T> list = null;
            if (!string.IsNullOrEmpty(sql))
            {
                try
                {
                    using (MySqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        list = connection.Query<T>(sql, param).ToList();
                    }
                }
                catch (Exception e)
                {
                    log.Error("获取列表数据失败", e);
                }
            }
            return list;
        }

        /// <summary>
        /// 根据SQL获IEnumerable<dynamic>数据
        /// </summary>
        /// <param name="sql">SQL完整语句</param>
        /// <param name="param">参数化对象</param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetEnumerable(string sql, object param = null)
        {
            return new DynamicModel().Query(sql, param);
        }

        #endregion

    }
}
