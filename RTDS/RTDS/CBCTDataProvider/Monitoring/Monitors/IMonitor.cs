using System;
using System.Threading.Tasks;
using RTDS.CBCTDataProvider.Monitoring.Args;

namespace RTDS.CBCTDataProvider.Monitoring.Monitors
{
    internal interface IMonitor
    {
        Task StartMonitoringAsync(string path);

        string MonitoredPath { get; }


        event EventHandler<SearchDirectoryArgs> Created;
    }
}