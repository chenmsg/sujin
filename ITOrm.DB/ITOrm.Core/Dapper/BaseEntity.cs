
using System;
namespace ITOrm.Core.Dapper
{
    /// <summary>
    /// 基本类型对象 抽像使用
    /// </summary>
    [Serializable]
    public abstract class BaseEntity
    {
        #region Hyperemia
        public virtual BaseEntity Clone()
        {
            return this as BaseEntity;
        }
        #endregion
    }
}
