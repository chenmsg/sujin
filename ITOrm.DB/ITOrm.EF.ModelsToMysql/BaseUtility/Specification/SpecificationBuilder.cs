using System;
using System.Linq.Expressions;

namespace ITOrm.Ms.Models.Specification
{
    public sealed class SpecificationBuilder
    {
        public static ISpecification<TEntity> Create<TEntity>()where TEntity:class 
        {
            return new SpecificationBuilder<TEntity>();
        }
    }
    public class SpecificationBuilder<TEntity>:Specification<TEntity> where TEntity:class 
    {
        private Expression<Func<TEntity, bool>> _predicate;
        public SpecificationBuilder()
        {
            _predicate = a => true;
        }
        public override System.Linq.Expressions.Expression<Func<TEntity, bool>> Predicate
        {
            get
            {
                return _predicate;
            }
            set
            {
                _predicate = value;
            }
        }
    }
}
