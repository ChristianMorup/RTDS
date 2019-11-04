using System.IO;
using System.Timers;
using RTDS.DTO;
using RTDS.Monitoring.Monitors;
using RTDS.Monitoring.Wrappers;
using RTDS.Utility;

namespace RTDS.Monitoring.Factory
{
    internal class MonitorFactory : IMonitorFactory
    {
        public IFileMonitor CreateFileMonitor()
        {
            var timer = new TimerWrapper(new Timer());
            var watcher = new FileSystemWatcherWrapper(new FileSystemWatcher());
            return new FileMonitor(watcher, timer);
        }

        public IMonitor CreateFolderMonitor()
        {
            var watcher = new FileSystemWatcherWrapper(new FileSystemWatcher());
            return new FolderMonitor(watcher);
        }

        public IFileMonitorListener CreateFileMonitorListener(PermStorageFolderStructure structure)
        {
            var factory = new ProjectionInfoFactory();
            var fileUtil = new FileUtil();

            var projectionController = new ProjectionController(factory, fileUtil, structure);

            return new FileMonitorListener(projectionController);
        }
    }
}