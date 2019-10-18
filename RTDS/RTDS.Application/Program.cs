using System;
using System.IO;
using System.Threading.Tasks;
using RTDS.Configuration;
using RTDS.Configuration.Data;
using RTDS.Monitoring;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Monitors;
using RTDS.Monitoring.Wrappers;
using RTDS.Utility;

namespace RTDS.Application
{
    class Program
    {
        private static BaseFolderController _baseFolderController;
        static void Main(string[] args)
        {

            RTDSConfiguration configuration = new RTDSConfiguration
            {
                Paths = new RTDSPaths
                {
                    BaseSourcePath = "C:\\Users\\herni\\OneDrive\\Skrivebord\\RTDS - Source",
                    BaseTargetPath = "C:\\Users\\herni\\OneDrive\\Skrivebord\\RTDS - Target"

                }
            };

            ConfigurationManager.OverrideConfiguration(configuration, true);





            IFileSystemWatcherWrapper watcher = new FileSystemWatcherWrapper(new FileSystemWatcher());
            IMonitor folderMonitor = new FolderMonitor(watcher);

      //      _baseFolderController =
     //           new BaseFolderController(new MonitorFactory(), new ProjectionInfoFactory(), new FileUtil(), new FolderCreator());

       //     _baseFolderController.StartMonitoring(@"C:\Users\chrmo\Desktop\RTDS");

            Console.ReadKey();
        }
    }
}
