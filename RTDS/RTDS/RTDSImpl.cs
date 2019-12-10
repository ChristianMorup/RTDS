using System.Collections.Generic;
using System.Threading.Tasks;
using RTDS.CBCTDataProvider.Monitoring;
using RTDS.CBCTDataProvider.Monitoring.Factory;
using RTDS.CBCTDataProvider.ProjectionProcessing.Args;
using RTDS.CBCTDataProvider.ProjectionProcessing.Factory;
using RTDS.CTDataProvider;
using RTDS.CTDataProvider.Callbacks;
using RTDS.ExceptionHandling;
using RTDS.Utility;
using VMS.TPS.Common.Model.API;

namespace RTDS
{
    public class RTDSImpl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly DataFlowSynchronizer _dataFlowSynchronizer;
        private readonly List<IEventReceiver> _eventReceivers;

        public RTDSImpl()
        {
            _dataFlowSynchronizer = new DataFlowSynchronizer();
            _eventReceivers = new List<IEventReceiver>();
        }

        public bool StartMonitoring()
        {
            if (TaskWatcher.HasSubscriber())
            {
                var fileController = CreateFileController();
                fileController.FolderDetected += _dataFlowSynchronizer.OnFolderCreated;
                fileController.FolderDetected += OnFolderCreated;
                var baseFolderController = CreateBaseFolderController(fileController);

                baseFolderController.StartFolderMonitor();
                return true;
            }

            Logger.Fatal("No errorhandler is subscribed.");
            return false;
        }

        private ISubfolderController CreateFileController()
        {
            var monitorFactory = new MonitorFactory();
            var folderCreator = new DefaultProjectionFolderCreator(new FolderCreator(new FileUtil()));
            return new SubfolderController(folderCreator, monitorFactory, new ProjectionPipelineFactory());
        }

        private BaseFolderController CreateBaseFolderController(ISubfolderController subfolderController)
        {
            return new BaseFolderController(new MonitorFactory(), subfolderController);
        }

        public void SubscribeEventReceiver(IEventReceiver eventReceiver)
        {
            _eventReceivers.Add(eventReceiver);
        }

        public void SubscribeErrorHandler(IErrorHandler errorHandler)
        {
            TaskWatcher.AddErrorListener(errorHandler);
        }

        public void UnsubscribeErrorHandler(IErrorHandler errorHandler)
        {
            TaskWatcher.RemoveErrorListener(errorHandler);
        }

        public void GetCTScan(Application app, string patientId, ICTScanRetrievedCallback callback)
        {
            ScriptExecutor scriptExecutor = new ScriptExecutor();
            TaskWatcher.WatchTask(scriptExecutor.Execute(app, patientId,
                new List<ICTScanRetrievedCallback> {callback, _dataFlowSynchronizer}));
        }

        public void GetCTScan(string patientId, ICTScanRetrievedCallback callback)
        {
            ScriptExecutor scriptExecutor = new ScriptExecutor();
            TaskWatcher.WatchTask(scriptExecutor.Execute(patientId, new List<ICTScanRetrievedCallback> {callback, _dataFlowSynchronizer}));
        }

        public void CorrectCTScan(string patientId, string cbctId, ICorrectedCTScanRetrievedCallback callback)
        {
            ScriptExecutor scriptExecutor = new ScriptExecutor();
            TaskWatcher.WatchTask(scriptExecutor.Execute(patientId, cbctId,
                new List<ICorrectedCTScanRetrievedCallback> {callback, _dataFlowSynchronizer}));
        }

        public void CorrectCTScan(Application app, string patientId, string cbctId,
            ICorrectedCTScanRetrievedCallback callback)
        {
            ScriptExecutor scriptExecutor = new ScriptExecutor();
            TaskWatcher.WatchTask(scriptExecutor.Execute(app, patientId, cbctId,
                new List<ICorrectedCTScanRetrievedCallback> {callback, _dataFlowSynchronizer}));
        }

        private void OnFolderCreated(object sender, PipelineStartedArgs args)
        {
            Task.Run(() =>
            {
                foreach (var receiver in _eventReceivers)
                {
                    receiver?.OnFolderCreated(args.Id);
                    if (receiver != null) args.Processor.ImageReconstructed += receiver.OnReconstructedImageFinished;
                }
            });
        }
    }
}