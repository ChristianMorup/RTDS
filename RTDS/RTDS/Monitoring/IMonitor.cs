using System;
using System.Threading.Tasks;
using RTDS.Monitoring.Args;

namespace RTDS.Monitoring
{
    internal interface  IMonitor
    {
        Task StartMonitoringAsync(string path);

        string MonitoredPath {get; }

        event EventHandler<SearchDirectoryArgs> Created;
    }
}