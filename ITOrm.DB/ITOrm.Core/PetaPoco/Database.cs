using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Configuration;
/**
PetaPoco是一款适用于.Net 和Mono的微小、快速、单文件的微型ORM。
PetaPoco有以下特色：
微小，没有依赖项……单个的C#文件可以方便的添加到任何项目中。
工作于严格的没有装饰的Poco类，和几乎全部加了特性的Poco类
Insert/Delete/Update/Save and IsNew 等帮助方法。
分页支持：自动得到总行数和数据
支持简单的事务
更好的支持参数替换，包括从对象属性中抓取命名的参数。
很好的性能，剔除了Linq，并通过Dynamic方法快速的为属性赋值
T4模板自动生成Poco类
查询语言是Sql……不支持别扭的fluent或Linq语法（仁者见仁，智者见智）
包含一个低耦合的Sql Builder类，让内联的Sql更容易书写
为异常信息记录、值转换器安装和数据映射提供钩子。（Hooks for logging exceptions, installing value converters and mapping columns to properties without attributes.）
兼容SQL Server, SQL Server CE, MySQL, PostgreSQL and Oracle。
可以在.NET 3.5 或Mono 2.6或更高版本上运行
在.NET 4.0 和Mono 2.8下支持dynamic
NUnit单元测试
开源（Apache License）
 */
namespace ITOrm.Core.PetaPoco
{
    /// <summary>
    /// 数据库类 Database class ... this is where most of the action happens
    /// </summary>
    public class Database : IDisposable
    {
        public Database()
        {
            _connectionString = "Server=192.168.1.113;Database=ITOrm_host;Charset=utf8;Uid=root;Pwd=123456;Convert Zero Datetime=true";
            _providerName = "MySql.Data.MySqlClient";
            CommonConstruct();
        }

        public Database(DbConnection connection)
        {
            _sharedConnection = connection;
            _connectionString = connection.ConnectionString;
            _sharedConnectionDepth = 2;//Prevent closing external connection
            CommonConstruct();
        }

        public Database(string connectionString, string providerName)
        {
            _connectionString = connectionString;
            _providerName = providerName;
            CommonConstruct();
        }

