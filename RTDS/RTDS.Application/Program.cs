using System;
using System.IO;
using System.Threading.Tasks;
using RTDS.Monitoring;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Wrapper;

namespace RTDS.Application
{
    class Program
    {
        private static MonitorController _monitorController;
        static void Main(string[] args)
        {
            IFileSystemWatcherWrapper watcher = new FileSystemWatcherWrapper(new FileSystemWatcher());
            IMonitor folderMonitor = new FolderMonitor(watcher);
            _monitorController = new MonitorController(folderMonitor, new MonitorFactory());

            _monitorController.StartMonitoring(@"C:\Users\mr_pi\OneDrive\Music\Skrivebord\Test");

            Console.ReadKey();
        }

        private static void OnFolderCreated(object sender, EventArgs e)
        {
            Console.WriteLine("Folder is created.");
        }
    }
}
