using ITOrm.Core.Helper;
using MySql.Data.MySqlClient;

namespace ITOrm.Core.Dapper.Context
{
    public class BasicConnection
    {
        public static readonly string connectionString = ConfigHelper.GetConnectionStrings("host");

        public static MySqlConnection GetOpenConnection(bool mars = true)
        {
            string cs = connectionString;
            MySqlConnection connection = new MySqlConnection(cs);
            connection.Open();
            return connection;
        }

        public static MySqlConnection GetClosedConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
