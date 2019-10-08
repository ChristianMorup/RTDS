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
            using (_systemWatcher)
            {
                _systemWatcher.Path = path;
                _systemWatcher.Changed += OnCreated;
                _systemWatcher.NotifyFilters = NotifyFilters.LastAccess
                                       | NotifyFilters.LastWrite
                                       | NotifyFilters.FileName
                                       | NotifyFilters.DirectoryName;
                //await SetFilters();
                _systemWatcher.EnableRaisingEvents = true;
            }
        }

        private async Task SetFilters()
        {
            _systemWatcher.NotifyFilters = NotifyFilters.DirectoryName;
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            //TODO Add logging
            FolderCreated?.Invoke(this, e);
        }
    }
}
