using System;
using System.Reflection;
using ITOrm.EF.Models.Reflection;
using System.Collections.Generic;
using System.Linq;
using ITOrm.EF.Models.Helper;
namespace ITOrm.EF.Models.Context
{
    /// <summary>
    /// 数据库:执行表单业务逻辑，和自定义的SQL语句
    /// </summary>
    public static class CustomSQL
    {

        #region ==========自定义的SQL语句
        /// <summary>
        /// 执行SQL语句，返回影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int Execute(string sql)
        {
            return Execute(sql, new object[0]);
        }

        /// <summary>
        /// 执行SQL语句，返回影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int Execute(string sql, params object[] parameters)
        {
            int result = 0;
            try
            {
                using (BaseSqlContext db = new BaseSqlContext())
                {
                    result = db.Database.ExecuteSqlCommand(sql, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("操作数据失败!", ex);
            }
            return result;
        }

        /// <summary>
        /// 执行SQL语句，返回集合列表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<T> GetQuery<T>(string sql)
        {
            return GetQuery<T>(sql, new object[0]);
        }

        /// <summary>
        /// 执行SQL语句，返回集合列表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<T> GetQuery<T>(string sql, params object[] parameters)
        {
            List<T> enumerable = null;
            try
            {
                using (BaseSqlContext db = new BaseSqlContext())
                {
                    enumerable = db.Database.SqlQuery<T>(sql, parameters).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("操作数据失败!", ex);
            }
            return enumerable;
        }

        /// <summary>
        /// 执行SQL语句，返回行数 [sql写法：select COUNT(*) ...] 限制：sql 语句中 select name from tableName ， 只能存在name一个字段
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int GetCount(string sql)
        {
            return GetCount(sql, new object[0]);
        }

        /// <summary>
        /// 执行SQL语句，返回行数 [sql写法：select COUNT(*) ...] 限制：sql 语句中 select name from tableName ， 只能存在name一个字段
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int GetCount(string sql, params object[] parameters)
        {
            List<string> listString = GetList(sql, parameters);
            if (listString != null && listString.Count > 0)
            {
                return listString.Count;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回List<string> 集合，限制：sql 语句中 select name from tableName ， 只能存在name一个字段
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<string> GetList(string sql, params object[] parameters)
        {
            List<string> listString = null;
            try
            {
                using (BaseSqlContext db = new BaseSqlContext())
                {
                    listString = db.Database.SqlQuery<string>(sql).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("操作数据失败!", ex);
            }
            return listString;
        }

        #endregion

        /// <summary>
        /// 根据实体名称，获取此实体字段名称的数组List<string>
        /// </summary>
        /// <param name="typeName">完整实体名称， [命名空间.实体名称]</param>
        /// <returns></returns>
        public static List<string> GetEntityFieldList(this string tableName)
        {
            List<string> list = new List<string>();
            tableName = string.Format("{0}.{1}", ConfigHelper.GetAppSettings("EntitySpaceName"), tableName);
            TypeX EntityType = TypeX.Create(TypeX.GetType(tableName, true));//根据类的完整名称建立类的类型,用于动态创建类 如： Clump.Mobile.Models.NewsInfo
            Object objEntity = EntityType.CreateInstance();//动态建立类的对象 实体类的对象Object objEntity = EntityType.CreateInstance(true);意思是在创建实体对象时 A a = new A(true)
            PropertyInfo[] props = objEntity.GetType().GetProperties();//获取此对象的，字段集合
            object propertValue = String.Empty;
            foreach (PropertyInfo item in props)
            {
                list.Add(item.Name);
            }
            return list;
        }
    }
}