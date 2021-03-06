//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的...
//    生成时间 2015-01-22 17:51:14 By CClump
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using ITOrm.Ms.Models.Context;
using ITOrm.Ms.Models.Helper;
using ITOrm.Ms.Models.Specification;
namespace ITOrm.Ms.Models.Host
{
    /// <summary>
    /// 表: 
    /// </summary>
    [Serializable]
    public class Abc : BaseEntityAction<Abc>
    {
        #region Entity Field
                
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public int id { get; set; }
                        
        /// <summary>
        /// 
        /// </summary>
        [StringLength(50)]
        public string name { get; set; }
        #endregion

        #region Data

        /// <summary>
        /// 数据库名称
        /// </summary>
        public override string DataName { get { return "clump.pro"; } }

        /// <summary>
        /// 表名称
        /// </summary>
        public override string DataTableName { get { return "abc"; } }

        #endregion

        #region Hyperemia
        public override BaseEntity Clone()
        {
            return this as BaseEntity;
        }
        #endregion

        #region ==========查询单一实体

        /// <summary>
        /// 根据lamda表达式获取单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="specification">满足表达式规范的组合</param>
        /// <returns>Abc实体对象</returns>
        public static Abc Single(ISpecification<Abc> specification)
        {
            Abc entity = new Abc();
            if (specification != null)
            {
                try
                {
                    using (BaseEntityDbContext<Abc> db = new BaseEntityDbContext<Abc>())
                    {
                        entity = db.BaseDbSet.FirstOrDefault(specification.Predicate);
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
        /// 根据lamda表达式获取单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="predicate">表达式</param>
        /// <returns>Abc实体对象</returns>
        public static Abc Single(Expression<Func<Abc, bool>> predicate)
        {
            Abc entity = new Abc();
            if (predicate != null)
            {
                try
                {
                    using (BaseEntityDbContext<Abc> db = new BaseEntityDbContext<Abc>())
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
        /// <param name="id">实体的主键ID</param>
        /// <returns>Abc实体对象</returns>
        public static Abc Single(int id)
        {
            Abc entity = new Abc();
            if (id > 0)
            {
                try
                {
                    using (BaseEntityDbContext<Abc> db = new BaseEntityDbContext<Abc>())
                    {
                        entity = db.BaseDbSet.FirstOrDefault(o => o.id == id);					}
                }
                catch (Exception ex)
                {
                    log.Error("获取失败", ex);
                }
            }
            return entity;
        }

        #endregion

        #region ==========查询数据

        /// <summary>
        /// 获取条数
        /// </summary>
        /// <param name="predicate">表达式</param>
        /// <returns></returns>
        public static int GetCount(Expression<Func<Abc, bool>> predicate)
        {
            int count = 0;
            if (predicate != null)
            {
                try
                {
                   using (BaseEntityDbContext<Abc> db = new BaseEntityDbContext<Abc>())
                    {
                        count = db.BaseDbSet.Where(predicate).Count();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("获取失败", ex);
                }
            }
            return count;
        }


		/// <summary>
        /// 获取条数
        /// </summary>
        /// <param name="predicate">表达式</param>
        /// <returns></returns>
        public static int GetCount(ISpecification<Abc> spec)
        {
            int count = 0;
            if (spec != null)
            {
                try
                {
                   using (BaseEntityDbContext<Abc> db = new BaseEntityDbContext<Abc>())
                    {
                        count = db.BaseDbSet.Where(spec.Predicate).Count();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("获取失败", ex);
                }
            }
            return count;
        }


        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="predicate">表达式</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetQuery(Expression<Func<Abc, bool>> predicate)
        {
            return GetQuery(0, predicate, null);
        }

        public static List<Abc> GetQuery(Expression<Func<Abc, bool>> predicate, Func<Abc, object> sortBy, bool desc = true)
        {
            return GetQuery(0, predicate, sortBy, desc);
        }

        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="top">查询前几条，top小于等于0则展示全部</param>
        /// <param name="predicate">表达式</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetQuery(int top, Expression<Func<Abc, bool>> predicate)
        {
            return GetQuery(top, predicate, null);
        }

        /// <summary>
        /// 获取该实体的查询,单排序
        /// </summary>
        /// <param name="top">查询前几条，top小于等于0则展示全部</param>
        /// <param name="predicate">表达式</param>
        /// <param name="sortBy">排序字段</param>
        /// <param name="desc">默认True为降序,False升序</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetQuery(int top, Expression<Func<Abc, bool>> predicate, Func<Abc, object> sortBy, bool desc = true)
        {
            List<Abc> list = null;
            if (predicate != null)
            {
                try
                {
                    using (BaseEntityDbContext<Abc> db = new BaseEntityDbContext<Abc>())
                    {
                        IQueryable<Abc> query = db.BaseDbSet.Where(predicate);//默认排序
                        if (top <= 0 && sortBy == null)//直接返回结果，防止不必要的操作
                        {
                            return query.ToList();
                        }
                        IOrderedEnumerable<Abc> queryOrder = null;
                        if (sortBy != null)//执行排序
                        {
                            if (desc)
                            {
                                queryOrder = query.OrderByDescending(sortBy);
                            }
                            else
                            {
                                queryOrder = query.OrderBy(sortBy);
                            }
                        }

                        if (queryOrder != null)
                        {
                            if (top > 0)//top<=0则展示全部
                            {
                                list = queryOrder.Take(top).ToList();
                            }
                            else
                            {
                                list = queryOrder.ToList();
                            }
                        }
                        else
                        {
                            if (top > 0)//top<=0则展示全部
                            {
                                list = query.Take(top).ToList();
                            }
                            else
                            {
                                list = query.ToList();
                            }
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
        /// 获取该实体的查询,多排序
        /// </summary>
        /// <param name="predicate">表达式</param>
        /// <param name="sortDic">字典存放多个排序字段，Key = 默认True为降序,False升序</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetQuery(Expression<Func<Abc, bool>> predicate, Dictionary<Func<Abc, object>,bool> sortDic)
        {
            return GetQuery(0, predicate, sortDic);
        }

        /// <summary>
        /// 获取该实体的查询,多排序
        /// </summary>
        /// <param name="top">查询前几条，top小于等于0则展示全部</param>
        /// <param name="predicate">表达式</param>
        /// <param name="sortDic">字典存放多个排序字段，Key = 默认True为降序,False升序</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetQuery(int top, Expression<Func<Abc, bool>> predicate, Dictionary<Func<Abc, object>,bool> sortDic)
        {
            List<Abc> list = null;
            if (predicate != null)
            {
                try
                {
                    using (BaseEntityDbContext<Abc> db = new BaseEntityDbContext<Abc>())
                    {
                        if (sortDic != null && sortDic.Count > 0)//根据字典内的排序执行
                        {
                            IQueryable<Abc> query = db.BaseDbSet.Where(predicate);
                            IOrderedEnumerable<Abc> queryOrder = null;
                            int num = 0;
                            foreach (KeyValuePair<Func<Abc, object>,bool> kvp in sortDic)
                            {
                                if (num == 0)//第一个
                                {
                                    if (kvp.Value)//True为降序
                                    {
                                        queryOrder = query.OrderByDescending(kvp.Key);
                                    }
                                    else
                                    {
                                        queryOrder = query.OrderBy(kvp.Key);
                                    }
                                }
                                else
                                {
                                    if (kvp.Value)//True为降序
                                    {
                                        queryOrder = queryOrder.ThenByDescending(kvp.Key);
                                    }
                                    else
                                    {
                                        queryOrder = queryOrder.ThenBy(kvp.Key);
                                    }
                                }
                                num++;
                            }
                            if (queryOrder != null)
                            {
                                if (top > 0)//top<=0则展示全部
                                {
                                    list = queryOrder.Take(top).ToList();
                                }
                                else
                                {
                                    list = queryOrder.ToList();
                                }
                            }
                            else
                            {
                                if (top > 0)//top<=0则展示全部
                                {
                                    list = query.Take(top).ToList();
                                }
                                else
                                {
                                    list = query.ToList();
                                }
                            }
                        }
                        else//默认排序 
                        {
                            if (top > 0)//top<=0则展示全部
                            {
                                list = db.BaseDbSet.Where(predicate).Take(top).ToList();
                            }
                            else
                            {
                                list = db.BaseDbSet.Where(predicate).ToList();
                            }
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
        /// <param name="specification">满足表达式规范的组合</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetQuery(ISpecification<Abc> specification)
        {
            return GetQuery(0, specification.Predicate);
        }

        /// <summary>
        /// 获取该实体的查询
        /// </summary>
        /// <param name="top">查询前几条，top小于等于0则展示全部</param>
        /// <param name="specification">满足表达式规范的组合</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetQuery(int top, ISpecification<Abc> specification)
        {
            return GetQuery(top, specification.Predicate);
        }

        /// <summary>
        /// 获取该实体的查询,单排序
        /// </summary>
        /// <param name="specification">满足表达式规范的组合</param>
        /// <param name="sortBy">排序字段</param>
        /// <param name="desc">默认True为降序,False升序</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetQuery(ISpecification<Abc> specification, Func<Abc, object> sortBy, bool desc = true)
        {
            return GetQuery(0, specification.Predicate, sortBy, desc);
        }

        /// <summary>
        /// 获取该实体的查询,单排序
        /// </summary>
        /// <param name="top">查询前几条，top小于等于0则展示全部</param>
        /// <param name="specification">满足表达式规范的组合</param>
        /// <param name="sortBy">排序字段</param>
        /// <param name="desc">默认True为降序,False升序</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetQuery(int top, ISpecification<Abc> specification, Func<Abc, object> sortBy, bool desc = true)
        {
            return GetQuery(top, specification.Predicate, sortBy, desc);
        }

        /// <summary>
        /// 获取该实体的查询,多排序
        /// </summary>
        /// <param name="specification">满足表达式规范的组合</param>
        /// <param name="sortDic">字典存放多个排序字段，Key = 默认True为降序,False升序</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetQuery(ISpecification<Abc> specification, Dictionary<Func<Abc, object>,bool> sortDic)
        {
            return GetQuery(0, specification.Predicate, sortDic);
        }

        /// <summary>
        /// 获取该实体的查询,多排序
        /// </summary>
        /// <param name="top">查询前几条，top小于等于0则展示全部</param>
        /// <param name="specification">满足表达式规范的组合</param>
        /// <param name="sortDic">字典存放多个排序字段，Key = 默认True为降序,False升序</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetQuery(int top, ISpecification<Abc> specification, Dictionary<Func<Abc, object>,bool> sortDic)
        {
            return GetQuery(top, specification.Predicate, sortDic);
        }

        #endregion

        #region ==========分页查询数据

        /// <summary>
        /// 获取分页数据,返回泛型实体List集合
        /// </summary>
        /// <param name="predicate">Linq表达式[条件查询]</param>
        /// <param name="pageIndex">当前索引的页面</param>
        /// <param name="pageSize">每一页，页面的行数.</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetPaged(Expression<Func<Abc, bool>> predicate, int pageIndex, int pageSize)
        {
            return GetPaged(predicate, pageIndex, pageSize, null);
        }

        /// <summary>
        /// 获取分页数据,返回泛型实体List集合
        /// </summary>
        /// <param name="predicate">Linq表达式[条件查询]</param>
        /// <param name="pageIndex">当前索引的页面</param>
        /// <param name="pageSize">每一页，页面的行数.</param>
        /// <param name="sortBy">Linq表达式[排序字段]</param>
        /// <param name="desc">默认True为降序,False升序</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetPaged(Expression<Func<Abc, bool>> predicate, int pageIndex, int pageSize, Func<Abc, object> sortBy, bool desc = true)
        {
            List<Abc> list = null;
            int totalCount = 0;
            if (predicate != null)
            {
                try
                {
                    using (BaseEntityDbContext<Abc> db = new BaseEntityDbContext<Abc>())
                    {
                        IQueryable<Abc> query = db.BaseDbSet.Where(predicate);//默认排序
                        if (sortBy != null)//执行排序
                        {
                            if (desc)
                            {
                                list = query.OrderByDescending(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                            }
                            else
                            {
                                list = query.OrderBy(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                            }
                        }
                        else//系统默认排序
                        {
                            list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        totalCount = query.Count();
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
        /// 获取分页数据,返回泛型实体List集合
        /// </summary>
        /// <param name="predicate">Linq表达式[条件查询]</param>
        /// <param name="pageIndex">当前索引的页面</param>
        /// <param name="pageSize">每一页，页面的行数.</param>
        /// <param name="totalCount">Out 返回数据总条数</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetPaged(Expression<Func<Abc, bool>> predicate, int pageIndex, int pageSize, out int totalCount)
        {
            return GetPaged(predicate, pageIndex, pageSize, out totalCount, null);
        }

        /// <summary>
        /// 获取分页数据,返回泛型实体List集合
        /// </summary>
        /// <param name="predicate">Linq表达式[条件查询]</param>
        /// <param name="pageIndex">当前索引的页面</param>
        /// <param name="pageSize">每一页，页面的行数.</param>
        /// <param name="totalCount">Out 返回数据总条数</param>
        /// <param name="sortBy">Linq表达式[排序字段]</param>
        /// <param name="desc">默认True为降序,False升序</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetPaged(Expression<Func<Abc, bool>> predicate, int pageIndex, int pageSize, out int totalCount, Func<Abc, object> sortBy, bool desc = true)
        {
            List<Abc> list = null;
            totalCount = 0;
            if (predicate != null)
            {
                try
                {
                    using (BaseEntityDbContext<Abc> db = new BaseEntityDbContext<Abc>())
                    {
                        IQueryable<Abc> query = db.BaseDbSet.Where(predicate);//默认排序
                        if (sortBy != null)//执行排序
                        {
                            if (desc)
                            {
                                list = query.OrderByDescending(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                            }
                            else
                            {
                                list = query.OrderBy(sortBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                            }
                        }
                        else//系统默认排序
                        {
                            list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        totalCount = query.Count();
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
        /// 获取分页数据,返回泛型实体List集合
        /// </summary>
        /// <param name="specification">满足Linq表达式规范的组合</param>
        /// <param name="pageIndex">当前索引的页面</param>
        /// <param name="pageSize">每一页，页面的行数.</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetPaged(ISpecification<Abc> specification, int pageIndex, int pageSize)
        {
            return GetPaged(specification.Predicate, pageIndex, pageSize, null);
        }
        /// <summary>
        /// 获取分页数据,返回泛型实体List集合
        /// </summary>
        /// <param name="specification">满足Linq表达式规范的组合</param>
        /// <param name="pageIndex">当前索引的页面</param>
        /// <param name="pageSize">每一页，页面的行数.</param>
        /// <param name="sortBy">Linq表达式[排序字段]</param>
        /// <param name="desc">默认True为降序,False升序</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetPaged(ISpecification<Abc> specification, int pageIndex, int pageSize, Func<Abc, object> sortBy, bool desc = true)
        {
            return GetPaged(specification.Predicate, pageIndex, pageSize, sortBy, desc);
        }

        /// <summary>
        /// 获取分页数据,返回泛型实体List集合
        /// </summary>
        /// <param name="specification">满足Linq表达式规范的组合</param>
        /// <param name="pageIndex">当前索引的页面</param>
        /// <param name="pageSize">每一页，页面的行数.</param>
        /// <param name="totalCount">Out 返回数据总条数</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetPaged(ISpecification<Abc> specification, int pageIndex, int pageSize, out int totalCount)
        {
            return GetPaged(specification.Predicate, pageIndex, pageSize, out totalCount, null);
        }

        /// <summary>
        /// 获取分页数据,返回泛型实体List集合
        /// </summary>
        /// <param name="specification">满足Linq表达式规范的组合</param>
        /// <param name="pageIndex">当前索引的页面</param>
        /// <param name="pageSize">每一页，页面的行数.</param>
        /// <param name="totalCount">Out 返回数据总条数</param>
        /// <param name="sortBy">Linq表达式[排序字段]</param>
        /// <param name="desc">默认True为降序,False升序</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetPaged(ISpecification<Abc> specification, int pageIndex, int pageSize, out int totalCount, Func<Abc, object> sortBy, bool desc = true)
        {
            return GetPaged(specification.Predicate, pageIndex, pageSize, out totalCount, sortBy, desc);
        }

        /// <summary>
        /// 获取分页数据,返回泛型实体List集合
        /// </summary>
        /// <param name="predicate">Linq表达式[条件查询]</param>
        /// <param name="pageIndex">当前索引的页面</param>
        /// <param name="pageSize">每一页，页面的行数.</param>db.BaseDbSet
        /// <param name="totalCount">Out 返回数据总条数</param>
        /// <param name="sortDic">字典存放多个排序字段，Key = 默认True为降序,False升序</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetPaged(Expression<Func<Abc, bool>> predicate, int pageIndex, int pageSize, out int totalCount, Dictionary<Func<Abc, object>,bool> sortDic)
        {
            List<Abc> list = null;
            totalCount = 0;
            if (predicate != null)
            {
                try
                {
                    using (BaseEntityDbContext<Abc> db = new BaseEntityDbContext<Abc>())
                    {
                        IQueryable<Abc> query = db.BaseDbSet.Where(predicate);
                        IOrderedEnumerable<Abc> queryOrder = null;
                        int num = 0;
                        foreach (KeyValuePair<Func<Abc, object>,bool> kvp in sortDic)
                        {
                            if (num == 0)//第一个
                            {
                                if (kvp.Value)//True为降序
                                {
                                    queryOrder = query.OrderByDescending(kvp.Key);
                                }
                                else
                                {
                                    queryOrder = query.OrderBy(kvp.Key);
                                }
                            }
                            else
                            {
                                if (kvp.Value)//True为降序
                                {
                                    queryOrder = queryOrder.ThenByDescending(kvp.Key);
                                }
                                else
                                {
                                    queryOrder = queryOrder.ThenBy(kvp.Key);
                                }
                            }
                            num++;
                        }
                        if (queryOrder != null)
                        {
                            list = queryOrder.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else//系统默认排序
                        {
                            list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        totalCount = query.Count();
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
        /// 获取分页数据,返回泛型实体List集合
        /// </summary>
        /// <param name="specification">满足Linq表达式规范的组合</param>
        /// <param name="pageIndex">当前索引的页面</param>
        /// <param name="pageSize">每一页，页面的行数.</param>
        /// <param name="totalCount">Out 返回数据总条数</param>
        /// <param name="sortDic">字典存放多个排序字段，Key = 默认True为降序,False升序</param>
        /// <returns>Abc实体对象的List集合</returns>
        public static List<Abc> GetPaged(ISpecification<Abc> specification, int pageIndex, int pageSize, out int totalCount, Dictionary<Func<Abc, object>,bool> sortDic)
        {
            return GetPaged(specification.Predicate, pageIndex, pageSize, out totalCount, sortDic);
        }

        #endregion

        #region ========Ajax Form Submit

        /// <summary>
        /// 配合前台ajax_form_submit.js文件的提交方式，封装成简易版添加，更改方法
        /// </summary>
        /// <param name="rval">表单集合字符串</param>
        /// <returns></returns>
        public static string AjaxEdit(string rval)
        {
            string result = "操作有误";
            int id = Util.StringToInt(rval.GetFromValue("id"), 0);
            try
            {
                using (BaseEntityDbContext<Abc> db = new BaseEntityDbContext<Abc>())
                {
                    Abc entity = new Abc();
                    if (id == 0)//添加操作
                    {
                        if (entity != null)
                        {
                            entity = rval.ConvertToModel(entity);//赋值
                            db.BaseDbSet.Add(entity);
                            if (db.SaveChanges() > 0)
                            {
                                result = "保存成功";
                            }
                        }
                    }
                    else if (id > 0)//更新操作
                    {
                        entity = db.BaseDbSet.FirstOrDefault(o => o.id == id);//查询
                        entity = rval.ConvertToModel(entity);//赋值
                        DbEntityEntry entry = db.Entry(entity);
                        //if (entry.State == EntityState.Detached)
                        //{
                        entry.State = EntityState.Modified;//通过更改 状态为EntityState.Modified 然后再保存 来实现更新操作
                        //}
                        result = "您没有做任何更改";
                        if (db.SaveChanges() > 0)
                        {
                            result = "保存成功";
                        }
                        else
                        {
                            result = "保存时遇到问题";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = "操作有误" + ex.Message;
                log.Error("操作有误", ex);
                return result;
            }
            return result;
        }

        /// <summary>
        /// 配合前台ajax_form_submit_json.js文件的提交方式，封装成简易版添加，更改方法
        /// </summary>
        /// <param name="rval">表单集合字符串</param>
        /// <returns></returns>
        public static string AjaxEdit(Abc entity)
        {
            string result = "操作有误";
            try
            {
                if (entity != null)
                {
                    using (BaseEntityDbContext<Abc> db = new BaseEntityDbContext<Abc>())
                    {
					                        if (entity.id== 0)//添加操作
                        {
                            db.BaseDbSet.Add(entity);
                            if (db.SaveChanges() > 0)
                            {
                                result = "保存成功";
                            }
                        }
                        else if (entity.id > 0)//更新操作
                        {
						

                            Abc edit = db.BaseDbSet.FirstOrDefault(o => o.id == entity.id);//查询
                            edit = CustomHelper.ConvertToModel(entity, edit);//赋值
                            DbEntityEntry entry = db.Entry(edit);
                            //if (entry.State == EntityState.Detached)
                            //{
                            entry.State = EntityState.Modified;//通过更改 状态为EntityState.Modified 然后再保存 来实现更新操作
                            //}
                            result = "您没有做任何更改";
                            if (db.SaveChanges() > 0)
                            {
                                result = "保存成功";
                            }
                            else
                            {
                                result = "保存时遇到问题";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = "操作有误" + ex.Message;
                log.Error("操作有误", ex);
                return result;
            }
            return result;
        }

        #endregion
    }
}

