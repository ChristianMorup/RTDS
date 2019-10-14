using System.IO;
using System.Timers;
using RTDS.Monitoring.Wrapper;

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
    }
}