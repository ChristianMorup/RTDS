using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;
using RTDS.CBCTDataProvider.Monitoring.Args;
using RTDS.CBCTDataProvider.Monitoring.Factory;
using RTDS.CBCTDataProvider.Monitoring.Monitors;
using RTDS.CBCTDataProvider.ProjectionProcessing;
using RTDS.DTO;
using RTDS.ExceptionHandling;

namespace RTDS.CBCTDataProvider.Monitoring
{
    internal class SubfolderController : ISubfolderController
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IProjectionFolderCreator _projectionFolderCreator;
        private readonly IMonitorFactory _monitorFactory;
        private readonly IProjectionProcessorFactory _projectionProcessorFactory;
        public event EventHandler<PermFolderCreatedArgs> FolderDetected;

        public SubfolderController(IProjectionFolderCreator projectionFolderCreator, IMonitorFactory monitorFactory, IProjectionProcessorFactory projectionProcessorFactory)
        {
            _projectionFolderCreator = projectionFolderCreator;
            _monitorFactory = monitorFactory;
            _projectionProcessorFactory = projectionProcessorFactory;
        }

        public Task StartNewFileMonitorInNewFolderAsync(string path, string folderName)
        {
            return Task.Run(async () =>
            {
                var folderStructure = await _projectionFolderCreator.CreateFolderStructure();
                var queue = new BlockingCollection<ProjectionInfo>();
                
                var newFileMonitor = await Task.Run(() => _monitorFactory.CreateFileMonitor());
                var processor = StartConsumer(queue, folderStructure);
                SubscribeNewFileMonitorListener(newFileMonitor, folderStructure);
                
                FolderDetected?.Invoke(this, new PermFolderCreatedArgs(folderStructure, processor));
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
        
        private IProjectionProcessor StartConsumer(BlockingCollection<ProjectionInfo> queue, PermStorageFolderStructure folderStructure)
        {
            IProjectionProcessor projectionProcessor =
                _projectionProcessorFactory.CreateProjectionProcessor(queue, folderStructure);
            projectionProcessor.StartConsumingProjections();
            return projectionProcessor;
        }
    }
}