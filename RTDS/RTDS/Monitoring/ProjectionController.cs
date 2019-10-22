using System;
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

        public ProjectionController(IFolderCreator folderCreator, IProjectionFactory projectionFactory, IFileUtil fileUtil)
        {
            _folderCreator = folderCreator;
            _projectionFactory = projectionFactory;
            _fileUtil = fileUtil;
        }


        public Task HandleNewFile(IMonitor relatedMonitor, string path, Dictionary<Guid, ProjectionInfo> monitorGuidByQueueMap)
        {
            return Task.Run(async () =>
            {
                ProjectionInfo info;
                if (monitorGuidByQueueMap.TryGetValue(relatedMonitor.Guid, out info))
                {
                    var fileName = Path.GetFileName(path);
                    var destinationFile = Path.Combine(info.Structure.XimPath, fileName);
                    var destPath = await _fileUtil.CopyFileAsync(path, destinationFile);
                    Logger.Info(CultureInfo.CurrentCulture, fileName + " has been moved");
                }
            });
        }

        public async Task<ProjectionInfo> CreateProjectionInfo()
        {
            var folderStructure = await Task.Run(async () =>
            {
                var structure = await _folderCreator.CreateFolderStructureForProjectionsAsync();
                _folderCreator.CreateFoldersAsync(structure);
                return structure;
            });

            return _projectionFactory.CreateProjectionInfo(folderStructure);
        }
    }
}