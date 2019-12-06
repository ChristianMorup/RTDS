using System;
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
        private readonly IProjectionFolderCreator _projectionFolderCreator;
        private readonly IMonitorFactory _monitorFactory;
        public event EventHandler<PermFolderCreatedArgs> FolderDetected;

        public FileController(IProjectionFolderCreator projectionFolderCreator, IMonitorFactory monitorFactory)
        {
            _projectionFolderCreator = projectionFolderCreator;
            _monitorFactory = monitorFactory;
        }

        public Task StartNewFileMonitorInNewFolderAsync(string path, string folderName)
        {
            return Task.Run(async () =>
            {
                var folderStructure = await _projectionFolderCreator.CreateFolderStructure();
                FolderDetected?.Invoke(this, new PermFolderCreatedArgs(folderStructure));
                var newFileMonitor = await Task.Run(() => _monitorFactory.CreateFileMonitor());

                SubscribeNewFileMonitorListener(newFileMonitor, folderStructure);

                Logger.Info(CultureInfo.CurrentCulture, "Starts file monitoring at path: {0}", path);
                TaskWatcher.AddTask(newFileMonitor.StartMonitoringAsync(path));
            });
        }

        private void SubscribeNewFileMonitorListener(IFileMonitor monitor, PermStorageFolderStructure structure)
        {
            var fileMonitorListener = _monitorFactory.CreateFileMonitorListener(structure);

            monitor.Created += fileMonitorListener.OnNewFileDetected;
            monitor.Finished += fileMonitorListener.OnMonitorFinished;
        }
    }
}