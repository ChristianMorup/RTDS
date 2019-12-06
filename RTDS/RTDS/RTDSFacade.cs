using System.Globalization;
using System.Linq;
using EvilDICOM.Core;
using RTDS.CTDataProvision;
using RTDS.Monitoring;
using RTDS.Monitoring.Factory;
using RTDS.Utility;
using RTDS.VarianAPI;
using VMS.TPS.Common.Model.API;

namespace RTDS
{
    public class RTDSFacade
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly DataFlowSynchronizer _dataFlowSynchronizer;

        public RTDSFacade()
        {
            this._dataFlowSynchronizer = new DataFlowSynchronizer();
        }


        public bool StartMonitoring()
        {
            if (TaskWatcher.HasSubscriber())
            {
                var fileController = CreateFileController();
                fileController.FolderDetected += _dataFlowSynchronizer.OnFolderCreated;
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

        public void SubscribeErrorHandler(AbstractErrorHandler errorHandler)
        {
            TaskWatcher.AddErrorListener(errorHandler);
        }

        public void UnsubscribeErrorHandler(AbstractErrorHandler errorHandler)
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
    }
}