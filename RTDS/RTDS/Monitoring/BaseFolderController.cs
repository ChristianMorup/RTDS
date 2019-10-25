using System.Globalization;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Monitors;

namespace RTDS.Monitoring
{
    //https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-1.1/czefa0ke(v=vs.71)?redirectedfrom=MSDN

    internal class BaseFolderController
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IMonitor _folderMonitor;
        private readonly IFileController _fileController;

        public BaseFolderController(IMonitorFactory monitorFactory, IFileController fileController)
        {
            _fileController = fileController;
            _folderMonitor = monitorFactory.CreateFolderMonitor();
            _folderMonitor.Created += HandleNewFolder;
        }

        public void StartMonitoring()
        {
            var sourcePath = Configuration.ConfigurationManager.GetConfigurationPaths().BaseSourcePath;
            Logger.Info(CultureInfo.CurrentCulture, "Starts folder monitoring at path: {0}", sourcePath);
            _folderMonitor.StartMonitoringAsync(sourcePath);
        }

        private void HandleNewFolder(object sender, SearchDirectoryArgs args)
        {
            Logger.Info(CultureInfo.CurrentCulture, "New folder detected: {0}", args.Name);
            _fileController.MonitorNewFolderAsync(args.Path, args.Name);
        }
    }
}