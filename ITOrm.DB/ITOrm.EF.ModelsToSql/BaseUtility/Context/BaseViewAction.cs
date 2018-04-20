using ITOrm.EF.Models.Logging;
using ITOrm.EF.Models.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
namespace ITOrm.EF.Models.Context
{
    /// <summary>
    /// 实体操作 这是个泛型,T必须是BaseView或者他的子类,而且必须有个无参构造函数.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseViewAction<T> : BaseView where T : BaseView, new()
    {
        public static ILogger log = LogManager.GetCurrentClassLogger();

        #region ==========查询数据
        /// <summary>
        /// 根据lamda表达式获取单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        //public T Single(Expression<Func<T, bool>> predicate)
        //{
        //    T entity = new T();
        //    if (predicate != null)
        //    {
        //        try
        //        {
        //            using (BaseViewDbContext<T> db = new BaseViewDbContext<T>())
        //            {
        //                entity = db.BaseDbSet.FirstOrDefault(predicate);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            log.Error("获取失败", ex);
        //            //throw new Exception("获取失败!", ex);
        //        }
        //    }
        //    return entity;
        //}

        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="predicate">Linq表达式</param>
        /// <returns></returns>
        //public List<T> GetQuery(Expression<Func<T, bool>> predicate)
        //{
        //    List<T> list = null;
        //    if (predicate != null)
        //    {
        //        try
        //        {
        //            using (BaseViewDbContext<T> db = new BaseViewDbContext<T>())
        //            {
        //                list = db.BaseDbSet.Where(predicate).ToList();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            log.Error("获取失败", ex);
        //            //throw new Exception("获取失败!", ex);
        //        }
        //    }
        //    return list;
        //}

        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="top">前几条，默认排序</param>
        /// <param name="predicate">表达式</param>
        /// <returns></returns>
        //public List<T> GetQuery(int top, Expression<Func<T, bool>> predicate)
        //{
        //    List<T> list = null;
        //    if (predicate != null)
        //    {
        //        try
        //        {
        //            using (BaseViewDbContext<T> db = new BaseViewDbContext<T>())
        //            {
        //                list = db.BaseDbSet.Where(predicate).Take(top).ToList();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            log.Error("获取失败", ex);
        //            //throw new Exception("获取失败!", ex);
        //        }
        //    }
        //    return list;
        //}

        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="predicate">表达式</param>
        /// <param name="sortBy">排序字段</param>
        /// <param name="desc">默认True为降序,False升序</param>
        /// <returns></returns>
        //public List<T> GetQuery(Expression<Func<T, bool>> predicate, Func<T, object> sortBy, bool desc = true)
        //{
        //    List<T> list = null;
        //    if (predicate != null)
        //    {
        //        try
        //        {
        //            using (BaseViewDbContext<T> db = new BaseViewDbContext<T>())
        //            {
        //                if (desc)
        //                {
        //                    list = db.BaseDbSet.Where(predicate).OrderByDescending(sortBy).ToList();
        //                }
        //                else
        //                {
        //                    list = db.BaseDbSet.Where(predicate).OrderBy(sortBy).ToList();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            log.Error("获取失败", ex);
        //            //throw new Exception("获取失败!", ex);
        //        }
        //    }
        //    return list;
        //}

        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="top">前几条，默认排序</param>
        /// <param name="predicate">表达式</param>
        /// <param name="sortBy">排序字段</param>
        /// <param name="desc">默认True为降序,False升序</param>
        /// <returns></returns>
        //public List<T> GetQuery(int top, Expression<Func<T, bool>> predicate, Func<T, object> sortBy, bool desc = true)
        //{
        //    List<T> list = null;
        //    if (predicate != null)
        //    {
        //        try
        //        {
        //            using (BaseViewDbContext<T> db = new BaseViewDbContext<T>())
        //            {
        //                if (desc)
        //                {
        //                    list = db.BaseDbSet.Where(predicate).OrderByDescending(sortBy).Take(top).ToList();
        //                }
        //                else
        //                {
        //                    list = db.BaseDbSet.Where(predicate).OrderBy(sortBy).Take(top).ToList();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            log.Error("获取失败", ex);
        //            //throw new Exception("获取失败!", ex);
        //        }
        //    }
        //    return list;
        //}

        /// <summary>
        /// 获取分页数据,返回泛型实体PagedList
        /// </summary>
        /// <param name="predicate">Linq表达式[条件查询]</param>
        /// <param name="pageIndex">当前索引的页面</param>
        /// <param name="pageSize">每一页，页面的行数.</param>
        /// <param name="sortBy">Linq表达式[排序字段]</param>
        /// <param name="desc">默认True为降序</param>
        /// <returns>返回泛型实体PagedList</returns>
        //public List<T> GetPaged(Func<T, bool> predicate, int pageIndex, int pageSize, Func<T, object> sortBy, bool desc = true)
        //{
        //    List<T> list = null;
        //    int totalCount = 0;
        //    if (predicate != null)
        //    {
        //        try
        //        {
        //            using (BaseViewDbContext<T> db = new BaseViewDbContext<T>())
        //            {
        //                if (desc)
        //                {
        //                    list = db.BaseDbSet.Where(predicate).OrderByDescending(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        //                }
        //                else
        //                {
        //                    list = db.BaseDbSet.Where(predicate).OrderBy(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        //                }
        //                totalCount = db.BaseDbSet.Where(predicate).Count();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            log.Error("获取失败", ex);
        //            //throw new Exception("获取失败!", ex);
        //        }
        //    }
        //    return list;
        //}

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
        //public List<T> GetPaged(ISpecification<T> specification, int pageIndex, int pageSize, Func<T, object> sortBy, bool desc = true)
        //{
        //    List<T> list = null;
        //    int totalCount = 0;
        //    if (specification != null)
        //    {
        //        try
        //        {
        //            using (BaseViewDbContext<T> db = new BaseViewDbContext<T>())
        //            {
        //                if (desc)
        //                {
        //                    list = db.BaseDbSet.Where(specification.Predicate).OrderByDescending(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
        //            log.Error("获取失败", ex);
        //            //throw new Exception("获取失败!", ex);
        //        }
        //    }
        //    return list;
        //}

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
        //public List<T> GetPaged(ISpecification<T> specification, int pageIndex, int pageSize, out int totalCount, Func<T, object> sortBy, bool desc = true)
        //{
        //    List<T> list = null;
        //    totalCount = 0;
        //    if (specification != null)
        //    {
        //        try
        //        {
        //            using (BaseViewDbContext<T> db = new BaseViewDbContext<T>())
        //            {
        //                if (desc)
        //                {
        //                    list = db.BaseDbSet.Where(specification.Predicate).OrderByDescending(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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
        //            log.Error("获取失败", ex);
        //            //throw new Exception("获取失败!", ex);
        //        }
        //    }
        //    return list;
        //}

        #endregion
    }
}
