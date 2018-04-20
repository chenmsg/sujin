using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ITOrm.Core.Memcached
{
    /// <summary>
    /// Memcached获取，设置，删除缓存接口
    /// <remarks>
    /// 调用需要引入log4net.dll组件并在config文件中配置日志跟踪
    /// 引用ICSharpCode.SharpZipLib.dll用于缓存压缩
    /// 引入Commons.dll用于扩展
    /// </remarks>
    /// </summary>
    public interface IMemcachedDao
    {
        /// <summary>
        /// 通过缓存键数组返回与之对应的缓存集合
        /// </summary>
        /// <param name="keys">缓存键数组</param>
        /// <returns></returns>
        object[] GetArray(string[] keys);

        /// <summary>
        /// 通过缓存键数组返回与之对应的缓存（键值对）集合
        /// </summary>
        /// <param name="keys">缓存键数组</param>
        /// <returns></returns>
        Hashtable GetHashtable(string[] keys);

        /// <summary>
        /// 通过缓存键返回与之对应的缓存对象
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// 将数据以键值对的方式，缓存到Memcached
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="obj">缓存值</param>
        /// <param name="expiry">有效期（服务器当前时间+expiry）</param>
        void Store(string key, object obj, TimeSpan expiry);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Remove(string key);

        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <returns></returns>
        bool RemoveAll();

        /// <summary>
        /// 编码缓存关键字。该方法只会对paramValue进行编码，编码使用一种近似于URL编码的方式，
        /// databaseName、tableName及paramName都不能含空格（包括SPACE,TAB,\r,\n），否则会抛ArgumentException。
        /// 与URL编码不同的是，它首先会对paramName进行排序，最终编码的结果与参数的顺序无关。
        /// </summary>
        /// <param name="databaseName">库名</param>
        /// <param name="tableName">表名</param>
        /// <param name="paramName">字段（主键）</param>
        /// <param name="paramValue">字段值</param>
        /// <returns></returns>
        string EncodeKey(string databaseName, string tableName, string paramName, object paramValue);
    }
}
