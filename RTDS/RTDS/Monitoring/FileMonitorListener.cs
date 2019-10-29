﻿using System.Globalization;
using RTDS.Monitoring.Args;

namespace RTDS.Monitoring
{
    internal class FileMonitorListener : IFileMonitorListener
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IProjectionController _projectionController;

        public FileMonitorListener(IProjectionController projectionController)
        {
            _projectionController = projectionController;
        }

        public void OnNewFileDetected(object sender, SearchDirectoryArgs args)
        {
            Logger.Info(CultureInfo.CurrentCulture, "New file detected: {0}", args.FileName);
            _projectionController.HandleNewFile(args.RelatedMonitorInfo, args.Path);
        }

        public void OnMonitorFinished(object sender, FileMonitorFinishedArgs args)
        {
            args.Monitor.Created -= OnNewFileDetected;
            args.Monitor.Finished -= OnMonitorFinished;
            Logger.Info(CultureInfo.CurrentCulture, "Stopped monitoring: {0}", args.Monitor.MonitoredPath);
        }
    }
}