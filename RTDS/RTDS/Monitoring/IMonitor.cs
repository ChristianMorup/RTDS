using System;
using System.Threading.Tasks;
using RTDS.Monitoring.Args;

namespace RTDS.Monitoring
{
    internal interface  IMonitor
    {
        Task StartMonitoringAsync(string path);

        event EventHandler<SearchDirectoryArgs> Created;
    }
}