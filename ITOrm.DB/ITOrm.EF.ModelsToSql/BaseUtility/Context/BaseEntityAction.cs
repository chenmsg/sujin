using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using ITOrm.EF.Models.Logging;
namespace ITOrm.EF.Models.Context
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
                        entry.State = System.Data.Entity.EntityState.Modified;//通过更改 状态为EntityState.Modified 然后再保存 来实现更新操作
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
                        entry.State = System.Data.Entity.EntityState.Modified;
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
                        db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
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
                            db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
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
            //            entity = db.BaseDbSet.FirstOrDefault(predicate);
            //            if (entity != null)
            //            {
            //                db.Entry(entity).State = EntityState.Deleted;
            //                if (db.SaveChanges() > 0)
            //                {
            //                    result = true;
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
    }
}
