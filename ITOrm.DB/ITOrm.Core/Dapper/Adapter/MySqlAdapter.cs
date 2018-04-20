using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ITOrm.Core.Dapper
{

    /// <summary>
    /// MySql实现接口ISqlAdapter,完成添加操作
    /// </summary>
    public class MySqlAdapter : ISqlAdapter
    {
        #region ISqlAdapter 成员

        public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, String tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
        {
            string cmd = String.Format("insert into {0} ({1}) values ({2})", tableName, columnList, parameterList);
            connection.Execute(cmd, entityToInsert, transaction: transaction, commandTimeout: commandTimeout);
            IEnumerable<dynamic> r = connection.Query("SELECT LAST_INSERT_ID() AS ID", transaction: transaction, commandTimeout: commandTimeout);//获取最后插入的数据ID
            int id = (int)r.First().ID;
            if (keyProperties.Any())
                keyProperties.First().SetValue(entityToInsert, id, null);
            return id;
        }

        public async Task<int> InsertAsync(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, String tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
        {
            string cmd = String.Format("insert into {0} ({1}) values ({2})", tableName, columnList, parameterList);
            await connection.ExecuteAsync(cmd, entityToInsert, transaction: transaction, commandTimeout: commandTimeout).ConfigureAwait(false);
            var r = await connection.QueryAsync<dynamic>("SELECT LAST_INSERT_ID() AS ID", transaction: transaction, commandTimeout: commandTimeout).ConfigureAwait(false);
            int id = (int)r.First().id;
            if (keyProperties.Any())
                keyProperties.First().SetValue(entityToInsert, id, null);
            return id;
        }

        #endregion
    }
}
