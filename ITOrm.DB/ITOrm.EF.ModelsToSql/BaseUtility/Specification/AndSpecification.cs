using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ITOrm.EF.Models.Specification
{
    public class AndSpecification<TEntity>:CompositeSpecification<TEntity>where TEntity:class
    {
        #region 变量定义

        private Expression<Func<TEntity, bool>> _predicate;
        private ISpecification<TEntity> _rightSideSpecification = null;
        private ISpecification<TEntity> _leftSideSpecification = null;
        #endregion

        #region
        /// <summary>
        /// Default constructor for AndSpecification
        /// </summary>
        /// <param name="leftSide">Left side specification</param>
        /// <param name="rightSide">Right side specification</param>
        public AndSpecification(ISpecification<TEntity> leftSide, ISpecification<TEntity> rightSide)
        {
            if (leftSide == (ISpecification<TEntity>)null)
                throw new ArgumentNullException("leftSide");

            if (rightSide == (ISpecification<TEntity>)null)
                throw new ArgumentNullException("rightSide");

            this._leftSideSpecification = leftSide;
            this._rightSideSpecification = rightSide;
        } 
        #endregion
        public override ISpecification<TEntity> LeftSideSpecification
        {
            get { return _leftSideSpecification; }
        }

        public override ISpecification<TEntity> RightSideSpecification
        {
            get { return _rightSideSpecification; }
        }
        public override Expression<Func<TEntity, bool>> Predicate
        {
            get
            {
                Expression<Func<TEntity, bool>> leftSite = _leftSideSpecification.Predicate;
                Expression<Func<TEntity, bool>> rightSite = _rightSideSpecification.Predicate;
                _predicate= leftSite.And(rightSite);
                return _predicate;
            }
            set { _predicate = value; }
        }
    }
}
