namespace RTDS.Monitoring.Factory
{
    internal interface IMonitorFactory
    {
        IFileMonitor CreateFileMonitor();
    }
}