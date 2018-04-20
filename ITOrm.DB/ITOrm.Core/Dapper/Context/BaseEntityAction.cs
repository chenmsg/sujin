using ITOrm.Core.Logging;
using ITOrm.Core.Memcached;
using ITOrm.Core.Memcached.Impl;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;
namespace ITOrm.Core.Dapper.Context
{

    /// <summary>
    /// 实体操作 这是个泛型,T必须是BaseEntity或者他的子类,而且必须有个无参构造函数.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseEntityAction<T> : BaseEntity where T : BaseEntity
    {
        public static ILogger log = LogManager.GetCurrentClassLogger();

        #region ==========添加数据
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <returns>返回,成功:true,失败:false</returns>
        public int Insert(T entity)
        {
            int result = 0;
            if (entity != null)
            {
                string tableName = (typeof(T).GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic).Name;
                try
                {
                    using (SqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        result = connection.Insert<T>(entity);
                    }
                }
                catch (DbException e)
                {
                    log.Error($"插入数据失败:Table:{tableName},Id:{JsonConvert.SerializeObject(entity)}", e);
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
        public bool Update(T entity)
        {
            bool result = false;
            if (entity != null)
            {
                string tableName = (typeof(T).GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic).Name;
                try
                {
                    using (SqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        result = connection.Update<T>(entity);
                    }
                }
                catch (Exception e)
                {
                    log.Error($"更新数据失败:Table:{tableName},Id:{JsonConvert.SerializeObject(entity)}", e);
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
        public bool Delete(T entity)
        {
            bool result = false;
            if (entity != null)
            {
                string tableName = (typeof(T).GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic).Name;
                try
                {
                    using (SqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        result = connection.Delete<T>(entity);
                    }
                }
                catch (Exception e)
                {
                    log.Error($"删除数据失败:Table:{tableName},Id:{JsonConvert.SerializeObject(entity)}", e);
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
            if (id > 0)
            {
                string tableName = (typeof(T).GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic).Name;
                string sql = string.Format("delete from {0} where {1} = @{1}", tableName, name);
                try
                {
                    using (SqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            result = connection.Execute(sql, new { id }) > 0;
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error($"删除数据失败:sql:{sql}", e);
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
        public T Single(int id, string name = "ID")
        {
            T entity = Clone() as T;
            if (id > 0)
            {
                string tableName = (typeof(T).GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic).Name;
                string sql = string.Format("select * from {0} WITH(NOLOCK) where {1} = @ID", tableName, name);
                try
                {
                    using (SqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            entity = connection.Query<T>(sql, new { id }).FirstOrDefault();
                            
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error($"查询单一数据失败:sql:{sql}", e);
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
        public int Single(string where, object param = null , string key="")
        {
            //T entity = Clone() as T;
            int result = 0;
            if (!string.IsNullOrEmpty(where))
            {
                string tableName = (typeof(T).GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic).Name;
                string sql = string.Format("select " + key + " from {0} WITH(NOLOCK) where {1} ", tableName, where);
                try
                {
                   
                    using (SqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        result = connection.Query<int>(sql, param).FirstOrDefault();
                    }
                }
                catch (Exception e)
                {
                    log.Error($"查询单一数据失败:sql:{sql}", e);
                }
            }
            return result;
        }

        #endregion

        #region ==========查询数据

        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="sql">SQL完整语句</param>
        /// <param name="param">参数化对象</param>
        /// <returns></returns>
        public int Count(string sql, object param = null)
        {
            int totalCount = 0;
            if (!string.IsNullOrEmpty(sql))
            {
                try
                {
                    using (SqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        totalCount = connection.ExecuteScalar<int>(sql, param, null, null, null);
                    }
                }
                catch (Exception e)
                {
                    log.Error($"获取数据条数失败:sql:{sql}", e);
                }
            }
            return totalCount;
        }

        /// <summary>
        /// 根据SQL语句查询一个结果值
        /// </summary>
        /// <param name="sql">SQL完整语句</param>
        /// <param name="param">参数化对象</param>
        /// <returns></returns>
        public object GetScalar(string sql, object param = null)
        {
            object totalCount = null;
            if (!string.IsNullOrEmpty(sql))
            {
                try
                {
                    using (SqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        totalCount = connection.ExecuteScalar<object>(sql, param, null, null, null);
                    }
                }
                catch (Exception e)
                {
                    log.Error($"获取结果值失败:sql:{sql}", e);
                }
            }
            return totalCount;
        }

        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="sql">SQL完整语句</param>
        /// <param name="param">参数化对象</param>
        /// <returns></returns>
        public List<int> GetQuery(string sql, object param = null)
        {
            List<int> list = null;
            if (!string.IsNullOrEmpty(sql))
            {
                try
                {
                    using (SqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        list = connection.Query<int>(sql, param).ToList();
                    }
                }
                catch (Exception e)
                {
                    log.Error($"获取列表数据失败：sql:{sql}", e);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="sql">SQL完整语句</param>
        /// <param name="param">参数化对象</param>
        /// <returns></returns>
        public List<T> GetQueryTest(string sql, object param = null)
        {
            List<T> list = null;
            if (!string.IsNullOrEmpty(sql))
            {
                try
                {
                    using (SqlConnection connection = RunConnection.GetOpenConnection())
                    {
                        list = connection.Query<T>(sql, param).ToList();
                    }
                }
                catch (Exception e)
                {
                    log.Error($"获取列表数据失败sql:{sql}", e);
                }
            }
            return list;
        }

        /// <summary>
        /// 根据SQL获取DataReader数据
        /// </summary>
        /// <param name="sql">SQL完整语句</param>
        /// <param name="param">参数化对象</param>
        /// <returns></returns>
        //public IDataReader GetDataReader(string sql, object param = null)
        //{
        //    IDataReader dataReader = null;
        //    if (!string.IsNullOrEmpty(sql))
        //    {
        //        try
        //        {
        //            using (MySqlConnection connection = RunConnection.GetOpenConnection())
        //            {
        //                dataReader = connection.ExecuteReader(sql, param);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            log.Error("获取列表数据失败", e);
        //        }
        //    }
        //    return dataReader;
        //}

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


        #region ==========缓存
        protected virtual IMemcachedDao Memcache
        {
            get
            {
                return MemcachedDao.GetInstance();
            }
        }
        protected virtual TimeSpan DurationCache
        {
            get
            {
                var duration = new TimeSpan(/*天*/365,/*小时*/ 0,/*分钟*/ 0,/*秒*/ 0, /*毫秒*/0);
                if (!string.IsNullOrEmpty(configDuration))
                {
                    var conf = configDuration.Split('|');
                    try
                    {
                        var confVal = int.Parse(conf[0]);
                        switch (conf[1].ToLower())
                        {
                            case "days":
                                duration = new TimeSpan(/*天*/confVal,/*小时*/ 0,/*分钟*/ 0,/*秒*/ 0, /*毫秒*/0);
                                break;
                            case "hours":
                                duration = new TimeSpan(/*天*/0,/*小时*/ confVal,/*分钟*/ 0,/*秒*/ 0, /*毫秒*/0);
                                break;
                            case "minutes":
                                duration = new TimeSpan(/*天*/0,/*小时*/ 0,/*分钟*/ confVal,/*秒*/ 0, /*毫秒*/0);
                                break;
                            case "seconds":
                                duration = new TimeSpan(/*天*/0,/*小时*/ 0,/*分钟*/ 0,/*秒*/ confVal, /*毫秒*/0);
                                break;
                            case "milliseconds":
                                duration = new TimeSpan(/*天*/0,/*小时*/ 0,/*分钟*/ 0,/*秒*/ 0, /*毫秒*/confVal);
                                break;
                            default:
                                break;
                        }
                    }
                    catch { }
                }
                return duration;
            }
        }
        private string configDuration = "";
        #endregion

    }
}