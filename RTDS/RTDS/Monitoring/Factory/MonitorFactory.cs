using System.IO;
using System.Timers;
using RTDS.Monitoring.Monitors;
using RTDS.Monitoring.Wrappers;

namespace RTDS.Monitoring.Factory
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