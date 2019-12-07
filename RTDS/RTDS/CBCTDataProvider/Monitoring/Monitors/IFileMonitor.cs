using System;
using RTDS.CBCTDataProvider.Monitoring.Args;

namespace RTDS.CBCTDataProvider.Monitoring.Monitors
{
    internal interface IFileMonitor : IMonitor
    {
        event EventHandler<FileMonitorFinishedArgs> Finished;
    }
}