using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NLog.Fluent;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Wrapper;

namespace RTDS.Monitoring
{
    internal class FolderMonitor : AbstractMonitor, IMonitor
    {
        public override event EventHandler<SearchDirectoryArgs> Created;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public FolderMonitor(IFileSystemWatcherWrapper watcher) : base(watcher)
        {

        }

        protected override Task StarMonitoringAsyncImpl(string path)
        {
            Logger.Info(CultureInfo.CurrentCulture, "Starts folder monitoring at path: {0}", path);

            Task task = new Task(() =>
            {
                _watcher.Path = path;
                _watcher.Created += OnCreated;
                _watcher.NotifyFilters = NotifyFilters.DirectoryName;
                _watcher.EnableRaisingEvents = true;
                Logger.Debug(CultureInfo.CurrentCulture, "Folder watcher enabled at path: {0}", path);
            }, TaskCreationOptions.LongRunning);

            task.Start();
            return task;
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            Created?.Invoke(this, new SearchDirectoryArgs(e.FullPath, e.Name, this));
        }
    }
}