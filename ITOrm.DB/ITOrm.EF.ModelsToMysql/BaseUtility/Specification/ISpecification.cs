using System;
using System.Linq.Expressions;

namespace ITOrm.Ms.Models.Specification
{
    /// <summary>
    /// 数据查询用了Specification模式 Specification是基于LINQ表达式的
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public  interface ISpecification<TEntity>
    {
        Expression<Func<TEntity, bool>> Predicate { get; set; }
    }
    /*      分组 AndSpecification  OrSpecification NotSpecification
            ISpecification<ArticleInfo> specLeft = SpecificationBuilder.Create<ArticleInfo>();
            ISpecification<ArticleInfo> specRight = SpecificationBuilder.Create<ArticleInfo>();
            AndSpecification<ArticleInfo> spec = new AndSpecification<ArticleInfo>(specLeft, specRight);
            .GetQuery(spec, o => o.CreateTime);
    */
    //导致调用的方法是IEnumerable<T>的Where<T>(Func<T, bool>)而不IQueryable<T>的Where<T>(Expression<Func<T, bool>>)，也就是把整个表的数据读到内存再重新筛选。
}
