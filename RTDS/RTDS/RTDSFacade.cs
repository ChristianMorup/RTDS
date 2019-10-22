using RTDS.Monitoring;
using RTDS.Monitoring.Factory;
using RTDS.Utility;

namespace RTDS
{
    public class RTDSFacade
    {
        public void StartMonitoring()
        {
            var projectionController = CreateProjectionController();
            var fileController = CreateFileController(projectionController);
            var baseFolderController = CreateBaseFolderController(fileController);

            baseFolderController.StartMonitoring();
        }

        private IProjectionController CreateProjectionController()
        {
            var projectionFactory = new ProjectionInfoFactory();
            var folderCreator = new FolderCreator(new FileUtil());
            return new ProjectionController(folderCreator, projectionFactory, new FileUtil());
        }

        private IFileController CreateFileController(IProjectionController projectionController)
        {
            var monitorFactory = new MonitorFactory();
            return new FileController(projectionController, monitorFactory);
        }

        private BaseFolderController CreateBaseFolderController(IFileController fileController)
        {
            return new BaseFolderController(new MonitorFactory(), fileController);
        }
    }
}