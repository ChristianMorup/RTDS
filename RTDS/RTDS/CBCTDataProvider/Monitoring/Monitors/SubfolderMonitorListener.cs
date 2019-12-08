using System.Globalization;
using RTDS.CBCTDataProvider.Monitoring.Args;
using RTDS.CBCTDataProvider.ProjectionProcessing;
using RTDS.ExceptionHandling;

namespace RTDS.CBCTDataProvider.Monitoring.Monitors
{
    internal class SubfolderMonitorListener : ISubfolderMonitorListener
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IProjectionHandler _projectionHandler;

        public SubfolderMonitorListener(IProjectionHandler projectionHandler)
        {
            _projectionHandler = projectionHandler;
        }

        public void OnNewFileDetected(object sender, SearchDirectoryArgs args)
        {
            Logger.Info(CultureInfo.CurrentCulture, "New file detected: {0}", args.FileName);
            TaskWatcher.AddTask(_projectionHandler.HandleNewFile(args.Path));
        }

        public void OnMonitorFinished(object sender, FileMonitorFinishedArgs args)
        {
            args.Monitor.Created -= OnNewFileDetected;
            args.Monitor.Finished -= OnMonitorFinished;
            Logger.Info(CultureInfo.CurrentCulture, "Stopped monitoring: {0}", args.Monitor.MonitoredPath);
        }
    }
}