﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDS.Monitoring
{
    public class FileSystemWatcherWrapper : IFileSystemWatcherWrapper
    {
        private readonly FileSystemWatcher _watcher;
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

        public event FileSystemEventHandler Changed;

        public bool EnableRaisingEvents
        {
            get => _watcher.EnableRaisingEvents;
            set => _watcher.EnableRaisingEvents = value;
        }

        public FileSystemWatcherWrapper(FileSystemWatcher watcher)
        {
            _watcher = watcher ?? throw new ArgumentNullException(nameof(watcher));
            _watcher.Changed += Changed;
        }

        public void Dispose()
        {
            _watcher?.Dispose();
        }
    }
}
