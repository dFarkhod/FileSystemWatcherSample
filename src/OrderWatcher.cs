using System.Configuration;

namespace OrderWatcherService
{
    public class OrderWatcher
    {
        public static OrderWatcher Instance
        {
            get
            {
                return Nested.OrderWatcher;
            }
        }

        private class Nested
        {
            static Nested() { }
            internal static readonly OrderWatcher OrderWatcher = new OrderWatcher();
        }

        private string _ordersPath = string.Empty;
        public void Init(string ordersPath)
        {
            _ordersPath = ordersPath;
            CreateFileSystemWatcher();
            ProcessExistingFiles();
        }

        private void ProcessExistingFiles()
        {
            var orderfiles = Directory.GetFiles(_ordersPath, Constants.ORDERS_FILE_EXTENSION);
            foreach (var orderFile in orderfiles)
            {
                OrderManager.Instance.ProcessNewOrders(orderFile);
            }
        }

        private void CreateFileSystemWatcher()
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = _ordersPath;
            watcher.IncludeSubdirectories = false;
            watcher.NotifyFilter = NotifyFilters.CreationTime |
                                   NotifyFilters.FileName |
                                   NotifyFilters.LastWrite |
                                   NotifyFilters.Size;

            watcher.Filter = Constants.ORDERS_FILE_EXTENSION;
            watcher.Created += OnCreated;
            watcher.Renamed += OnRenamed;
            watcher.EnableRaisingEvents = true;
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            OrderManager.Instance.ProcessNewOrders(e.FullPath);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
           OrderManager.Instance.ProcessNewOrders(e.FullPath);
        }


    }
}
