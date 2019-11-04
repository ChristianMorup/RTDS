using RTDS.Monitoring;
using RTDS.Monitoring.Factory;
using RTDS.Utility;

namespace RTDS
{
    public class RTDSFacade
    {
        public void StartMonitoring()
        {
            var fileController = CreateFileController();
            var baseFolderController = CreateBaseFolderController(fileController);

            baseFolderController.StartFolderMonitor();
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
    }
}