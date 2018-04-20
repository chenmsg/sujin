using Clump.Core.Helper;
using Clump.Data.Models;
using MySql.Data.MySqlClient;

namespace Clump.Data.Models.Host.Context
{
    public class RunConnection
    {
        public static readonly string connectionString = ConfigHelper.GetConnectionStrings("clump_host");

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
