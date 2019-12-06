using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EvilDICOM.Core;
using RTDS.CTDataProvision;
using RTDS.Monitoring;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Factory;
using RTDS.Utility;
using VMS.TPS.Common.Model.API;

namespace RTDS
{
    public class RTDSFacade
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly DataFlowSynchronizer _dataFlowSynchronizer;
        private readonly List<IEventReceiver> _eventReceivers;

        public RTDSFacade()
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

        private IFileController CreateFileController()
        {
            var monitorFactory = new MonitorFactory();
            var folderCreator = new DefaultProjectionFolderCreator(new FolderCreator(new FileUtil()));
            return new FileController(folderCreator, monitorFactory);
        }

        private BaseFolderController CreateBaseFolderController(IFileController fileController)
        {
            return new BaseFolderController(new MonitorFactory(), fileController);
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

        public void GetCTScan(Application app, string patientId)
        {
            ScriptExecutor scriptExecutor = new ScriptExecutor();
            scriptExecutor.Execute(app, patientId, _dataFlowSynchronizer);
        }

        public void GetCTScan(string patientId)
        {
            ScriptExecutor scriptExecutor = new ScriptExecutor();
            scriptExecutor.Execute(patientId, _dataFlowSynchronizer);
        }

        public void CorrectCTScan(string patientId, string cbctId)
        {
            ScriptExecutor scriptExecutor = new ScriptExecutor();
            scriptExecutor.Execute(patientId, cbctId, _dataFlowSynchronizer);
        }

        public void CorrectCTScan(Application app, string patientId, string cbctId)
        {
            ScriptExecutor scriptExecutor = new ScriptExecutor();
            scriptExecutor.Execute(app, patientId, cbctId, _dataFlowSynchronizer);
        }

        private void OnFolderCreated(object sender, PermFolderCreatedArgs args)
        {
            Task.Run(() =>
            {
                foreach (var receiver in _eventReceivers)
                {
                    receiver?.OnFolderCreated(args.Id);
                }
            });
        }
    }
}