using System;
using System.IO;
using System.Threading.Tasks;

namespace RTDS.Monitoring
{
    public class FolderMonitor
    {
        private IFileSystemWatcherWrapper _systemWatcher;

        public event EventHandler FolderCreated; 
        public FolderMonitor(IFileSystemWatcherWrapper systemWatcher)
        {
            _systemWatcher = systemWatcher;
        }

        public Task StarMonitoringAsync(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return StarMonitoringAsyncImpl(path);
        }

        private async Task StarMonitoringAsyncImpl(string path)
        {
            _systemWatcher.Path = path;
            _systemWatcher.Created += OnCreated;
            _systemWatcher.NotifyFilters = NotifyFilters.DirectoryName;
            _systemWatcher.EnableRaisingEvents = true;
        }

        //TODO Ændrer events til Async await syntax hvis muligt. 
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            FolderCreated?.Invoke(this, e);
        }
    }
}
