using System;
using System.Threading.Tasks;
using RTDS.Monitoring.Args;

namespace RTDS.Monitoring
{
    internal interface  IMonitor
    {
        Task StartMonitoringAsync(string path);

        string MonitoredPath {get; }

        Guid Guid { get; }

        event EventHandler<SearchDirectoryArgs> Created;
    }
}