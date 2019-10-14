namespace RTDS.Monitoring.Args
{
    internal class FileMonitorFinishedArgs
    {
        public FileMonitorFinishedArgs(IFileMonitor monitor)
        {
            Monitor = monitor;
        }

        public IFileMonitor Monitor { get; }
    }
}