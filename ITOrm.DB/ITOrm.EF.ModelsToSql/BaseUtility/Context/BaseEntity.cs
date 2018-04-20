namespace ITOrm.EF.Models.Context
{
    /// <summary>
    /// 基本类型对象 抽像使用
    /// </summary>
    public abstract class BaseEntity
    {
        ///<summary>
        ///数据库名称
        ///</summary>
        public abstract string DataName { get; }

        ///<summary>
        ///表名称
        ///</summary>
        public abstract string DataTableName { get; }

        #region Hyperemia
        public virtual BaseEntity Clone()
        {
            return this as BaseEntity;
        }
        #endregion
    }
}
