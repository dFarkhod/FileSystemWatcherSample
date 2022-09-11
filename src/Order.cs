using Polly;
using System.Configuration;
using System.Globalization;
using System.Text;

namespace OrderWatcherService
{
    public class Order
    {
        public string UniqueId { get; set; }
        public long StaffId { get; set; }
        public long ProductId { get; set; }
        public long CustomerId { get; set; }
        public double UnitPrice { get; set; }
        public double Quantity { get; set; }
        public DateTime OrderDate { get; set; }

        public static class Factory
        {
            private static string _pathToOrderFile = string.Empty;
            private static string[] _lines = null;
            private static List<Order> _orders = null;
            private static string _orderFileName = string.Empty;

            public static List<Order> GetOrders(string pathToOrdersFile)
            {
                _pathToOrderFile = pathToOrdersFile;
                _orderFileName = Path.GetFileNameWithoutExtension(_pathToOrderFile);
                _orders = new List<Order>();
                ReadLinesFromOrdersFile();
                BuildOrders();
                return _orders;
            }

            private static void BuildOrders()
            {
                foreach (string line in _lines)
                {
                    var order = BuildOrderFromString(line);
                    if (order != null)
                        _orders.Add(order);
                }
                Console.WriteLine($"Processing of {_orderFileName} completed.");
            }

            private static void ReadLinesFromOrdersFile()
            {
                var MAX_RETRIES = 3;

                var retryPolicy = Policy.Handle<IOException>()
                    .WaitAndRetry(retryCount: MAX_RETRIES, sleepDurationProvider: (attemptCount) => TimeSpan.FromMilliseconds(200),
                    onRetry: (exception, sleepDuration, attemptNumber, context) =>
                    {
                        Console.WriteLine($"Error occurred. Retrying in {sleepDuration}. {attemptNumber} / {MAX_RETRIES}");
                    });


                retryPolicy.Execute(() =>
                {
                    if (File.Exists(_pathToOrderFile))
                        _lines = File.ReadAllLines(_pathToOrderFile);
                });
            }

            private static Order BuildOrderFromString(string line)
            {
                if (string.IsNullOrWhiteSpace(line))
                    return null;

                Order order = new();
                try
                {
                    var orderItems = line.Split(";;");
                    order.StaffId = long.TryParse(orderItems[0], out long staffId) ? staffId : 0;
                    order.ProductId = long.TryParse(orderItems[1], out long productId) ? productId : 0;
                    order.CustomerId = long.TryParse(orderItems[2], out long customerId) ? customerId : 0;
                    order.UnitPrice = double.TryParse(orderItems[3], out double unitPrice) ? unitPrice : 0;
                    order.Quantity = double.TryParse(orderItems[4], out double quantity) ? quantity : 0;
                    StringBuilder sb = new();
                    sb.Append(_orderFileName).Append("-").Append(order.StaffId).Append("-").Append(order.ProductId).Append("-").Append(order.CustomerId).Append("-").Append(order.Quantity);
                    order.UniqueId = sb.ToString();
                    order.OrderDate = DateTime.Now;
                }
                catch (Exception)
                {
                    // todo: logging
                    return null;
                }
                return order;
            }
        }
    }
}
