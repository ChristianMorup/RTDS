using System.IO;
using System.Timers;
using RTDS.CBCTDataProvider.Monitoring.Monitors;
using RTDS.CBCTDataProvider.Monitoring.Wrappers;
using RTDS.CBCTDataProvider.ProjectionProcessing;
using RTDS.DTO;
using RTDS.Utility;

namespace RTDS.CBCTDataProvider.Monitoring.Factory
{
    internal class MonitorFactory : IMonitorFactory
    {
        public IFileMonitor CreateFileMonitor()
        {
            var timer = new TimerWrapper(new Timer());
            var watcher = new FileSystemWatcherWrapper(new FileSystemWatcher());
            return new FileMonitor(watcher, timer);
        }

        public IMonitor CreateFolderMonitor()
        {
            var watcher = new FileSystemWatcherWrapper(new FileSystemWatcher());
            return new FolderMonitor(watcher);
        }
    }
}