using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;
using RTDS.CBCTDataProvider.Monitoring.Args;
using RTDS.CBCTDataProvider.Monitoring.Factory;
using RTDS.CBCTDataProvider.Monitoring.Monitors;
using RTDS.CBCTDataProvider.ProjectionProcessing;
using RTDS.CBCTDataProvider.ProjectionProcessing.Args;
using RTDS.CBCTDataProvider.ProjectionProcessing.Factory;
using RTDS.DTO;
using RTDS.ExceptionHandling;

namespace RTDS.CBCTDataProvider.Monitoring
{
    internal class SubfolderController : ISubfolderController
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IProjectionFolderCreator _projectionFolderCreator;
        private readonly IMonitorFactory _monitorFactory;
        private readonly IProjectionPipelineFactory _projectionPipelineFactory;
        public event EventHandler<PipelineStartedArgs> PipelineStarted;

        public SubfolderController(IProjectionFolderCreator projectionFolderCreator, IMonitorFactory monitorFactory, IProjectionPipelineFactory projectionPipelineFactory)
        {
            _projectionFolderCreator = projectionFolderCreator;
            _monitorFactory = monitorFactory;
            _projectionPipelineFactory = projectionPipelineFactory;
        }

        public Task StartNewFileMonitorInNewFolderAsync(string path, string folderName)
        {
            return Task.Run(async () =>
            {
                var folderStructure = await _projectionFolderCreator.CreateFolderStructure();

                var newFileMonitor = await Task.Run(() => _monitorFactory.CreateFileMonitor());
                var processor = StartPipeline(newFileMonitor, folderStructure);
                
                TaskWatcher.AddTask(Task.Run(() => PipelineStarted?.Invoke(this, new PipelineStartedArgs(folderStructure, processor))));
                
                Logger.Info(CultureInfo.CurrentCulture, "Starts file monitoring at path: {0}", path);
                TaskWatcher.AddTask(newFileMonitor.StartMonitoringAsync(path));

            });
        }

        private IReconstructionProcessor StartPipeline(IFileMonitor fileMonitor, PermStorageFolderStructure folderStructure)
        {
            BlockingCollection<TempProjectionInfo> queue1 = new BlockingCollection<TempProjectionInfo>();
            BlockingCollection<PermProjectionInfo> queue2 = new BlockingCollection<PermProjectionInfo>();

            var eventHandler = _projectionPipelineFactory.CreateFileMonitorListener(folderStructure, queue1);
            fileMonitor.Created += eventHandler.OnNewFileDetected;
            fileMonitor.Finished += eventHandler.OnMonitorFinished;

            var copier = _projectionPipelineFactory.CreateProjectionCopier(queue1, queue2, folderStructure);
            TaskWatcher.AddTask(copier.StartCopyingFiles());
            
            var reconstructionProcessor = _projectionPipelineFactory.CreateReconstructionProcessor(queue2, folderStructure);
            TaskWatcher.AddTask(reconstructionProcessor.StartConsumingProjections());
            return reconstructionProcessor;
        }
    }
}