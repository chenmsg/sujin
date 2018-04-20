using ITOrm.Ms.Models.Helper;
using ITOrm.Ms.Models.Logging;
using ITOrm.Ms.Models.Specification;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace ITOrm.Ms.Models.Context
{
    /// <summary>
    /// 实体操作 这是个泛型,T必须是BaseEntity或者他的子类,而且必须有个无参构造函数.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseEntityAction123321<T> : BaseEntity where T : BaseEntity, new()
    {
        public static ILogger log = LogManager.GetCurrentClassLogger();

        #region ==========添加数据
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <returns>返回,成功:true,失败:false</returns>
        public bool Insert()
        {
            bool result = false;
            try
            {
                T entity = base.Clone() as T;
                if (entity != null)
                {
                    using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
                    {
                        db.BaseDbSet.Add(entity);
                        if (db.SaveChanges() > 0)
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (DbException ex)
            {
                log.Error("插入数据失败", ex);
            }
            return result;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <returns>返回影响的行数</returns>
        public int Add()
        {
            int result = 0;
            try
            {
                T entity = base.Clone() as T;
                if (entity != null)
                {
                    using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
                    {
                        db.BaseDbSet.Add(entity);
                        result = db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("插入数据失败", ex);
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
                    using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
                    {
                        DbEntityEntry entry = db.Entry(entity);
                        //if (entry.State == EntityState.Detached)
                        //{
                        entry.State = EntityState.Modified;//通过更改 状态为EntityState.Modified 然后再保存 来实现更新操作
                        //}
                        if (db.SaveChanges() > 0)
                        {
                            result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("更新表数据失败", ex);
                }
            }
            return result;
        }

        /// <summary>
        /// Update的对象，必须通过Single()获取重置属性后操作！传入实体修改，根据传入的实体主健修改，如果是new出来的实体，则要把Single()的对象赋给他才可更新
        /// </summary>
        /// <returns>返回影响的行数</returns>
        public int Edit()
        {
            int result = 0;
            T entity = Clone() as T;
            if (entity != null)
            {
                try
                {
                    using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
                    {
                        DbEntityEntry entry = db.Entry(entity);
                        //if (entry.State == EntityState.Detached)
                        //{
                        entry.State = EntityState.Modified;
                        //}
                        result = db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("更新表数据失败", ex);
                }
            }
            return result;
        }

        public int Update(BaseEntityDbContext<T> db, T item)
        {
            try
            {
                DbEntityEntry entry = db.Entry(item);
                if (entry.State == EntityState.Detached)
                {
                    entry.State = EntityState.Modified;
                }
                return db.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error("更新表数据失败", ex);
            }
            return -1;
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
                    using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
                    {
                        db.Entry(entity).State = EntityState.Deleted;
                        if (db.SaveChanges() > 0)
                        {
                            result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("删除表数据失败", ex);
                }
            }
            return result;
        }

        /// <summary>
        /// 删除对象,lamda表达式删除
        /// </summary>
        /// <param name="predicate">Linq表达式</param>
        /// <returns>返回,成功:true,失败:false</returns>
        public bool Delete(Expression<Func<T, bool>> predicate)
        {
            bool result = false;
            T entity = new T();
            if (predicate != null)
            {
                try
                {
                    using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
                    {
                        entity = db.BaseDbSet.FirstOrDefault(predicate);
                        if (entity != null)
                        {
                            db.Entry(entity).State = EntityState.Deleted;
                            if (db.SaveChanges() > 0)
                            {
                                result = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("删除表数据失败", ex);
                }
            }
            return result;
        }

        /// <summary>
        /// 删除对象,根据ID删除
        /// </summary>
        /// <param name="predicate">Linq表达式</param>
        /// <returns>返回,成功:true,失败:false</returns>
        public bool Delete(int id)
        {
            bool result = false;
            //T entity = new T();
            //if (id > 0)
            //{
            //    Expression<Func<T, bool>> predicate = o => o.ID == id;
            //    if (predicate != null)
            //    {
            //        try
            //        {
            //            using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
            //            {
            //                entity = db.BaseDbSet.FirstOrDefault(predicate);
            //                if (entity != null)
            //                {
            //                    db.Entry(entity).State = EntityState.Deleted;
            //                    if (db.SaveChanges() > 0)
            //                    {
            //                        result = true;
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            log.Error("删除表数据失败", ex);
            //        }
            //    }
            //}
            return result;
        }


        #endregion

        #region ==========查询数据
        /// <summary>
        /// 根据lamda表达式获取单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Single(Expression<Func<T, bool>> predicate)
        {
            T entity = new T();
            if (predicate != null)
            {
                try
                {
                    using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
                    {
                        entity = db.BaseDbSet.FirstOrDefault(predicate);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("获取失败", ex);
                }
            }
            return entity;
        }

        /// <summary>
        /// 根据主键ID取得单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Single(int id)
        {
            T entity = new T();
            //if (id > 0)
            //{
            //    Expression<Func<T, bool>> predicate = o => o.ID == id;
            //    if (predicate != null)
            //    {
            //        try
            //        {
            //            using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
            //            {
            //                entity = db.BaseDbSet.FirstOrDefault(predicate);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            log.Error("获取失败", ex);
            //        }
            //    }
            //}
            return entity;
        }


        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="predicate">Linq表达式</param>
        /// <returns></returns>
        public List<T> GetQuery(Expression<Func<T, bool>> predicate)
        {
            List<T> list = null;
            if (predicate != null)
            {
                try
                {
                    using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
                    {
                        list = db.BaseDbSet.Where(predicate).ToList();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("获取失败", ex);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="top">前几条，默认排序</param>
        /// <param name="predicate">表达式</param>
        /// <returns></returns>
        public List<T> GetQuery(int top, Expression<Func<T, bool>> predicate)
        {
            List<T> list = null;
            if (predicate != null)
            {
                try
                {
                    using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
                    {
                        list = db.BaseDbSet.Where(predicate).Take(top).ToList();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("获取失败", ex);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="predicate">表达式</param>
        /// <param name="sortBy">排序字段</param>
        /// <param name="desc">默认True为降序,False升序</param>
        /// <returns></returns>
        public List<T> GetQuery(Expression<Func<T, bool>> predicate, Func<T, object> sortBy, bool desc = true)
        {
            List<T> list = null;
            if (predicate != null)
            {
                try
                {
                    using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
                    {
                        if (desc)
                        {
                            list = db.BaseDbSet.Where(predicate).OrderByDescending(sortBy).ToList();
                        }
                        else
                        {
                            list = db.BaseDbSet.Where(predicate).OrderBy(sortBy).ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("获取失败", ex);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="top">前几条，默认排序</param>
        /// <param name="predicate">表达式</param>
        /// <param name="sortBy">排序字段</param>
        /// <param name="desc">默认True为降序,False升序</param>
        /// <returns></returns>
        public List<T> GetQuery(int top, Expression<Func<T, bool>> predicate, Func<T, object> sortBy, bool desc = true)
        {
            List<T> list = null;
            if (predicate != null)
            {
                try
                {
                    using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
                    {
                        if (desc)
                        {
                            list = db.BaseDbSet.Where(predicate).OrderByDescending(sortBy).Take(top).ToList();
                        }
                        else
                        {
                            list = db.BaseDbSet.Where(predicate).OrderBy(sortBy).Take(top).ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("获取失败", ex);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取分页数据,返回泛型实体PagedList
        /// </summary>
        /// <param name="predicate">Linq表达式[条件查询]</param>
        /// <param name="pageIndex">当前索引的页面</param>
        /// <param name="pageSize">每一页，页面的行数.</param>
        /// <param name="sortBy">Linq表达式[排序字段]</param>
        /// <param name="desc">默认True为降序</param>
        /// <returns>返回泛型实体PagedList</returns>
        public List<T> GetPaged(Func<T, bool> predicate, int pageIndex, int pageSize, Func<T, object> sortBy, bool desc = true)
        {
            List<T> list = null;
            int totalCount = 0;
            if (predicate != null)
            {
                try
                {
                    using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
                    {
                        if (desc)
                        {
                            list = db.BaseDbSet.Where(predicate).OrderByDescending(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = db.BaseDbSet.Where(predicate).OrderBy(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        totalCount = db.BaseDbSet.Where(predicate).Count();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("获取失败", ex);
                }
            }
            return list;
        }

        /// <summary>
        /// 根据自定义条件查询数据
        /// </summary>
        /// <typeparam name="PEntity"></typeparam>
        /// <param name="specification"></param>
        /// <param name="sortBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public List<T> GetPaged(ISpecification<T> specification, int pageIndex, int pageSize, Func<T, object> sortBy, bool desc = true)
        {
            List<T> list = null;
            int totalCount = 0;
            if (specification != null)
            {
                try
                {
                    using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
                    {
                        if (desc)
                        {
                            list = db.BaseDbSet.Where(specification.Predicate).OrderByDescending(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = db.BaseDbSet.Where(specification.Predicate).OrderBy(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        totalCount = db.BaseDbSet.Where(specification.Predicate).Count();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("获取失败", ex);
                }
            }
            return list;
        }

        /// <summary>
        /// 根据自定义条件查询数据
        /// </summary>
        /// <typeparam name="PEntity"></typeparam>
        /// <param name="specification"></param>
        /// <param name="sortBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public List<T> GetPaged(ISpecification<T> specification, int pageIndex, int pageSize, out int totalCount, Func<T, object> sortBy, bool desc = true)
        {
            List<T> list = null;
            totalCount = 0;
            if (specification != null)
            {
                try
                {
                    using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
                    {
                        if (desc)
                        {
                            list = db.BaseDbSet.Where(specification.Predicate).OrderByDescending(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            list = db.BaseDbSet.Where(specification.Predicate).OrderBy(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        totalCount = db.BaseDbSet.Where(specification.Predicate).Count();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("获取失败", ex);
                }
            }
            return list;
        }

        /// <summary>
        /// 按指定条件排序分页，返回指定字段
        /// </summary>
        /// <typeparam name="PEntity">实体</typeparam>
        /// <typeparam name="PFields">返回对象</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="fields">返回字段</param>
        /// <param name="sortBy">排序</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="count">总记录数</param>
        /// <param name="desc">是否是降序</param>
        /// <returns></returns>
        //public virtual PagedList<T> GetPagedFields<TEntity>(ISpecification<T> specification, Func<T,T> fields, int pageIndex, int pageSize, Func<T, object> sortBy, bool desc = true)
        //{
        //    List<T> list = null;
        //    int totalCount = 0;
        //    if (specification != null)
        //    {
        //        try
        //        {
        //            using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
        //            {
        //                if (desc)
        //                {
        //                    list = db.BaseDbSet.Where(specification.Predicate).OrderByDescending(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(fields).ToList();
        //                }
        //                else
        //                {
        //                    list = db.BaseDbSet.Where(specification.Predicate).OrderBy(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        //                }
        //                totalCount = db.BaseDbSet.Where(specification.Predicate).Count();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            string msg = "获取失败!";
        //            log.Error(msg, ex);
        //            throw new Exception(msg, ex);
        //        }
        //    }
        //    return new PagedList<T>(list, pageIndex, pageSize, totalCount);
        //}

        #endregion

        #region ========Ajax Form Submit

        /// <summary>
        /// 配合前台ajax_form_submit.js文件的提交方式，封装成简易版添加，更改方法
        /// </summary>
        /// <param name="rval">表单集合字符串</param>
        /// <returns></returns>
        public string AjaxEdit(string rval)
        {
            string result = "操作有误";
            int id = Util.StringToInt(rval.GetFromValue("ID"), 0);
            //try
            //{
            //    T entity = base.Clone() as T;
            //    if (id == 0)//添加操作
            //    {
            //        if (entity != null)
            //        {
            //            entity = rval.ConvertToModel(entity);//赋值
            //            using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
            //            {
            //                db.BaseDbSet.Add(entity);
            //                if (db.SaveChanges() > 0)
            //                {
            //                    result = "保存成功";
            //                }
            //            }
            //        }
            //    }
            //    else if (id > 0)//更新操作
            //    {
            //        using (BaseEntityDbContext<T> db = new BaseEntityDbContext<T>())
            //        {
            //            entity = db.BaseDbSet.FirstOrDefault(o => o.ID == id);//查询
            //            entity = rval.ConvertToModel(entity);//赋值
            //            DbEntityEntry entry = db.Entry(entity);
            //            //if (entry.State == EntityState.Detached)
            //            //{
            //            entry.State = EntityState.Modified;//通过更改 状态为EntityState.Modified 然后再保存 来实现更新操作
            //            //}
            //            result = "您没有做任何更改";
            //            if (db.SaveChanges() > 0)
            //            {
            //                result = "保存成功";
            //            }
            //            else
            //            {
            //                result = "保存时遇到问题";
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    result = "操作有误" + ex.Message;
            //    log.Error("操作有误", ex);
            //    return result;
            //}
            return result;
        }

        #endregion

    }
}
