using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Wrappers;

namespace RTDS.Monitoring.Monitors
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
            Task task = new Task(() =>
            {
                Watcher.Path = path;
                Watcher.Created += OnCreated;
                Watcher.NotifyFilters = NotifyFilters.DirectoryName;
                Watcher.EnableRaisingEvents = true;
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