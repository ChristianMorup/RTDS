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
            _monitorController =
                new MonitorController(new MonitorFactory(), new ProjectionInfoFactory(), new FileMover(), new FolderCreator());

            _monitorController.StartMonitoring(@"C:\Users\chrmo\Desktop\RTDS");

            Console.ReadKey();
        }
    }
}
