using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Monitors;

namespace RTDS.Monitoring
{
    //https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-1.1/czefa0ke(v=vs.71)?redirectedfrom=MSDN

    internal class MonitorController
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private Dictionary<Guid, ProjectionInfo> _monitorByQueueMap;
        private readonly IMonitor _folderMonitor;
        private readonly IMonitorFactory _monitorFactory;
        private readonly IProjectionFactory _projectionFactory;
        private readonly IFileUtil _fileUtil;
        private readonly IFolderCreator _folderCreator;

        public MonitorController(IMonitorFactory monitorFactory,
            IProjectionFactory projectionFactory, IFileUtil fileUtil, IFolderCreator folderCreator)
        {
            _monitorByQueueMap = new Dictionary<Guid, ProjectionInfo>();
            _monitorFactory = monitorFactory;
            _folderMonitor = _monitorFactory.CreateFolderMonitor();
            _projectionFactory = projectionFactory;
            _fileUtil = fileUtil;
            _folderCreator = folderCreator;
            _folderMonitor.Created += HandleNewFolder;
        }

        public void StartMonitoring(string path)
        {
            _folderMonitor.StartMonitoringAsync(path);
        }

        private void HandleNewFolder(object sender, SearchDirectoryArgs args)
        {
            var newFileMonitor = _monitorFactory.CreateFileMonitor();

            var permanentStorageXimPath = _folderCreator.CreateFolderStructureForProjectionsAsync();
            
           // var projectionInfo = _projectionFactory.CreateProjectionInfo(permanentStorageXimPath);

         //   _monitorByQueueMap.Add(newFileMonitor.Guid, projectionInfo);

            newFileMonitor.Created += OnNewFileDetected;
            newFileMonitor.Finished += OnMonitorFinished;

            Logger.Info(CultureInfo.CurrentCulture, "New folder detected: {0}", args.Name);

            newFileMonitor.StartMonitoringAsync(args.Path);
        }

 

        private void OnNewFileDetected(object sender, SearchDirectoryArgs args)
        {
            Logger.Info(CultureInfo.CurrentCulture, "New file detected: {0}", args.Name);
            HandleNewFile(args.RelatedMonitor, args.Path);
        }

        private void HandleNewFile(IMonitor relatedMonitor, string path)
        {
            Task task = new Task(async () =>
            {
                ProjectionInfo info;
                if (_monitorByQueueMap.TryGetValue(relatedMonitor.Guid, out info))
                {
                    var fileName = Path.GetFileName(path);
                    var destinationFile = Path.Combine(info.PermanentStorageXimPath, fileName);
                    var destPath = await _fileUtil.CopyFileAsync(path, destinationFile);
                    Logger.Info(CultureInfo.CurrentCulture, "File has been moved");
                }
            });

            task.Start();
        }

        private void OnMonitorFinished(object sender, FileMonitorFinishedArgs args)
        {
            _monitorByQueueMap.Remove(args.Monitor.Guid);
            Logger.Info(CultureInfo.CurrentCulture, "Stops monitoring: {0}", args.Monitor.MonitoredPath);
        }
    }
}