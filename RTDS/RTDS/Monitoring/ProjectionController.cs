using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using RTDS.DTO;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Monitors;
using RTDS.Utility;

namespace RTDS.Monitoring
{
    internal class ProjectionController : IProjectionController
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IProjectionFactory _projectionFactory;
        private readonly IFileUtil _fileUtil;
        private int _currentFileIndex;
        private ConcurrentQueue<ProjectionInfo> _queue;
        private readonly object _lock;

        public ProjectionController(IProjectionFactory projectionFactory, IFileUtil fileUtil)
        {
            _projectionFactory = projectionFactory;
            _fileUtil = fileUtil;
            _queue = new ConcurrentQueue<ProjectionInfo>();
            _lock = new object();
            _currentFileIndex = 0;
        }

        public Task HandleNewFile(MonitorInfo relatedMonitorInfo, string path)
        {
            Task task = new Task(async () =>
            {
                var info = CreateProjectionInfo(relatedMonitorInfo, path);
                var destPath = await _fileUtil.CopyFileAsync(info.TempStoragePath, info.PermanentStoragePath);
                _queue.Enqueue(info);

                Logger.Info(CultureInfo.CurrentCulture, info.Name + " has been moved to " + destPath);
            });

            task.Start();
            return task;
        }

        private ProjectionInfo CreateProjectionInfo(MonitorInfo relatedMonitorInfo, string path)
        {
            lock (_lock)
            {
                var info = _projectionFactory.CreateProjectionInfo(relatedMonitorInfo.RelatedStructure.XimPath,
                    path, _currentFileIndex);
                _currentFileIndex++;
                return info;
            }
        }
    }
}