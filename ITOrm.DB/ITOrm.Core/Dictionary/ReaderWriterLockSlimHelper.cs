using System;
using System.Threading;
using System.Collections.Generic;

namespace ITOrm.Core.Dictionary
{
    public static class ReaderWriterLockSlimHelper
    {
        /// <summary> 
        /// 为读写锁创建支持using的IDisposable帮手
        /// </summary>  
        /// <param name="instance">读写锁实例</param> 
        /// <param name="lockType">加锁类型 读/写</param> 
        /// <returns>帮手实例</returns>
        public static IDisposable CreateDisposable(this ReaderWriterLockSlim instance, LockType lockType)
        {
            var kvp = LockDisposeDic[lockType];
            return new Disposable(() => kvp.Key(instance), () => kvp.Value(instance));
        }

        /// <summary>
        /// 读写的不同操作字典
        /// </summary>
        static Dictionary<LockType, KeyValuePair<Action<ReaderWriterLockSlim>, Action<ReaderWriterLockSlim>>> LockDisposeDic = new Dictionary<LockType, KeyValuePair<Action<ReaderWriterLockSlim>, Action<ReaderWriterLockSlim>>>() { { LockType.Read, new KeyValuePair<Action<ReaderWriterLockSlim>, Action<ReaderWriterLockSlim>>(ins => ins.EnterReadLock(), ins => ins.ExitReadLock()) }, { LockType.Write, new KeyValuePair<Action<ReaderWriterLockSlim>, Action<ReaderWriterLockSlim>>(ins => ins.EnterWriteLock(), ins => ins.ExitWriteLock()) } };

    }
}