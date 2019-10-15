using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Factory;

namespace RTDS.Monitoring
{
    //https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-1.1/czefa0ke(v=vs.71)?redirectedfrom=MSDN

    internal class MonitorController
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private List<IFileMonitor> _fileMonitors;
        private readonly IMonitor _folderMonitor;
        private readonly IMonitorFactory _factory;

        public MonitorController(IMonitor folderMonitor, IMonitorFactory factory)
        {
            _fileMonitors = new List<IFileMonitor>();
            _folderMonitor = folderMonitor;
            _factory = factory;
            _folderMonitor.Created += HandleNewFolder;
        }

        public void StartMonitoring(string path)
        {
            _folderMonitor.StartMonitoringAsync(path);
        }

        private void HandleNewFolder(object sender, SearchDirectoryArgs args)
        {
            var newFileMonitor = _factory.CreateFileMonitor();
            _fileMonitors.Add(newFileMonitor);

            newFileMonitor.Created += OnNewFileDetected;
            newFileMonitor.Finished += OnMonitorFinished;

            Logger.Info(CultureInfo.CurrentCulture, "New folder detected: {0}", args.Name);
            
            //TODO Create queue and notify some listener that it has been created. 

            newFileMonitor.StartMonitoringAsync(args.Path);
        }

        private void OnNewFileDetected(object sender, SearchDirectoryArgs args)
        {
            Logger.Info(CultureInfo.CurrentCulture, "New file detected: {0}", args.Name);

            //TODO Create DTO and post in queue
        }

        private void OnMonitorFinished(object sender, FileMonitorFinishedArgs args)
        {
            _fileMonitors.Remove(args.Monitor);
            Logger.Info(CultureInfo.CurrentCulture,"Stops monitoring: {0}", args.Monitor.MonitoredPath);
        }
    }
}
