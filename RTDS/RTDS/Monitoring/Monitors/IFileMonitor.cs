using System;
using RTDS.Monitoring.Args;

namespace RTDS.Monitoring.Monitors
{
    internal interface IFileMonitor : IMonitor
    {
        event EventHandler<FileMonitorFinishedArgs> Finished;
    }
}