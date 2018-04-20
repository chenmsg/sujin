using System;
using System.Linq.Expressions;

namespace ITOrm.EF.Models.Specification
{
    public sealed class DirectSpecification<TEntity> : Specification<TEntity> where TEntity : class
    {
        public DirectSpecification(Expression<Func<TEntity, bool>> expression)
        {
            _expression = expression;
        }
        Expression<Func<TEntity, bool>> _expression;
        public override Expression<Func<TEntity, bool>> Predicate
        {
            get { return _expression; }
            set { _expression = value; }
        }
    }
}
