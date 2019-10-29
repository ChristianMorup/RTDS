using System;
using System.Threading.Tasks;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Wrappers;

namespace RTDS.Monitoring.Monitors
{
    internal abstract class AbstractMonitor : IMonitor
    {
        public abstract event EventHandler<SearchDirectoryArgs> Created;
        protected readonly IFileSystemWatcherWrapper Watcher;

        protected AbstractMonitor(IFileSystemWatcherWrapper watcher)
        {
            Watcher = watcher;
            Guid = Guid.NewGuid();
        }
        public Guid Guid { get; }

        public Task StartMonitoringAsync(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return StarMonitoringAsyncImpl(path);
        }

        protected abstract Task StarMonitoringAsyncImpl(string path);

        public string MonitoredPath => Watcher.Path;
    }
}