using System.Configuration;

namespace OrderWatcherService
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                var ordersPath = ConfigurationManager.AppSettings["OrdersPath"];
                if (string.IsNullOrWhiteSpace(ordersPath))
                    throw new Exception("OrdersPath is empty.");
                
                OrderWatcher.Instance.Init(ordersPath);
                Console.WriteLine("Press Enter to quit the program.");
                Console.WriteLine();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}