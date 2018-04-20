using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ITOrm.Core.Dapper
{
    public class SQLiteAdapter : ISqlAdapter
    {
        public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, String tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
        {
            string cmd = String.Format("insert into {0} ({1}) values ({2})", tableName, columnList, parameterList);
            connection.Execute(cmd, entityToInsert, transaction: transaction, commandTimeout: commandTimeout);
            var r = connection.Query("select last_insert_rowid() id", transaction: transaction, commandTimeout: commandTimeout);
            int id = (int)r.First().id;
            if (keyProperties.Any())
                keyProperties.First().SetValue(entityToInsert, id, null);
            return id;
        }

        public async Task<int> InsertAsync(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, String tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
        {
            string cmd = String.Format("insert into {0} ({1}) values ({2})", tableName, columnList, parameterList);
            await connection.ExecuteAsync(cmd, entityToInsert, transaction: transaction, commandTimeout: commandTimeout).ConfigureAwait(false);
            var r = await connection.QueryAsync<dynamic>("select last_insert_rowid() id", transaction: transaction, commandTimeout: commandTimeout).ConfigureAwait(false);
            int id = (int)r.First().id;
            if (keyProperties.Any())
                keyProperties.First().SetValue(entityToInsert, id, null);
            return id;
        }
    }
}
