using System;
using System.IO;
using System.Threading.Tasks;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Wrapper;

namespace RTDS.Monitoring
{
    internal class FolderMonitor : IMonitor
    {
        private readonly IFileSystemWatcherWrapper _watcher;
        public event EventHandler<SearchDirectoryArgs> Created;

        public Task StartMonitoringAsync(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return StarMonitoringAsyncImpl(path);
        }


        public FolderMonitor(IFileSystemWatcherWrapper watcher)
        {
            _watcher = watcher;
        }

        private async Task StarMonitoringAsyncImpl(string path)
        {
            _watcher.Path = path;
            _watcher.Created += OnCreated;
            _watcher.NotifyFilters = NotifyFilters.DirectoryName;
            _watcher.EnableRaisingEvents = true;
        }

        //TODO Ændrer events til Async await syntax hvis muligt. 
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            Created?.Invoke(this, new SearchDirectoryArgs(e.FullPath));
        }
    }
}
