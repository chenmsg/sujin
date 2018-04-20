using System.ComponentModel.DataAnnotations;
namespace ITOrm.EF.Models.Context
{
    /// <summary>
    /// 基本类型对象 抽像使用
    /// </summary>
    public abstract class BaseView
    {

        ///<summary>
        ///数据库名称
        ///</summary>
        public abstract string DataName { get; }

        ///<summary>
        ///视图名称
        ///</summary>
        public abstract string DataTableName { get; }

        #region Hyperemia

        public virtual BaseView Clone()
        {
            return this as BaseView;
        }

        #endregion
    }
}
