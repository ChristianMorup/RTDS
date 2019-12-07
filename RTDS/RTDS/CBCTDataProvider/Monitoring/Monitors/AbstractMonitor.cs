using System;
using System.Threading.Tasks;
using RTDS.CBCTDataProvider.Monitoring.Args;
using RTDS.CBCTDataProvider.Monitoring.Wrappers;

namespace RTDS.CBCTDataProvider.Monitoring.Monitors
{
    internal abstract class AbstractMonitor : IMonitor
    {
        public abstract event EventHandler<SearchDirectoryArgs> Created;
        protected readonly IFileSystemWatcherWrapper Watcher;

        protected AbstractMonitor(IFileSystemWatcherWrapper watcher)
        {
            Watcher = watcher;
        }

        public Task StartMonitoringAsync(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return StarMonitoringAsyncImpl(path);
        }

        protected abstract Task StarMonitoringAsyncImpl(string path);

        public string MonitoredPath => Watcher.Path;
    }
}