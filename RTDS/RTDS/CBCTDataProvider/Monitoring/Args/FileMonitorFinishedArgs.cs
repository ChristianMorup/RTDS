using System;
using RTDS.CBCTDataProvider.Monitoring.Monitors;

namespace RTDS.CBCTDataProvider.Monitoring.Args
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