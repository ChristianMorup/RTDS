using RTDS.Monitoring.Args;
using RTDS.Monitoring.Wrapper;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;

namespace RTDS.Monitoring
{
    internal class FileMonitor : IFileMonitor
    {
        public event EventHandler<SearchDirectoryArgs> Created;
        public event EventHandler<FileMonitorFinishedArgs> Finished;
        private readonly IFileSystemWatcherWrapper _watcher;
        private readonly ITimerWrapper _timer;

        public FileMonitor(IFileSystemWatcherWrapper watcher, ITimerWrapper timer)
        {
            _watcher = watcher;
            _timer = timer;
        }

        public Task StartMonitoringAsync(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return StarMonitoringAsyncImpl(path);
        }

        private Task StarMonitoringAsyncImpl(string path)
        {
            Task task = new Task(() =>
            {
                StartWatcher(path);
                StartTimer();
            }, TaskCreationOptions.LongRunning);

            task.Start();
            return task;
        }

        private void StartWatcher(string path)
        {
            _watcher.Path = path;
            _watcher.Created += OnCreated;
            _watcher.NotifyFilters = NotifyFilters.FileName;
            _watcher.EnableRaisingEvents = true;
        }

        private void StartTimer()
        {
            _timer.Elapsed += OnTimerExpired;
            _timer.Interval = 10000; //TODO This should probably be changed
            _timer.Enabled = true;
        }

        private void OnTimerExpired(object sender, ElapsedEventArgs e)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
            _timer.Enabled = false;
            _timer.Dispose();

            Finished?.Invoke(this, new FileMonitorFinishedArgs(this));
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            _timer.Reset();
            Created?.Invoke(this, new SearchDirectoryArgs(e.FullPath, e.Name));
        }
    }
}