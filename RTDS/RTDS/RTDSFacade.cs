using System.Configuration;
using System.Linq;
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

        public bool StartMonitoring()
        {
            if (TaskWatcher.HasSubscriber())
            {
                var fileController = CreateFileController();
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
            Patient pt = app.OpenPatientById(patientId);
            var courses = pt.Courses;
            var planSetup = courses.First().PlanSetups.First();

            ScriptCT scriptCt = new ScriptCT();
            scriptCt.Execute(planSetup, pt);
        }

        public void GetCTScan(string patientId)
        {
            var app = Application.CreateApplication(null, null);
            Patient pt = app.OpenPatientById(patientId);
            var courses = pt.Courses;
            var planSetup = courses.First().PlanSetups.First();

            ScriptCT scriptCt = new ScriptCT();
            scriptCt.Execute(planSetup, pt);
        }
    }
}