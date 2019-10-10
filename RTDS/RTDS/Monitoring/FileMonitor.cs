using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Wrapper;

namespace RTDS.Monitoring
{
    internal class FileMonitor : IFileMonitor
    {
        private readonly IFileSystemWatcherWrapper _watcher;
        private readonly ITimerWrapper _timer;
        public event EventHandler<SearchDirectoryArgs> Created;
        public event EventHandler<FileMonitorFinishedArgs> Finished;
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

        private async Task StarMonitoringAsyncImpl(string path)
        {
            await StartWatcher(path).ConfigureAwait(false);
            await StartTimer().ConfigureAwait(false);
        }

        private async Task StartWatcher(string path)
        {
            _watcher.Path = path;
            _watcher.Created += async (s, e) => await OnCreated(s, e).ConfigureAwait(false);
            _watcher.NotifyFilters = NotifyFilters.FileName;
            _watcher.EnableRaisingEvents = true;
        }

        private async Task StartTimer()
        {
            _timer.Elapsed += async (sender, args) => await OnTimerExpired(sender, args).ConfigureAwait(false);
            _timer.Interval = 10000; //TODO This should probably be changed
            _timer.Enabled = true;
        }

        private async Task OnTimerExpired(object sender, ElapsedEventArgs e)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
            _timer.Enabled = false;
            _timer.Dispose();

            await new Task(() => Finished?.Invoke(this, new FileMonitorFinishedArgs(this))).ConfigureAwait(true);
        }

        private async Task OnCreated(object source, FileSystemEventArgs e)
        {
            await _timer.Reset().ConfigureAwait(false);
            Created?.Invoke(this, new SearchDirectoryArgs(e.FullPath));
        }
    }
}