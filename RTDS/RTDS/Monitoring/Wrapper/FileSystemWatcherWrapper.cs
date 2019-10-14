using System;
using System.IO;

namespace RTDS.Monitoring.Wrapper
{
    internal class FileSystemWatcherWrapper : IFileSystemWatcherWrapper
    {
        public event FileSystemEventHandler Created;
        private readonly FileSystemWatcher _watcher;

        public FileSystemWatcherWrapper(FileSystemWatcher watcher)
        {
            _watcher = watcher ?? throw new ArgumentNullException(nameof(watcher));
            _watcher.Created += OnCreated;
        }

        public string Path
        {
            get => _watcher.Path;
            set => _watcher.Path = value;
        }

        public string Filter
        {
            get => _watcher.Filter;
            set => _watcher.Filter = value;
        }

        public NotifyFilters NotifyFilters
        {
            get => _watcher.NotifyFilter;
            set => _watcher.NotifyFilter = value;
        }

        public bool EnableRaisingEvents
        {
            get => _watcher.EnableRaisingEvents;
            set => _watcher.EnableRaisingEvents = value;
        }

        public void Dispose()
        {
            _watcher?.Dispose();
        }

        private void OnCreated(object source, FileSystemEventArgs e) => Created?.Invoke(source, e);
    }
}