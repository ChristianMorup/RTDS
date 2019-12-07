using System.Globalization;
using RTDS.CBCTDataProvider.Monitoring.Args;
using RTDS.CBCTDataProvider.Monitoring.Factory;
using RTDS.CBCTDataProvider.Monitoring.Monitors;
using RTDS.ExceptionHandling;

namespace RTDS.CBCTDataProvider.Monitoring
{
    internal class BaseFolderController
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IMonitor _folderMonitor;
        private readonly ISubfolderController _subfolderController;

        public BaseFolderController(IMonitorFactory monitorFactory, ISubfolderController subfolderController)
        {
            _subfolderController = subfolderController;
            _folderMonitor = monitorFactory.CreateFolderMonitor();
            _folderMonitor.Created += HandleNewFolder;
        }

        public void StartFolderMonitor()
        {
            var sourcePath = Configuration.ConfigurationManager.GetConfigurationPaths().BaseSourcePath;
            Logger.Info(CultureInfo.CurrentCulture, "Starts folder monitoring at path: {0}", sourcePath);
            TaskWatcher.WatchTask(_folderMonitor.StartMonitoringAsync(sourcePath));
        }

        private void HandleNewFolder(object sender, SearchDirectoryArgs args)
        {
            Logger.Info(CultureInfo.CurrentCulture, "New folder detected: {0}", args.FileName);
            TaskWatcher.AddTask(_subfolderController.StartNewFileMonitorInNewFolderAsync(args.Path, args.FileName));
        }
    }
}