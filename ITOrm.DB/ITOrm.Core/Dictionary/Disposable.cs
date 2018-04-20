using System;

namespace ITOrm.Core.Dictionary
{
    /// <summary>
    /// 销毁帮手，生成可以支持using的自定义IDisposable实例  
    /// </summary>    
    public class Disposable : IDisposable
    {
        /// <summary> 
        /// 销毁时要做的操作 
        /// </summary>
        private Action OnDispose
        {
            get;
            set;
        }

        /// <summary>
        /// 创建销毁帮手实例
        /// </summary> 
        /// <param name="onCreate">创建时要做的操作</param>
        /// <param name="onDispose">销毁是要做的操作</param> 
        public Disposable(Action onCreate, Action onDispose)
        {
            OnDispose = onDispose;
            onCreate();
        }

        ////// <summary> 
        ////// 销毁时要做的操作
        ////// </summary> 
        //////public event Action OnDispose ; 
        #region IDisposable 成员

        void IDisposable.Dispose()
        {
            OnDispose();
            OnDispose = null;
        }

        #endregion
    }
}

