using Clump.Core.Dapper;
using Clump.Core.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Clump.Data.Models.Host.Context
{
    /// <summary>
    /// 实体操作 这是个泛型,T必须是BaseEntity或者他的子类,而且必须有个无参构造函数.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseEntityAction<T> : BaseEntity where T : BaseEntity, new()
    {
        public static ILogger log = LogManager.GetCurrentClassLogger();

        #region ==========添加数据
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <returns>返回,成功:true,失败:false</returns>
        public int Insert()
        {
            int result = 0;
            T entity = Clone() as T;
            if (entity != null)
            {
                try
                {
                    using (MySqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        //using (MySqlTransaction transaction = connection.BeginTransaction())//事务,对目前的封装来说,不适合使用
                        //{
                        result = connection.Insert<T>(entity);
                        //result = connection.Insert<T>(entity, transaction);
                        //transaction.Commit();
                        //}
                    }
                }
                catch (DbException e)
                {
                    log.Error("插入数据失败", e);
                }
            }
            return result;
        }

        #endregion

        #region ==========更改数据

        /// <summary>
        /// Update的对象，必须通过Single()获取重置属性后操作！传入实体修改，根据传入的实体主健修改，如果是new出来的实体，则要把Single()的对象赋给他才可更新
        /// </summary>
        /// <returns>返回,成功:true,失败:false</returns>
        public bool Update()
        {
            bool result = false;
            T entity = Clone() as T;
            if (entity != null)
            {
                try
                {
                    using (MySqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        result = connection.Update<T>(entity);
                    }
                }
                catch (Exception e)
                {
                    log.Error("更新数据失败", e);
                }
            }
            return result;
        }

        #endregion

        #region ==========删除数据

        /// <summary>
        /// Delete，根据实体对象删除
        /// </summary>
        /// <returns>返回,成功:true,失败:false</returns>
        public bool Delete()
        {
            bool result = false;
            T entity = Clone() as T;
            if (entity != null)
            {
                try
                {
                    using (MySqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        result = connection.Delete<T>(entity);
                    }
                }
                catch (Exception e)
                {
                    log.Error("删除数据失败", e);
                }
            }
            return result;
        }

        /// <summary>
        /// 根据主键ID删除数据
        /// </summary>
        /// <param name="id">主键ID的值</param>
        /// <param name="name">主键ID的数据库字段名称，默认是ID</param>
        /// <returns></returns>
        public bool Delete(int id, string name = "ID")
        {
            bool result = false;
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
                            result = connection.Execute(string.Format("delete from {0} where {1} = @{1}", tableName, name), new { id }) > 0;
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error("删除数据失败", e);
                }
            }
            return result;
        }

        #endregion

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
