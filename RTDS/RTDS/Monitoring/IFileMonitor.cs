using System;
using RTDS.Monitoring.Args;

namespace RTDS.Monitoring
{
    internal interface IFileMonitor : IMonitor
    {
        event EventHandler<FileMonitorFinishedArgs> Finished;
    }
}