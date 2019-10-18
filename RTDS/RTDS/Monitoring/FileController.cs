using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using RTDS.DTO;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Monitors;

namespace RTDS.Monitoring
{
    internal class FileController : IFileController
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IMonitorFactory _monitorFactory;
        private readonly IProjectionController _projectionController;
        private Dictionary<Guid, ProjectionInfo> _monitorByQueueMap;

        public FileController(IProjectionController projectionController, IMonitorFactory monitorFactory)
        {
            _projectionController = projectionController;
            _monitorFactory = monitorFactory;
            _monitorByQueueMap = new Dictionary<Guid, ProjectionInfo>();
        }

        public Task MonitorNewFolderAsync(string path, string folderName)
        {
            return Task.Run(async () =>
            {
                Task<IFileMonitor> createMonitorTask = Task.Run(() => _monitorFactory.CreateFileMonitor());

                var projectionInfo = await _projectionController.CreateProjectionInfo();
                var newFileMonitor = await createMonitorTask;

                _monitorByQueueMap.Add(newFileMonitor.Guid, projectionInfo);

                newFileMonitor.Created += OnNewFileDetected;
                newFileMonitor.Finished += OnMonitorFinished;

                newFileMonitor.StartMonitoringAsync(path);
            });
        }

        private void OnNewFileDetected(object sender, SearchDirectoryArgs args)
        {
            Logger.Info(CultureInfo.CurrentCulture, "New file detected: {0}", args.Name);
            _projectionController.HandleNewFile(args.RelatedMonitor, args.Path, _monitorByQueueMap);
        }

        private void OnMonitorFinished(object sender, FileMonitorFinishedArgs args)
        {
            _monitorByQueueMap.Remove(args.Monitor.Guid);
            Logger.Info(CultureInfo.CurrentCulture, "Stops monitoring: {0}", args.Monitor.MonitoredPath);
        }
    }
}