        public Database(string connectionStringName)
        {
            // Use first?
            if (connectionStringName == "")
                connectionStringName = ConfigurationManager.ConnectionStrings[0].Name;

            // Work out connection string and provider name
            var providerName = "System.Data.SqlClient";
            if (ConfigurationManager.ConnectionStrings[connectionStringName] != null)
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName))
                    providerName = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
            }
            else
            {
                throw new InvalidOperationException("Can't find a connection string with the name '" + connectionStringName + "'");
            }

            // Store factory and connection string
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _providerName = providerName;
            CommonConstruct();
        }

        /// <summary>
        /// 通用的初始化 Common initialization
        /// </summary>
        void CommonConstruct()
        {
            _transactionDepth = 0;
            EnableAutoSelect = true;
            EnableNamedParams = true;
            ForceDateTimesToUtc = true;

            if (_providerName != null)
                _factory = DbProviderFactories.GetFactory(_providerName);

            if (_connectionString != null && _connectionString.IndexOf("Allow User Variables=true") >= 0 && IsMySql())
                _paramPrefix = "?";
        }

        /// <summary>
        /// 自动关闭一个打开的共享连接 Automatically close one open shared connection
        /// </summary>
        public void Dispose()
        {
            if (_sharedConnectionDepth > 0)
                CloseSharedConnection();
        }

        /// <summary>
        /// 是否是MySql数据库 Who are we talking too?
        /// </summary>
        /// <returns></returns>
        bool IsMySql() { return string.Compare(_providerName, "MySql.Data.MySqlClient", true) == 0; }

        /// <summary>
        /// 是否是SqlServer数据库 Who are we talking too?
        /// </summary>
        /// <returns></returns>
        bool IsSqlServer() { return string.Compare(_providerName, "System.Data.SqlClient", true) == 0; }

        /// <summary>
        /// 打开一个连接（可嵌套）Open a connection (can be nested)
        /// </summary>
        public void OpenSharedConnection()
        {
            if (_sharedConnectionDepth == 0)
            {
                _sharedConnection = _factory.CreateConnection();
                _sharedConnection.ConnectionString = _connectionString;
                _sharedConnection.Open();
            }
            _sharedConnectionDepth++;
        }

        /// <summary>
        /// 关闭先前打开的连接 Close a previously opened connection
        /// </summary>
        public void CloseSharedConnection()
        {
            _sharedConnectionDepth--;
            if (_sharedConnectionDepth == 0)
            {
                _sharedConnection.Dispose();
                _sharedConnection = null;
            }
        }

        /// <summary>
        /// 帮助创造一个事务 Helper to create a transaction scope
        /// </summary>
        public Transaction Transaction
        {
            get
            {
                return new Transaction(this);
            }
        }

        /// <summary>
        /// 使用由T4模板产生的派生回购 Use by derived repo generated by T4 templates
        /// </summary>
        public virtual void OnBeginTransaction() { }

        /// <summary>
        /// 使用由T4模板产生的派生回购 Use by derived repo generated by T4 templates
        /// </summary>
        public virtual void OnEndTransaction() { }

        /// <summary>
        /// 开始一个新的事务，可以嵌套，每次调用必须通过调用AbortTransaction或CompleteTransaction匹配 Start a new transaction, can be nested, every call must be matched by a call to AbortTransaction or CompleteTransaction
        /// Use `using (var scope=db.Transaction) { scope.Complete(); }` to ensure correct semantics
        /// </summary>
        public void BeginTransaction()
        {
            _transactionDepth++;

            if (_transactionDepth == 1)
            {
                OpenSharedConnection();
                _transaction = _sharedConnection.BeginTransaction();
                _transactionCancelled = false;
                OnBeginTransaction();
            }

        }

        /// <summary>
        /// 内部帮手清理事务 Internal helper to cleanup transaction stuff
        /// </summary>
        void CleanupTransaction()
        {
            OnEndTransaction();

            if (_transactionCancelled)
                _transaction.Rollback();
            else
                _transaction.Commit();

            _transaction.Dispose();
            _transaction = null;

            CloseSharedConnection();
        }

        /// <summary>
        /// 中止整个最外层事务范围 Abort the entire outer most transaction scope
        /// </summary>
        public void AbortTransaction()
        {
            _transactionCancelled = true;
            if ((--_transactionDepth) == 0)
                CleanupTransaction();
        }

        /// <summary>
        /// 完成事务 Complete the transaction
        /// </summary>
        public void CompleteTransaction()
        {
            if ((--_transactionDepth) == 0)
                CleanupTransaction();
        }

        /// <summary>
        /// 助手来处理来自对象属性命名参数 Helper to handle named parameters from object properties
        /// </summary>
        static Regex rxParams = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);

        /// <summary>
        /// 助手来处理来自对象属性命名参数 Helper to handle named parameters from object properties
        /// </summary>
        public static string ProcessParams(string _sql, object[] args_src, List<object> args_dest)
        {
            return rxParams.Replace(_sql, m =>
            {
                string param = m.Value.Substring(1);

                int paramIndex;
                if (int.TryParse(param, out paramIndex))
                {
                    // Numbered parameter
                    if (paramIndex < 0 || paramIndex >= args_src.Length)
                        throw new ArgumentOutOfRangeException(string.Format("Parameter '@{0}' specified but only {1} parameters supplied (in `{2}`)", paramIndex, args_src.Length, _sql));
                    args_dest.Add(args_src[paramIndex]);
                }
                else
                {
                    // Look for a property on one of the arguments with this name
                    bool found = false;
                    foreach (var o in args_src)
                    {
                        var pi = o.GetType().GetProperty(param);
                        if (pi != null)
                        {
                            args_dest.Add(pi.GetValue(o, null));
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        throw new ArgumentException(string.Format("Parameter '@{0}' specified but none of the passed arguments have a property with this name (in '{1}')", param, _sql));
                }

                return "@" + (args_dest.Count - 1).ToString();
            }
            );
        }

        /// <summary>
        /// 参数添加到一个数据库命令 Add a parameter to a DB command
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="item"></param>
        /// <param name="ParameterPrefix"></param>
        static void AddParam(DbCommand cmd, object item, string ParameterPrefix)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = string.Format("{0}{1}", ParameterPrefix, cmd.Parameters.Count);
            if (item == null)
            {
                p.Value = DBNull.Value;
            }
            else
            {
                if (item.GetType() == typeof(Guid))
                {
                    p.Value = item.ToString();
                    p.DbType = DbType.String;
                    p.Size = 4000;
                }
                else if (item.GetType() == typeof(string))
                {
                    p.Size = (item as string).Length + 1;
                    if (p.Size < 4000)
                        p.Size = 4000;
                    p.Value = item;
                }
                else
                {
                    p.Value = item;
                }
            }

            cmd.Parameters.Add(p);
        }

        /// <summary>
        /// 创建命令 Create a command
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DbCommand CreateCommand(DbConnection connection, string sql, params object[] args)
        {
            if (EnableNamedParams)
            {
                // Perform named argument replacements
                var new_args = new List<object>();
                sql = ProcessParams(sql, args, new_args);
                args = new_args.ToArray();
            }

            // If we're in MySQL "Allow User Variables", we need to fix up parameter prefixes
            if (_paramPrefix == "?")
            {
                // Convert "@parameter" -> "?parameter"
                Regex paramReg = new Regex(@"(?<!@)@\w+");
                sql = paramReg.Replace(sql, m => "?" + m.Value.Substring(1));

                // Convert @@uservar -> @uservar and @@@systemvar -> @@systemvar
                sql = sql.Replace("@@", "@");
            }

            // 保存最后一个sql和参数 Save the last sql and args
            _lastSql = sql;
            _lastArgs = args;

            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Transaction = _transaction;
            foreach (var item in args)
            {
                var p = cmd.CreateParameter();
                p.ParameterName = string.Format("{0}{1}", _paramPrefix, cmd.Parameters.Count);
                if (item == null)
                {
                    p.Value = DBNull.Value;
                }
                else
                {
                    if (item.GetType() == typeof(Guid))
                    {
                        p.Value = item.ToString();
                        p.DbType = DbType.String;
                        p.Size = 4000;
                    }
                    else if (item.GetType() == typeof(string))
                    {
                        p.Size = (item as string).Length + 1;
                        if (p.Size < 4000)
                            p.Size = 4000;
                        p.Value = item;
                    }
                    else
                    {
                        p.Value = item;
                    }
                }

                cmd.Parameters.Add(p);
            }
            return cmd;
        }

        /// <summary>
        /// 覆盖此记录/捕获异常 Override this to log/capture exceptions
        /// </summary>
        /// <param name="x"></param>
        public virtual void OnException(Exception x)
        {
            System.Diagnostics.Debug.WriteLine(x.ToString());
            System.Diagnostics.Debug.WriteLine(LastCommand);
        }

        /// <summary>
        /// 执行非查询命令 Execute a non-query command
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int Execute(string sql, params object[] args)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args))
                    {
                        return cmd.ExecuteNonQuery();
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
        }

        /// <summary>
        /// 执行非查询命令
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Execute(Sql sql)
        {
            return Execute(sql.SQL, sql.Arguments);
        }

        /// <summary>
        /// 执行蒙上了标量属性 Execute and cast a scalar property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, params object[] args)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args))
                    {
                        object val = cmd.ExecuteScalar();
                        return (T)Convert.ChangeType(val, typeof(T));
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
        }

        /// <summary>
        /// 执行蒙上了标量属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(Sql sql)
        {
            return ExecuteScalar<T>(sql.SQL, sql.Arguments);
        }

        Regex rxSelect = new Regex(@"^\s*SELECT\s", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        string AddSelectClause<T>(string sql)
        {
            // 已经存在? Already present?
            if (rxSelect.IsMatch(sql))
                return sql;

            // 获取对象T的类型 Get the poco data for this type
            var pd = PocoData.ForType(typeof(T));
            return string.Format("SELECT {0} FROM {1} {2}", pd.QueryColumns, pd.TableName, sql);
        }

        public bool EnableAutoSelect { get; set; }
        public bool EnableNamedParams { get; set; }
        public bool ForceDateTimesToUtc { get; set; }

        /// <summary>
        /// 返回List<T>的类型列表 Return a typed list of pocos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<T> Fetch<T>(string sql, params object[] args) where T : new()
        {
            // 默认选择条款? Auto select clause?
            if (EnableAutoSelect)
                sql = AddSelectClause<T>(sql);

            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args))
                    {
                        using (var r = cmd.ExecuteReader())
                        {
                            var l = new List<T>();
                            var pd = PocoData.ForType(typeof(T));
                            var factory = pd.GetFactory<T>(sql + "-" + _sharedConnection.ConnectionString + ForceDateTimesToUtc.ToString(), ForceDateTimesToUtc, r);
                            while (r.Read())
                            {
                                l.Add(factory(r));
                            }
                            return l;
                        }
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
        }

        /// <summary>
        /// 查询单条 Optimized version when only needing a single record
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T FirstOrDefault<T>(string sql, params object[] args) where T : new()
        {
            // 默认选择条款? Auto select clause?
            if (EnableAutoSelect)
                sql = AddSelectClause<T>(sql);

            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args))
                    {
                        using (var r = cmd.ExecuteReader())
                        {
                            if (!r.Read())
                                return default(T);

                            var pd = PocoData.ForType(typeof(T));
                            var factory = pd.GetFactory<T>(sql + "-" + _sharedConnection.ConnectionString + ForceDateTimesToUtc.ToString(), ForceDateTimesToUtc, r);
                            return factory(r);
                        }
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
        }

        /// <summary>
        /// 查询单条 Optimized version when only wanting a single record
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T SingleOrDefault<T>(string sql, params object[] args) where T : new()
        {
            // 默认选择条款? Auto select clause?
            if (EnableAutoSelect)
                sql = AddSelectClause<T>(sql);

            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args))
                    {
                        using (var r = cmd.ExecuteReader())
                        {
                            if (!r.Read())
                                return default(T);

                            var pd = PocoData.ForType(typeof(T));
                            var factory = pd.GetFactory<T>(sql + "-" + _sharedConnection.ConnectionString + ForceDateTimesToUtc.ToString(), ForceDateTimesToUtc, r);
                            T ret = factory(r);

                            if (r.Read())
                                throw new InvalidOperationException("Sequence contains more than one element");

                            return ret;
                        }
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
        }

        /// <summary>
        /// 警告：可怕的正则表达式如下 Warning: scary regex follows
        /// </summary>
        static Regex rxColumns = new Regex(@"^\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b",
                            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
        static Regex rxOrderBy = new Regex(@"\bORDER\s+BY\s+(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*",
                            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
        public static bool SplitSqlForPaging(string sql, out string sqlCount, out string sqlSelectRemoved, out string sqlOrderBy)
        {
            sqlSelectRemoved = null;
            sqlCount = null;
            sqlOrderBy = null;

            // Extract the columns from "SELECT <whatever> FROM"
            var m = rxColumns.Match(sql);
            if (!m.Success)
                return false;

            // Save column list and replace with COUNT(*)
            Group g = m.Groups[1];
            sqlCount = sql.Substring(0, g.Index) + "COUNT(*) " + sql.Substring(g.Index + g.Length);
            sqlSelectRemoved = sql.Substring(g.Index);

            // Look for an "ORDER BY <whatever>" clause
            m = rxOrderBy.Match(sqlCount);
            if (!m.Success)
                return false;

            g = m.Groups[0];
            sqlOrderBy = g.ToString();
            sqlCount = sqlCount.Substring(0, g.Index) + sqlCount.Substring(g.Index + g.Length);

            return true;
        }

        /// <summary>
        /// 分页 Fetch a page	
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Page<T> Page<T>(long page, long itemsPerPage, string sql, params object[] args) where T : new()
        {
            // Add auto select clause
            if (EnableAutoSelect)
                sql = AddSelectClause<T>(sql);

            // Split the SQL into the bits we need
            string sqlCount, sqlSelectRemoved, sqlOrderBy;
            if (!SplitSqlForPaging(sql, out sqlCount, out sqlSelectRemoved, out sqlOrderBy))
                throw new Exception("Unable to parse SQL statement for paged query");

            // Setup the paged result
            var result = new Page<T>();
            result.CurrentPage = page;
            result.ItemsPerPage = itemsPerPage;
            result.TotalItems = ExecuteScalar<long>(sqlCount, args);
            result.TotalPages = result.TotalItems / itemsPerPage;
            if ((result.TotalItems % itemsPerPage) != 0)
                result.TotalPages++;


            // Build the SQL for the actual final result
            string sqlPage;
            if (IsSqlServer())
            {
                // Ugh really?
                sqlSelectRemoved = rxOrderBy.Replace(sqlSelectRemoved, "");
                sqlPage = string.Format("SELECT * FROM (SELECT ROW_NUMBER() OVER ({0}) AS __rn, {1}) as __paged WHERE __rn>{2} AND __rn<={3}",
                                        sqlOrderBy, sqlSelectRemoved, (page - 1) * itemsPerPage, page * itemsPerPage);
            }
            else
            {
                // Nice
                sqlPage = string.Format("{0}\nLIMIT {1} OFFSET {2}", sql, itemsPerPage, (page - 1) * itemsPerPage);
            }

            // Get the records
            result.Items = Fetch<T>(sqlPage, args);

            // Done
            return result;
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Page<T> Page<T>(long page, long itemsPerPage, Sql sql) where T : new()
        {
            return Page<T>(page, itemsPerPage, sql.SQL, sql.Arguments);
        }

        /// <summary>
        /// 返回IEnumerable<T>的枚举集合 Return an enumerable collection of pocos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql, params object[] args) where T : new()
        {
            if (EnableAutoSelect)
                sql = AddSelectClause<T>(sql);

            using (var conn = new ShareableConnection(this))
            {
                using (var cmd = CreateCommand(conn.Connection, sql, args))
                {
                    IDataReader r;
                    var pd = PocoData.ForType(typeof(T));
                    try
                    {
                        r = cmd.ExecuteReader();
                    }
                    catch (Exception x)
                    {
                        OnException(x);
                        throw;
                    }
                    var factory = pd.GetFactory<T>(sql + "-" + conn.Connection.ConnectionString + ForceDateTimesToUtc.ToString(), ForceDateTimesToUtc, r);
                    using (r)
                    {
                        while (true)
                        {
                            T poco;
                            try
                            {
                                if (!r.Read())
                                    yield break;
                                poco = factory(r);
                            }
                            catch (Exception x)
                            {
                                OnException(x);
                                throw;
                            }

                            yield return poco;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 返回List<T>的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<T> Fetch<T>(Sql sql) where T : new()
        {
            return Fetch<T>(sql.SQL, sql.Arguments);
        }

        /// <summary>
        /// 返回IEnumerable<T>的枚举集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(Sql sql) where T : new()
        {
            return Query<T>(sql.SQL, sql.Arguments);
        }

        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T Single<T>(string sql, params object[] args) where T : new()
        {
            T val = SingleOrDefault<T>(sql, args);
            if (val != null)
                return val;
            else
                throw new InvalidOperationException("The sequence contains no elements");
        }

        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T First<T>(string sql, params object[] args) where T : new()
        {
            T val = FirstOrDefault<T>(sql, args);
            if (val != null)
                return val;
            else
                throw new InvalidOperationException("The sequence contains no elements");
        }

        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T Single<T>(Sql sql) where T : new()
        {
            return Single<T>(sql.SQL, sql.Arguments);
        }

        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T SingleOrDefault<T>(Sql sql) where T : new()
        {
            return SingleOrDefault<T>(sql.SQL, sql.Arguments);
        }

        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T FirstOrDefault<T>(Sql sql) where T : new()
        {
            return FirstOrDefault<T>(sql.SQL, sql.Arguments);
        }

        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T First<T>(Sql sql) where T : new()
        {
            return First<T>(sql.SQL, sql.Arguments);
        }

        /// <summary>
        /// 添加 插入POCO到表。如果POCO与作为主键的新记录的ID分配给它的名称相同的属性。无论哪种方式，返回新的id。Insert a poco into a table.  If the poco has a property with the same name as the primary key the id of the new record is assigned to it.  Either way,the new id is returned.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKeyName"></param>
        /// <param name="poco"></param>
        /// <returns></returns>
        public object Insert(string tableName, string primaryKeyName, object poco)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, ""))
                    {
                        var pd = PocoData.ForType(poco.GetType());
                        var names = new List<string>();
                        var values = new List<string>();
                        var index = 0;
                        foreach (var i in pd.Columns)
                        {
                            // Don't insert the primary key or result only columns
                            if ((primaryKeyName != null && i.Key == primaryKeyName) || i.Value.ResultColumn)
                                continue;

                            names.Add(i.Key);
                            values.Add(string.Format("{0}{1}", _paramPrefix, index++));
                            AddParam(cmd, i.Value.PropertyInfo.GetValue(poco, null), _paramPrefix);
                        }

                        cmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2}); SELECT @@IDENTITY AS NewID;",
                                tableName,
                                string.Join(",", names.ToArray()),
                                string.Join(",", values.ToArray())
                                );

                        _lastSql = cmd.CommandText;
                        _lastArgs = values.ToArray();

                        // Insert the record, should get back it's ID
                        var id = cmd.ExecuteScalar();

                        // Assign the ID back to the primary key property
                        if (primaryKeyName != null)
                        {
                            PocoColumn pc;
                            if (pd.Columns.TryGetValue(primaryKeyName, out pc))
                            {
                                pc.PropertyInfo.SetValue(poco, Convert.ChangeType(id, pc.PropertyInfo.PropertyType), null);
                            }
                        }

                        return id;
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
        }

        /// <summary>
        /// 添加 Insert an annotated poco object
        /// </summary>
        /// <param name="poco"></param>
        /// <returns></returns>
        public object Insert(object poco)
        {
            var pd = PocoData.ForType(poco.GetType());
            //return Insert(pd.TableName, pd.PrimaryKey, poco); cITOrm edit 2015-01-28
            string tableName = (poco.GetType().GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic).Name;
            return Insert(tableName, pd.PrimaryKey, poco);
        }

        /// <summary>
        /// 更改 Update a record with values from a poco.  primary key value can be either supplied or read from the poco
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKeyName"></param>
        /// <param name="poco"></param>
        /// <param name="primaryKeyValue"></param>
        /// <returns></returns>
        public int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, ""))
                    {
                        var sb = new StringBuilder();
                        var index = 0;
                        var pd = PocoData.ForType(poco.GetType());
                        foreach (var i in pd.Columns)
                        {
                            // Don't update the primary key, but grab the value if we don't have it
                            if (i.Key == primaryKeyName)
                            {
                                if (primaryKeyValue == null)
                                    primaryKeyValue = i.Value.PropertyInfo.GetValue(poco, null);
                                continue;
                            }

                            // Dont update result only columns
                            if (i.Value.ResultColumn)
                                continue;

                            // Build the sql
                            if (index > 0)
                                sb.Append(", ");
                            sb.AppendFormat("{0} = {1}{2}", i.Key, _paramPrefix, index++);

                            // Store the parameter in the command
                            AddParam(cmd, i.Value.PropertyInfo.GetValue(poco, null), _paramPrefix);
                        }

                        cmd.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2} = {3}{4}",
                                tableName,
                                sb.ToString(),
                                primaryKeyName,
                                _paramPrefix,
                                index++
                                );
                        AddParam(cmd, primaryKeyValue, _paramPrefix);

                        _lastSql = cmd.CommandText;
                        _lastArgs = new object[] { primaryKeyValue };

                        // Do it
                        return cmd.ExecuteNonQuery();
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
        }

        /// <summary>
        /// 更改
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKeyName"></param>
        /// <param name="poco"></param>
        /// <returns></returns>
        public int Update(string tableName, string primaryKeyName, object poco)
        {
            return Update(tableName, primaryKeyName, poco, null);
        }

        /// <summary>
        /// 更改
        /// </summary>
        /// <param name="poco"></param>
        /// <returns></returns>
        public int Update(object poco)
        {
            return Update(poco, null);
        }

        /// <summary>
        /// 更改
        /// </summary>
        /// <param name="poco"></param>
        /// <param name="primaryKeyValue"></param>
        /// <returns></returns>
        public int Update(object poco, object primaryKeyValue)
        {
            var pd = PocoData.ForType(poco.GetType());
            string tableName = (poco.GetType().GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic).Name;
            return Update(tableName, pd.PrimaryKey, poco, primaryKeyValue);//cITOrm edit 2015-01-28
        }

        /// <summary>
        /// 更改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int Update<T>(string sql, params object[] args)
        {
            var pd = PocoData.ForType(typeof(T));
            return Execute(string.Format("UPDATE {0} {1}", pd.TableName, sql), args);
        }

        /// <summary>
        /// 更改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Update<T>(Sql sql)
        {
            string tableName = (typeof(T).GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic).Name;
            return Execute(new Sql(string.Format("UPDATE {0}", tableName)).Append(sql));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKeyName"></param>
        /// <param name="poco"></param>
        /// <returns></returns>
        public int Delete(string tableName, string primaryKeyName, object poco)
        {
            return Delete(tableName, primaryKeyName, poco, null);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKeyName"></param>
        /// <param name="poco"></param>
        /// <param name="primaryKeyValue"></param>
        /// <returns></returns>
        public int Delete(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            // If primary key value not specified, pick it up from the object
            if (primaryKeyValue == null)
            {
                var pd = PocoData.ForType(poco.GetType());
                PocoColumn pc;
                if (pd.Columns.TryGetValue(primaryKeyName, out pc))
                {
                    primaryKeyValue = pc.PropertyInfo.GetValue(poco, null);
                }
            }

            // Do it
            var sql = string.Format("DELETE FROM {0} WHERE {1}=@0", tableName, primaryKeyName);
            return Execute(sql, primaryKeyValue);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="poco"></param>
        /// <returns></returns>
        public int Delete(object poco)
        {
            var pd = PocoData.ForType(poco.GetType());
            string tableName = (poco.GetType().GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic).Name;
            return Delete(tableName, pd.PrimaryKey, poco);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int Delete<T>(string sql, params object[] args)
        {
            string tableName = (typeof(T).GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic).Name;
            return Execute(string.Format("DELETE FROM {0} {1}", tableName, sql), args);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Delete<T>(Sql sql)
        {
            string tableName = (typeof(T).GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic).Name;
            return Execute(new Sql(string.Format("DELETE FROM {0}", tableName)).Append(sql));
        }

        /// <summary>
        /// 检查是否有poco对象的新纪录 Check if a poco represents a new record
        /// </summary>
        /// <param name="primaryKeyName"></param>
        /// <param name="poco"></param>
        /// <returns></returns>
        public bool IsNew(string primaryKeyName, object poco)
        {
            // If primary key value not specified, pick it up from the object
            var pd = PocoData.ForType(poco.GetType());
            PropertyInfo pi;
            PocoColumn pc;
            if (pd.Columns.TryGetValue(primaryKeyName, out pc))
            {
                pi = pc.PropertyInfo;
            }
            else
            {
                pi = poco.GetType().GetProperty(primaryKeyName);
                if (pi == null)
                    throw new ArgumentException("The object doesn't have a property matching the primary key column name '{0}'", primaryKeyName);
            }

            // Get it's value
            var pk = pi.GetValue(poco, null);
            if (pk == null)
                return true;

            var type = pk.GetType();

            if (type.IsValueType)
            {
                // Common primary key types
                if (type == typeof(long))
                    return (long)pk == 0;
                else if (type == typeof(ulong))
                    return (ulong)pk == 0;
                else if (type == typeof(int))
                    return (int)pk == 0;
                else if (type == typeof(uint))
                    return (int)pk == 0;

                // Create a default instance and compare
                return pk == Activator.CreateInstance(pk.GetType());
            }
            else
            {
                return pk == null;
            }
        }

        public bool IsNew(object poco)
        {
            var pd = PocoData.ForType(poco.GetType());
            return IsNew(pd.PrimaryKey, poco);
        }

        /// <summary>
        /// 插入新记录或更新现有记录 Insert new record or Update existing record
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKeyName"></param>
        /// <param name="poco"></param>
        public void Save(string tableName, string primaryKeyName, object poco)
        {
            if (IsNew(primaryKeyName, poco))
            {
                Insert(tableName, primaryKeyName, poco);
            }
            else
            {
                Update(tableName, primaryKeyName, poco);
            }
        }

        public void Save(object poco)
        {
            var pd = PocoData.ForType(poco.GetType());
            string tableName = (poco.GetType().GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic).Name;
            Save(tableName, pd.PrimaryKey, poco);
        }

        public string LastSQL { get { return _lastSql; } }

        public object[] LastArgs { get { return _lastArgs; } }

        public string LastCommand
        {
            get
            {
                var sb = new StringBuilder();
                if (_lastSql == null)
                    return "";
                sb.Append(_lastSql);
                if (_lastArgs != null)
                {
                    sb.Append("\r\n\r\n");
                    for (int i = 0; i < _lastArgs.Length; i++)
                    {
                        sb.AppendFormat("{0} - {1}\r\n", i, _lastArgs[i].ToString());
                    }
                }
                return sb.ToString();
            }
        }

        public static IMapper Mapper
        {
            get;
            set;
        }

        internal class PocoColumn
        {
            public string ColumnName;
            public PropertyInfo PropertyInfo;
            public bool ResultColumn;
        }

        internal class PocoData
        {
            public static PocoData ForType(Type t)
            {
                lock (m_PocoData)
                {
                    PocoData pd;
                    if (!m_PocoData.TryGetValue(t, out pd))
                    {
                        pd = new PocoData(t);
                        m_PocoData.Add(t, pd);
                    }
                    return pd;
                }
            }

            public PocoData(Type t)
            {
                // Get the table name
                var a = t.GetCustomAttributes(typeof(TableName), true);
                var tempTableName = a.Length == 0 ? t.Name : (a[0] as TableName).Value;

                // Get the primary key
                a = t.GetCustomAttributes(typeof(PrimaryKey), true);
                var tempPrimaryKey = a.Length == 0 ? "ID" : (a[0] as PrimaryKey).Value;

                // Call column mapper
                if (Database.Mapper != null)
                    Database.Mapper.GetTableInfo(t, ref tempTableName, ref tempPrimaryKey);
                TableName = tempTableName;
                PrimaryKey = tempPrimaryKey;

                // Work out bound properties
                bool ExplicitColumns = t.GetCustomAttributes(typeof(ExplicitColumns), true).Length > 0;
                Columns = new Dictionary<string, PocoColumn>(StringComparer.OrdinalIgnoreCase);
                foreach (var pi in t.GetProperties())
                {
                    // Work out if properties is to be included
                    var ColAttrs = pi.GetCustomAttributes(typeof(Column), true);
                    if (ExplicitColumns)
                    {
                        if (ColAttrs.Length == 0)
                            continue;
                    }
                    else
                    {
                        if (pi.GetCustomAttributes(typeof(Ignore), true).Length != 0)
                            continue;
                    }

                    var pc = new PocoColumn();
                    pc.PropertyInfo = pi;

                    // Work out the DB column name
                    if (ColAttrs.Length > 0)
                    {
                        var colattr = (Column)ColAttrs[0];
                        pc.ColumnName = colattr.Name;
                        if ((colattr as ResultColumn) != null)
                            pc.ResultColumn = true;
                    }
                    if (pc.ColumnName == null)
                    {
                        pc.ColumnName = pi.Name;
                        if (Database.Mapper != null && !Database.Mapper.MapPropertyToColumn(pi, ref pc.ColumnName, ref pc.ResultColumn))
                            continue;
                    }

                    // Store it
                    Columns.Add(pc.ColumnName, pc);
                }

                // Build column list for automatic select
                QueryColumns = string.Join(", ", (from c in Columns where !c.Value.ResultColumn select c.Key).ToArray());
            }

            // Create factory function that can convert a IDataReader record into a POCO
            public Func<IDataReader, T> GetFactory<T>(string key, bool ForceDateTimesToUtc, IDataReader r)
            {
                lock (PocoFactories)
                {
                    // Have we already created it?
                    object factory;
                    if (PocoFactories.TryGetValue(key, out factory))
                        return factory as Func<IDataReader, T>;

                    lock (m_Converters)
                    {
                        // Create the method
                        var m = new DynamicMethod("petapoco_factory_" + PocoFactories.Count.ToString(), typeof(T), new Type[] { typeof(IDataReader) }, true);
                        var il = m.GetILGenerator();

                        // Running under mono?
                        int p = (int)Environment.OSVersion.Platform;
                        bool Mono = (p == 4) || (p == 6) || (p == 128);

                        // var poco=new T()
                        il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));

                        // Enumerate all fields generating a set assignment for the column
                        for (int i = 0; i < r.FieldCount; i++)
                        {
                            // Get the PocoColumn for this db column, ignore if not known
                            PocoColumn pc;
                            if (!Columns.TryGetValue(r.GetName(i), out pc))
                                continue;

                            // Get the source type for this column
                            var srcType = r.GetFieldType(i);
                            var dstType = pc.PropertyInfo.PropertyType;

                            // "if (!rdr.IsDBNull(i))"
                            il.Emit(OpCodes.Ldarg_0);										// poco,rdr
                            il.Emit(OpCodes.Ldc_I4, i);										// poco,rdr,i
                            il.Emit(OpCodes.Callvirt, fnIsDBNull);							// poco,bool
                            var lblNext = il.DefineLabel();
                            il.Emit(OpCodes.Brtrue_S, lblNext);								// poco

                            il.Emit(OpCodes.Dup);											// poco,poco

                            // Do we need to install a converter?
                            Func<object, object> converter = null;

                            // Get converter from the mapper
                            if (Database.Mapper != null)
                            {
                                converter = Database.Mapper.GetValueConverter(pc.PropertyInfo, srcType);
                            }

                            // Standard DateTime->Utc mapper
                            if (ForceDateTimesToUtc && converter == null && srcType == typeof(DateTime) && (dstType == typeof(DateTime) || dstType == typeof(DateTime?)))
                            {
                                converter = delegate(object src) { return new DateTime(((DateTime)src).Ticks, DateTimeKind.Utc); };
                            }

                            // Forced type conversion
                            if (converter == null && !dstType.IsAssignableFrom(srcType))
                            {
                                converter = delegate(object src) { return Convert.ChangeType(src, dstType, null); };
                            }

                            // Fast
                            bool Handled = false;
                            if (converter == null)
                            {
                                var valuegetter = typeof(IDataRecord).GetMethod("Get" + srcType.Name, new Type[] { typeof(int) });
                                if (valuegetter != null
                                        && valuegetter.ReturnType == srcType
                                        && (valuegetter.ReturnType == dstType || valuegetter.ReturnType == Nullable.GetUnderlyingType(dstType)))
                                {
                                    il.Emit(OpCodes.Ldarg_0);										// *,rdr
                                    il.Emit(OpCodes.Ldc_I4, i);										// *,rdr,i
                                    il.Emit(OpCodes.Callvirt, valuegetter);							// *,value

                                    // Mono give IL error if we don't explicitly create Nullable instance for the assignment
                                    if (Mono && Nullable.GetUnderlyingType(dstType) != null)
                                    {
                                        il.Emit(OpCodes.Newobj, dstType.GetConstructor(new Type[] { Nullable.GetUnderlyingType(dstType) }));
                                    }

                                    il.Emit(OpCodes.Callvirt, pc.PropertyInfo.GetSetMethod());		// poco
                                    Handled = true;
                                }
                            }

                            // Not so fast
                            if (!Handled)
                            {
                                // Setup stack for call to converter
                                int converterIndex = -1;
                                if (converter != null)
                                {
                                    // Add the converter
                                    converterIndex = m_Converters.Count;
                                    m_Converters.Add(converter);

                                    // Generate IL to push the converter onto the stack
                                    il.Emit(OpCodes.Ldsfld, fldConverters);
                                    il.Emit(OpCodes.Ldc_I4, converterIndex);
                                    il.Emit(OpCodes.Callvirt, fnListGetItem);					// Converter
                                }

                                // "value = rdr.GetValue(i)"
                                il.Emit(OpCodes.Ldarg_0);										// *,rdr
                                il.Emit(OpCodes.Ldc_I4, i);										// *,rdr,i
                                il.Emit(OpCodes.Callvirt, fnGetValue);							// *,value

                                // Call the converter
                                if (converter != null)
                                    il.Emit(OpCodes.Callvirt, fnInvoke);

                                // Assign it
                                il.Emit(OpCodes.Unbox_Any, pc.PropertyInfo.PropertyType);		// poco,poco,value
                                il.Emit(OpCodes.Callvirt, pc.PropertyInfo.GetSetMethod());		// poco
                            }

                            il.MarkLabel(lblNext);
                        }

                        il.Emit(OpCodes.Ret);

                        // Cache it, return it
                        var del = (Func<IDataReader, T>)m.CreateDelegate(typeof(Func<IDataReader, T>));
                        PocoFactories.Add(key, del);
                        return del;
                    }
                }
            }


            static Dictionary<Type, PocoData> m_PocoData = new Dictionary<Type, PocoData>();
            static List<Func<object, object>> m_Converters = new List<Func<object, object>>();

            static MethodInfo fnGetValue = typeof(IDataRecord).GetMethod("GetValue", new Type[] { typeof(int) });
            static MethodInfo fnIsDBNull = typeof(IDataRecord).GetMethod("IsDBNull");
            static FieldInfo fldConverters = typeof(PocoData).GetField("m_Converters", BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic);
            static MethodInfo fnListGetItem = typeof(List<Func<object, object>>).GetProperty("Item").GetGetMethod();
            static MethodInfo fnInvoke = typeof(Func<object, object>).GetMethod("Invoke");

            public string TableName { get; private set; }
            public string PrimaryKey { get; private set; }
            public string QueryColumns { get; private set; }
            public Dictionary<string, PocoColumn> Columns { get; private set; }
            Dictionary<string, object> PocoFactories = new Dictionary<string, object>();
        }

        /// <summary>
        /// ShareableConnection represents either a shared connection used by a transaction,or a one-off connection if not in a transaction.Non-shared connections are disposed 
        /// </summary>
        class ShareableConnection : IDisposable
        {
            public ShareableConnection(Database db)
            {
                _db = db;
                _db.OpenSharedConnection();
            }

            public DbConnection Connection
            {
                get
                {
                    return _db._sharedConnection;
                }
            }

            Database _db;

            public void Dispose()
            {
                _db.CloseSharedConnection();
            }
        }

        // Member variables
        string _connectionString;
        string _providerName;
        DbProviderFactory _factory;
        DbConnection _sharedConnection;
        DbTransaction _transaction;
        int _sharedConnectionDepth;
        int _transactionDepth;
        bool _transactionCancelled;
        string _lastSql;
        object[] _lastArgs;
        string _paramPrefix = "@";
    }
}
