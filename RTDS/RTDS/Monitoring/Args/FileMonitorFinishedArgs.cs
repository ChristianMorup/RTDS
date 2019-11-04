using System;
using RTDS.Monitoring.Monitors;

namespace RTDS.Monitoring.Args
{
    internal class FileMonitorFinishedArgs : EventArgs
    {
        public FileMonitorFinishedArgs(IFileMonitor monitor)
        {
            Monitor = monitor;
        }

        public IFileMonitor Monitor { get; }
    }
}