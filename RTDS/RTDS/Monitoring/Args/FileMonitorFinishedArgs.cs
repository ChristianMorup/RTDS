namespace RTDS.Monitoring.Args
{
    internal class FileMonitorFinishedArgs
    {
        public IFileMonitor Monitor { get; }

        public FileMonitorFinishedArgs(IFileMonitor monitor)
        {
            Monitor = monitor;
        }
    }
}