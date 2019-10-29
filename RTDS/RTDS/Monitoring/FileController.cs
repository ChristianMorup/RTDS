using System.Globalization;
using System.Threading.Tasks;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Monitors;

namespace RTDS.Monitoring
{
    internal class FileController : IFileController
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IMonitorFactory _monitorFactory;
        private readonly IProjectionFolderCreator _projectionFolderCreator;

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
                var newFileMonitor = await Task.Run(() => _monitorFactory.CreateFileMonitor(folderStructure));

                SubscribeNewFileMonitorListener(newFileMonitor);

                Logger.Info(CultureInfo.CurrentCulture, "Starts file monitoring at path: {0}", path);
                newFileMonitor.StartMonitoringAsync(path);
            });
        }

        private void SubscribeNewFileMonitorListener(IFileMonitor monitor)
        {
            var fileMonitorListener = _monitorFactory.CreateFileMonitorListener();

            monitor.Created += fileMonitorListener.OnNewFileDetected;
            monitor.Finished += fileMonitorListener.OnMonitorFinished;
        }
    }
}