using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Wrappers;

namespace RTDS.Monitoring.Monitors
{
    internal class FileMonitor : AbstractMonitor, IFileMonitor
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public override event EventHandler<SearchDirectoryArgs> Created;
        public event EventHandler<FileMonitorFinishedArgs> Finished;
        private readonly ITimerWrapper _timer;

        public FileMonitor(IFileSystemWatcherWrapper watcher, ITimerWrapper timer) : base(watcher)
        {
            _timer = timer;
        }

        protected override Task StarMonitoringAsyncImpl(string path)
        {
            Logger.Info(CultureInfo.CurrentCulture, "Starts file monitoring at path: {0}", path);

            Task task = new Task(() =>
            {
                StartWatcher(path);
                StartTimer();
                Logger.Debug(CultureInfo.CurrentCulture, "File watcher enabled at path: {0}", path);
            }, TaskCreationOptions.LongRunning);

            task.Start();
            return task;
        }

        private void StartWatcher(string path)
        {
            Watcher.Path = path;
            Watcher.Created += OnCreated;
            Watcher.NotifyFilters = NotifyFilters.FileName;
            Watcher.EnableRaisingEvents = true;
        }

        private void StartTimer()
        {
            _timer.Elapsed += OnTimerExpired;
            _timer.Interval = Configuration.ConfigurationManager.GetMonitorSettings().TimeOutThreshold;
            _timer.Enabled = true;
        }

        private void OnTimerExpired(object sender, ElapsedEventArgs e)
        {
            Watcher.EnableRaisingEvents = false;
            Watcher.Dispose();
            _timer.Enabled = false;
            _timer.Dispose();

            Finished?.Invoke(this, new FileMonitorFinishedArgs(this));
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            _timer.Reset();
            Created?.Invoke(this, new SearchDirectoryArgs(e.FullPath, e.Name, this));
        }
    }
}