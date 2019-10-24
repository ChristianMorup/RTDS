using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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


        public FileController(IProjectionController projectionController, IMonitorFactory monitorFactory)
        {
            _projectionController = projectionController;
            _monitorFactory = monitorFactory;
        }

        public Task MonitorNewFolderAsync(string path, string folderName)
        {
            return Task.Run(async () =>
            {
                var folderStructure = await _projectionController.CreateProjectionFolderStructure();
                var newFileMonitor = await Task.Run(() => _monitorFactory.CreateFileMonitor(folderStructure));
                
                newFileMonitor.Created += OnNewFileDetected;
                newFileMonitor.Finished += OnMonitorFinished;

                newFileMonitor.StartMonitoringAsync(path);
            });
        }

        private void OnNewFileDetected(object sender, SearchDirectoryArgs args)
        {
            Logger.Info(CultureInfo.CurrentCulture, "New file detected: {0}", args.Name);
            _projectionController.HandleNewFile(args.RelatedMonitorInfo, args.Path);
        }

        private void OnMonitorFinished(object sender, FileMonitorFinishedArgs args)
        {
            args.Monitor.Created -= OnNewFileDetected;
            args.Monitor.Finished -= OnMonitorFinished;
            Logger.Info(CultureInfo.CurrentCulture, "Stopped monitoring: {0}", args.Monitor.MonitoredPath);
        }
    }
}