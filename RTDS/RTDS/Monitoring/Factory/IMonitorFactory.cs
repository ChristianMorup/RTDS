using RTDS.DTO;
using RTDS.Monitoring.Monitors;

namespace RTDS.Monitoring.Factory
{
    internal interface IMonitorFactory
    {
        IFileMonitor CreateFileMonitor(ProjectionFolderStructure structure);

        IMonitor CreateFolderMonitor();

        IFileMonitorListener CreateFileMonitorListener();
    }
}