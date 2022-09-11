namespace OrderWatcherService
{
    public class OrderManager
    {
        public static OrderManager Instance
        {
            get
            {
                return Nested.OrderManager;
            }
        }

        private class Nested
        {
            static Nested() { }
            internal static readonly OrderManager OrderManager = new();
        }

        public void ProcessNewOrders(string ordersPath)
        {
            var orders = Order.Factory.GetOrders(ordersPath);
            if (orders.Count > 0)
            {
                InsertOrdersToDatabase(orders);
                MoveOrdersFile(ordersPath);
                orders.Clear();
            }
        }

        private static void InsertOrdersToDatabase(List<Order> orders)
        {
            OrderRepository ordersRepo = new();
            foreach (var order in orders)
            {
                ordersRepo.Create(order);
            }
        }

        private static void MoveOrdersFile(string ordersPath)
        {
            if (File.Exists(ordersPath))
            {
                string processedDirectoryPath = Path.Combine(Directory.GetParent(ordersPath).FullName, "Processed");
                if (!Directory.Exists(processedDirectoryPath))
                    Directory.CreateDirectory(processedDirectoryPath);

                File.Move(ordersPath, Path.Combine(processedDirectoryPath, Path.GetFileName(ordersPath)), overwrite: true);
            }
        }
    }
}
