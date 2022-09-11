using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data.SqlClient;

namespace OrderWatcherService
{
    public class OrderRepository
    {
        public OrderRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings[Constants.ORDERSDB_CONN_STRING_NAME].ToString();
        }

        private string _connectionString = string.Empty;

        public void Create(Order order)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd1 = new SqlCommand("SELECT GETDATE()", conn); // todo: replace this part with Insert
                    cmd1.ExecuteNonQuery();
                    Console.WriteLine($"Order {order.UniqueId} has been inserted to database!"); // forst testing
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}
