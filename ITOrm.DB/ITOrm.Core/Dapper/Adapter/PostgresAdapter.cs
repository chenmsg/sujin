﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Core.Dapper
{
    public class PostgresAdapter : ISqlAdapter
    {
        public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, String tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("insert into {0} ({1}) values ({2})", tableName, columnList, parameterList);

            // If no primary key then safe to assume a join table with not too much data to return
            if (!keyProperties.Any())
                sb.Append(" RETURNING *");
            else
            {
                sb.Append(" RETURNING ");
                bool first = true;
                foreach (var property in keyProperties)
                {
                    if (!first)
                        sb.Append(", ");
                    first = false;
                    sb.Append(property.Name);
                }
            }

            var results = connection.Query(sb.ToString(), entityToInsert, transaction: transaction, commandTimeout: commandTimeout);

            // Return the key by assinging the corresponding property in the object - by product is that it supports compound primary keys
            int id = 0;
            foreach (var p in keyProperties)
            {
                var value = ((IDictionary<string, object>)results.First())[p.Name.ToLower()];
                p.SetValue(entityToInsert, value, null);
                if (id == 0)
                    id = Convert.ToInt32(value);
            }
            return id;
        }

        public async Task<int> InsertAsync(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, String tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("insert into {0} ({1}) values ({2})", tableName, columnList, parameterList);

            // If no primary key then safe to assume a join table with not too much data to return
            if (!keyProperties.Any())
                sb.Append(" RETURNING *");
            else
            {
                sb.Append(" RETURNING ");
                bool first = true;
                foreach (var property in keyProperties)
                {
                    if (!first)
                        sb.Append(", ");
                    first = false;
                    sb.Append(property.Name);
                }
            }

            var results = await connection.QueryAsync<dynamic>(sb.ToString(), entityToInsert, transaction: transaction, commandTimeout: commandTimeout).ConfigureAwait(false);

            // Return the key by assinging the corresponding property in the object - by product is that it supports compound primary keys
            int id = 0;
            foreach (var p in keyProperties)
            {
                var value = ((IDictionary<string, object>)results.First())[p.Name.ToLower()];
                p.SetValue(entityToInsert, value, null);
                if (id == 0)
                    id = Convert.ToInt32(value);
            }
            return id;
        }
    }
}
