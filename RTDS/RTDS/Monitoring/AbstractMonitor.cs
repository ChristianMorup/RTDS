using System;
using System.Threading.Tasks;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Wrapper;

namespace RTDS.Monitoring
{
    internal abstract class AbstractMonitor : IMonitor
    {
        public abstract event EventHandler<SearchDirectoryArgs> Created;
        protected readonly IFileSystemWatcherWrapper _watcher;

        protected AbstractMonitor(IFileSystemWatcherWrapper watcher)
        {
            _watcher = watcher;
            Guid = Guid.NewGuid();
        }

        public Guid Guid { get; }

        public Task StartMonitoringAsync(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return StarMonitoringAsyncImpl(path);
        }

        protected abstract Task StarMonitoringAsyncImpl(string path);

        public string MonitoredPath => _watcher.Path;
    }
}