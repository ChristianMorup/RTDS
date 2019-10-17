using System;
using System.IO;
using System.Threading.Tasks;
using RTDS.Configuration;
using RTDS.Monitoring;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Monitors;
using RTDS.Monitoring.Wrappers;
using RTDS.Utility;

namespace RTDS.Application
{
    class Program
    {
        private static MonitorController _monitorController;
        static void Main(string[] args)
        {
            var v = ConfigurationManager.GetConfiguration("BaseTargetPath");

            Console.WriteLine(v);


            IFileSystemWatcherWrapper watcher = new FileSystemWatcherWrapper(new FileSystemWatcher());
            IMonitor folderMonitor = new FolderMonitor(watcher);

      //      _monitorController =
     //           new MonitorController(new MonitorFactory(), new ProjectionInfoFactory(), new FileUtil(), new FolderCreator());

       //     _monitorController.StartMonitoring(@"C:\Users\chrmo\Desktop\RTDS");

            Console.ReadKey();
        }
    }
}
