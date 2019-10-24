using System.IO;
using System.Timers;
using RTDS.DTO;
using RTDS.Monitoring.Monitors;
using RTDS.Monitoring.Wrappers;

namespace RTDS.Monitoring.Factory
{
    internal class MonitorFactory : IMonitorFactory
    {
        public IFileMonitor CreateFileMonitor(ProjectionFolderStructure structure)
        {
            var timer = new TimerWrapper(new Timer());
            var watcher = new FileSystemWatcherWrapper(new FileSystemWatcher());
            return new FileMonitor(watcher, timer, structure);
        }

        public IMonitor CreateFolderMonitor()
        {
            var watcher = new FileSystemWatcherWrapper(new FileSystemWatcher());
            return new FolderMonitor(watcher);
        }
    }
}