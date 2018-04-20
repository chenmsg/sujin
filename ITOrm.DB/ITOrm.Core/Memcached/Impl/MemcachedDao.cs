using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Memcached.ClientLibrary;

namespace ITOrm.Core.Memcached.Impl
{
    public class MemcachedDao : IMemcachedDao
    {
        private readonly MemcachedClient memcachedClient;
        private static readonly MemcachedDao Dao = new MemcachedDao();

        /// <summary>
        /// 单例模式初始化MemcachedDao对象
        /// </summary>
        /// <returns></returns>
        public static MemcachedDao GetInstance()
        {
            return Dao;
        }

        /// <summary>
        /// 初始化Memcache对象
        /// </summary>
        public MemcachedDao()
            : this(new MemcachedClient())
        {
        }

        /// <summary>
        /// 用于初始化Memcache对象
        /// </summary>
        /// <param name="memcachedClient"></param>
        protected MemcachedDao(MemcachedClient memcachedClient)
        {
            this.memcachedClient = memcachedClient;
            this.memcachedClient.EnableCompression = true;
            //this.memcachedClient.CompressionThreshold = 120;//压缩阈值默认120
            this.memcachedClient.DefaultEncoding = "utf-8";
        }

        /// <summary>
        /// 通过缓存键，判断缓存是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return memcachedClient.KeyExists(key);
        }

        /// <summary>
        /// 通过缓存键数组返回与之对应的缓存集合
        /// </summary>
        /// <param name="keys">缓存键数组</param>
        /// <returns></returns>
        public object[] GetArray(string[] keys)
        {
            return memcachedClient.GetMultipleArray(keys);
        }

        /// <summary>
        /// 通过缓存键数组返回与之对应的缓存（键值对）集合
        /// </summary>
        /// <param name="keys">缓存键数组</param>
        /// <returns></returns>
        public System.Collections.Hashtable GetHashtable(string[] keys)
        {
            return memcachedClient.GetMultiple(keys);
        }

        /// <summary>
        /// 通过缓存键返回与之对应的缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public object Get(string key)
        {
            return memcachedClient.Get(key);
        }

        /// <summary>
        /// 将数据以键值对的方式，缓存到Memcached
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="obj">缓存值</param>
        /// <param name="expiry">有效期（服务器当前时间+expiry）</param>
        public void Store(string key, object obj, TimeSpan expiry)
        {
            DateTime timeout = DateTime.Now.Add(expiry);
            if (Exists(key))
            {
                if (Get(key) == null)
                {
                    memcachedClient.Set(key, obj, timeout);//缓存存在(且对象为null)，强行覆盖
                }
                else
                {
                    memcachedClient.Replace(key, obj, timeout);//缓存存在，强行覆盖
                }
            }
            else
            {
                memcachedClient.Add(key, obj, timeout);//第一次加载缓存
            }
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return memcachedClient.Delete(key);
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <returns></returns>
        public bool RemoveAll()
        {
            return memcachedClient.FlushAll();
        }

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
        public string EncodeKey(string databaseName, string tableName, string paramName, object paramValue)
        {
            var resourceName = databaseName + "." + tableName;
            var resourceParams = new Dictionary<string, string> { { paramName, paramValue.ToString() } };
            return EncodeKey(resourceName, resourceParams);
        }

        /// <summary>
        /// 编码缓存关键字。该方法只会对paramValue进行编码，编码使用一种近似于URL编码的方式，
        /// resourceName和paramName都不能含空格（包括SPACE,TAB,\r,\n），否则会抛ArgumentException。
        /// 与URL编码不同的是，它首先会对paramName进行排序，最终编码的结果与参数的顺序无关。
        /// </summary>
        /// <example>
        /// EncodeKey("Resource", new Dictionary<string, object>
        ///    {
        ///        {"name", "John"},
        ///        {"address", "Beijing"},
        ///    });
        /// ===> "Resource?address=Beijing&name=John"
        /// </example>
        /// <param name="resourceName">资源名，不能为空，不能含空格</param>
        /// <param name="resourceParams">资源参数，参数名不能为空，不能含空格</param>
        private string EncodeKey(string resourceName, IDictionary<string, string> resourceParams)
        {
            if (string.IsNullOrEmpty(resourceName))
                throw new ArgumentException("resourceName cannot be empty");

            if (DetectSpace(resourceName))
            {
                throw new ArgumentException("resourceName cannot contains space(SPACE, \t, \r, \n)");
            }
            if (resourceParams.Count == 0)
                return resourceName;

            return resourceName + "?" + EncodeParams(new SortedDictionary<string, string>(resourceParams));
        }

        // resourceParams musted be sorted by key
        private static string EncodeParams(ICollection<KeyValuePair<string, string>> resourceParams)
        {
            if (resourceParams.Count == 0)
                return string.Empty;

            var sb = new StringBuilder(256);
            foreach (var param in resourceParams)
            {
                if (string.IsNullOrEmpty(param.Key))
                {
                    throw new ArgumentException("paramName cannot be empty");
                }
                if (DetectSpace(param.Key))
                {
                    throw new ArgumentException("paramName cannot contains space(SPACE, \t, \r, \n)");
                }

                if (param.Value != null)
                {
                    sb.Append(param.Key);
                    sb.Append("=");
                    sb.Append(Encode(param.Value));
                    sb.Append("&");
                }
            }
            sb.Length--; // remove last '&' or '?'

            return sb.ToString();
        }

        /// <summary>
        /// 对字符串进行编码，采用一种类似URL的编码，如下字符需要编码：SPACE,ASCII码小于等于0x20或者大于127的字符,'/', 
        /// '+',':', '&', '?', '=', '%'，编码方式为'%'加上ASCII的两位十六进制表示，唯一的例外是SPACE编码成'+'。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string Encode(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var sb = new StringBuilder(bytes.Length * 2);
            for (var i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                switch (b)
                {
                    case ' ': // 0x20
                        sb.Append('+');
                        break;
                    case 0x00:
                    case 0x01:
                    case 0x02:
                    case 0x03:
                    case 0x04:
                    case 0x05:
                    case 0x06:
                    case 0x07:
                    case 0x08:
                    case 0x09:
                    case 0x0A:
                    case 0x0B:
                    case 0x0C:
                    case 0x0D:
                    case 0x0E:
                    case 0x0F:
                    case 0x10:
                    case 0x11:
                    case 0x12:
                    case 0x13:
                    case 0x14:
                    case 0x15:
                    case 0x16:
                    case 0x17:
                    case 0x18:
                    case 0x19:
                    case 0x1A:
                    case 0x1B:
                    case 0x1C:
                    case 0x1D:
                    case 0x1E:
                    case 0x1F:
                    case '/':
                    case '+':
                    case '&':
                    case '?':
                    case ':':
                    case '=':
                    case '%':
                        sb.Append('%').Append(ByteToHex(b));
                        break;
                    default:
                        if (b > 127)
                        {
                            sb.Append("%").Append(ByteToHex(b));
                        }
                        else
                        {
                            sb.Append((char)b);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        private static readonly char[] HexChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        private static char[] ByteToHex(int b)
        {
            var chs = new char[2];
            chs[0] = HexChars[(b >> 4) & 0x0F];
            chs[1] = HexChars[b & 0x0F];
            return chs;
        }

        private static bool DetectSpace(IEnumerable<char> str)
        {
            foreach (var ch in str)
            {
                switch (ch)
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        return true;
                }
            }
            return false;
        }
    }
}
