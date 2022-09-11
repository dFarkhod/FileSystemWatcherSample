using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderWatcherService
{
    public static class Constants
    {
        public const string ORDERS_FILE_EXTENSION = "*.order";
        public const int MAX_DEGREE_OF_PARALLELISM = 4;
        public const string ORDERSDB_CONN_STRING_NAME = "OrdersDb";

    }
}
