using ITOrm.Core.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Core.Dapper.Context
{
    public class DapperHelper
    {
        public static ILogger log = LogManager.GetCurrentClassLogger();
        public static List<T> ExecuteProcedure<T>(string ProcName, object param = null)
        {
            try
            {
                using (SqlConnection connection = RunConnection.GetOpenConnection())
                {
                    List<T> list = new List<T>();
                    list = connection.Query<T>(ProcName,
                                            param,
                                            null,
                                            true,
                                            null,
                                            CommandType.StoredProcedure).ToList();
                    return list;
                }
            }
            catch (Exception e)
            {
                log.Error($"执行存储过程:ProcName={ProcName},param:{param.ToString()}", e);
            }
            return new List<T>();
        }



        /// <summary>
        /// 执行返回第一行第一列值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlContent"></param>
        /// <returns></returns>
        public static T ExecScalarSql<T>(string sqlContent)
        {
            try
            {
                using (SqlConnection connection = RunConnection.GetOpenConnection())
                {
                    var resutl = connection.ExecuteScalar<T>(sqlContent,null,null,null,CommandType.Text);
                    connection.Close();
                    return resutl;
                }
            }
            catch (Exception ex)
            {
                log.Error($"执行sql:{sqlContent}", ex);
            }
            return default(T);
        }

    }
}
