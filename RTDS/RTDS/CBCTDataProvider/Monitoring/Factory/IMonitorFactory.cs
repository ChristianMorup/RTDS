using RTDS.CBCTDataProvider.Monitoring.Monitors;
using RTDS.DTO;

namespace RTDS.CBCTDataProvider.Monitoring.Factory
{
    internal interface IMonitorFactory
    {
        IFileMonitor CreateFileMonitor();

        IMonitor CreateFolderMonitor();

        ISubfolderMonitorListener CreateFileMonitorListener(PermStorageFolderStructure structure);
    }
}