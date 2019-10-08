using System;
using System.IO;
using System.Threading.Tasks;
using RTDS.Monitoring;

namespace RTDS.Application
{
    class Program
    {
        private static FolderMonitor folderMonitor;
        static void Main(string[] args)
        {
            var systemwatcher = new FileSystemWatcher();
            IFileSystemWatcherWrapper watcher = new FileSystemWatcherWrapper(systemwatcher);
            folderMonitor = new FolderMonitor(watcher);

            folderMonitor.FolderCreated += OnFolderCreated;
            
            folderMonitor.StarMonitoringAsync("C:\\Users\\chrmo\\Desktop\\RTDS");
            
            Console.WriteLine("Press 'q' to quit.");
            while (Console.Read() != 'q') ;
        }

        private static void OnFolderCreated(object sender, EventArgs e)
        {
            Console.WriteLine("Folder is created.");
        }
    }
}
