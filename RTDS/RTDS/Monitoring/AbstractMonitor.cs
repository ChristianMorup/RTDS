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

        public override int GetHashCode()
        {
            //Inspiration: https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-overriding-gethashcode
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Guid.GetHashCode();
                hash = hash * 23 + _watcher.GetHashCode();
                hash = hash * 23 + MonitoredPath.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            var myObject = obj as AbstractMonitor;

            if (myObject != null)
            {
                return Equals((AbstractMonitor)obj);
            }
            else
            {
                return false;
            }
        }

        private bool Equals(AbstractMonitor monitor)
        {
            return monitor.Guid.Equals(Guid);
        }
    }
}