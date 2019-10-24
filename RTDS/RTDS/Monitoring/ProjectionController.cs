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
        private readonly IFolderCreator _folderCreator;
        private readonly IFileUtil _fileUtil;
        private ConcurrentQueue<ProjectionInfo> Projections { get; }

        private ConcurrentDictionary<Guid, ConcurrentQueue<ProjectionInfo>> _monitorByProjectionsMap;
        private readonly object _lock;

        public ProjectionController(IFolderCreator folderCreator, IProjectionFactory projectionFactory,
            IFileUtil fileUtil)
        {
            _lock = new object();
            _folderCreator = folderCreator;
            _projectionFactory = projectionFactory;
            _fileUtil = fileUtil;
            Projections = new ConcurrentQueue<ProjectionInfo>();
            _monitorByProjectionsMap = new ConcurrentDictionary<Guid, ConcurrentQueue<ProjectionInfo>>();
        }

        public Task HandleNewFile(MonitorInfo relatedMonitorInfo, string path)
        {
            Task task = new Task(async () =>
            {
                var info = CreateAndAddNewProjectionToCollection(relatedMonitorInfo.RelatedStructure.XimPath,
                    path,
                    relatedMonitorInfo.MonitorGuid);
                var destPath = await _fileUtil.CopyFileAsync(info.TempStoragePath, info.PermanentStoragePath);

                Logger.Info(CultureInfo.CurrentCulture, info.Name + " has been moved");
            });

            task.Start();
            return task;
        }

        public async Task<ProjectionFolderStructure> CreateProjectionFolderStructure()
        {
            return await Task.Run(async () =>
            {
                var structure = await _folderCreator.CreateFolderStructureForProjectionsAsync();
                _folderCreator.CreateFoldersAsync(structure);
                return structure;
            });
        }

        private ProjectionInfo CreateAndAddNewProjectionToCollection(string baseTargetPath, string sourcePath,
            Guid monitorGuid)
        {
            lock (_lock)
            {
                ConcurrentQueue<ProjectionInfo> projections;
                if (_monitorByProjectionsMap.TryGetValue(monitorGuid, out projections))
                {
                    var fileIndex = projections.Count;
                    var info = _projectionFactory.CreateProjectionInfo(baseTargetPath, sourcePath, fileIndex);
                    projections.Enqueue(info);
                    return info;
                }
                else
                {
                    projections = new ConcurrentQueue<ProjectionInfo>();
                    var info = _projectionFactory.CreateProjectionInfo(baseTargetPath, sourcePath, 0);
                    projections.Enqueue(info);
                    AddMonitorAndQueueToDictionary(monitorGuid, projections);
                    return info;
                }
            }
        }

        private void AddMonitorAndQueueToDictionary(Guid monitorGuid, ConcurrentQueue<ProjectionInfo> projections)
        {
            if (!_monitorByProjectionsMap.TryAdd(monitorGuid, projections))
            {
                AddMonitorAndQueueToDictionary(monitorGuid, projections);
            }
        }
    }
}