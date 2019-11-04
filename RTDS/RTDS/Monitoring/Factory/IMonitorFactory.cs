using RTDS.DTO;
using RTDS.Monitoring.Monitors;

namespace RTDS.Monitoring.Factory
{
    internal interface IMonitorFactory
    {
        IFileMonitor CreateFileMonitor();

        IMonitor CreateFolderMonitor();

        IFileMonitorListener CreateFileMonitorListener(PermStorageFolderStructure structure);
    }
}