using System;
using System.IO;

namespace RTDS.Monitoring.Wrapper
{

    internal class FileSystemWatcherWrapper : IFileSystemWatcherWrapper
    {
        private readonly FileSystemWatcher _watcher;
        public event FileSystemEventHandler Created;

        public string Path
        {
            get => _watcher.Path;
            set => _watcher.Path = value;
        }

        public string Filter { get; set; }

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

        public FileSystemWatcherWrapper(FileSystemWatcher watcher)
        {
            _watcher = watcher ?? throw new ArgumentNullException(nameof(watcher));
            _watcher.Created += OnCreated;
        }

        private void OnCreated(object source, FileSystemEventArgs e) => Created?.Invoke(source, e);

        public void Dispose()
        {
            _watcher?.Dispose();
        }
    }
}