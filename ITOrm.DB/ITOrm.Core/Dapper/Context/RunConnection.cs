using ITOrm.Core.Helper;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace ITOrm.Core.Dapper.Context
{
    public class RunConnection
    {
        public static readonly string connectionString = ConfigHelper.GetConnectionStrings("ITOrmdb");

        public static SqlConnection GetOpenConnection(bool mars = true)
        {
            string cs = connectionString;
            SqlConnection connection = new SqlConnection(cs);
            connection.Open();
            return connection;
        }

        public static SqlConnection GetClosedConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
