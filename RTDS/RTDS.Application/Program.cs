using System;
using System.IO;
using System.Threading.Tasks;
using RTDS.Monitoring;

namespace RTDS.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            IFileSystemWatcherWrapper watcher = new FileSystemWatcherWrapper(new FileSystemWatcher());
            FolderMonitor folderMonitor = new FolderMonitor(watcher);

            folderMonitor.FolderCreated += OnFolderCreated;
            
            folderMonitor.StarMonitoringAsync("C:\\Users\\herni\\OneDrive\\Skrivebord\\RTDS_Experimental_Test");
            
            Console.WriteLine("Press 'q' to quit the sample.");
            while (Console.Read() != 'q') ;
        }

        private static void OnFolderCreated(object sender, EventArgs e)
        {
            Console.WriteLine("Folder is created.");
        }
    }
}
