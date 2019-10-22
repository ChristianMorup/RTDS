using RTDS.Monitoring.Monitors;

namespace RTDS.Monitoring.Factory
{
    internal interface IMonitorFactory
    {
        IFileMonitor CreateFileMonitor();

        IMonitor CreateFolderMonitor();
    }
}