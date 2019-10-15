using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using NLog.Fluent;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Wrapper;

namespace RTDS.Monitoring
{
    internal class FolderMonitor : IMonitor
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public event EventHandler<SearchDirectoryArgs> Created;
        private readonly IFileSystemWatcherWrapper _watcher;

        public Task StartMonitoringAsync(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return StarMonitoringAsyncImpl(path);
        }

        public string MonitoredPath => _watcher.Path;

        public FolderMonitor(IFileSystemWatcherWrapper watcher)
        {
            _watcher = watcher;
        }

        private Task StarMonitoringAsyncImpl(string path)
        {
            Task task = new Task(() =>
            {
                _watcher.Path = path;
                _watcher.Created += OnCreated;
                _watcher.NotifyFilters = NotifyFilters.DirectoryName;
                _watcher.EnableRaisingEvents = true;
                Logger.Debug(CultureInfo.CurrentCulture, "Starts folder monitoring (watcher) from path: {0}", path);
            }, TaskCreationOptions.LongRunning);

            task.Start();
            return task;
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            Created?.Invoke(this, new SearchDirectoryArgs(e.FullPath, e.Name));
        }
    }
}