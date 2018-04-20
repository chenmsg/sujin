using System;
using System.Collections.Generic;
using System.Threading;

namespace ITOrm.Core.Dictionary
{
    /// <summary>
    /// Dictionary缓存类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Cache<T> where T : class
    {
        private readonly static ReaderWriterLockSlim m_rwLock = new ReaderWriterLockSlim();
        private readonly IDictionary<string, T> _dict;

        /// <summary>
        /// 默认构造方法
        /// </summary>
        public Cache()
        {
            _dict = new Dictionary<string, T>();
        }

        /// <summary>
        /// 判断是否存在Key值的对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasKey(string key)
        {
            using (m_rwLock.CreateDisposable(LockType.Read))
            {
                return _dict.ContainsKey(key);
            }
        }

        /// <summary>
        /// 添加一个缓存
        /// </summary>
        /// <param name="key">Key值</param>
        /// <param name="obj">缓存的对象</param>
        public void Add(string key, T obj)
        {
            using (m_rwLock.CreateDisposable(LockType.Write))
            {
                _dict.Remove(key);
                _dict.Add(key, obj);
            }
        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">Key值</param>
        /// <param name="gener">委托名称</param>
        /// <returns></returns>
        public T Get(string key, Func<string, T> gener)
        {
            T value;
            using (m_rwLock.CreateDisposable(LockType.Read))
            {
                if (this._dict.TryGetValue(key, out value))
                {
                    return value;
                }
            }

            using (m_rwLock.CreateDisposable(LockType.Write))
            {
                value = gener(key);
                this._dict.Remove(key);//已经有存在的Key,则不能Add
                this._dict.Add(key, value);
                return value;
            }
        }

        /// <summary>
        /// 移除Key的缓存
        /// </summary>
        /// <param name="key">Key值</param>
        public void Remove(string key)
        {
            using (m_rwLock.CreateDisposable(LockType.Write))
            {
                _dict.Remove(key);
            }
        }
    }
}