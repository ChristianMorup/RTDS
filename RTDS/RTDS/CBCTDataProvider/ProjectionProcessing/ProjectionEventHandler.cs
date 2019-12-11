using System.Collections.Concurrent;
using System.Threading.Tasks;
using RTDS.CBCTDataProvider.Monitoring.Factory;
using RTDS.DTO;

namespace RTDS.CBCTDataProvider.ProjectionProcessing
{
    internal class ProjectionEventHandler : IProjectionEventHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IProjectionInfoFactory _projectionInfoFactory;
        private int _currentFileIndex;
        private readonly BlockingCollection<TempProjectionInfo> _queue;
        private readonly object _lock;
        private readonly PermStorageFolderStructure _folderStructure;

        public ProjectionEventHandler(IProjectionInfoFactory projectionInfoFactory,
            PermStorageFolderStructure relatedFolderStructure, BlockingCollection<TempProjectionInfo> queue)
        {
            _projectionInfoFactory = projectionInfoFactory;
            _lock = new object();
            _currentFileIndex = 0;
            _folderStructure = relatedFolderStructure;
            _queue = queue;
        }

        public Task HandleNewFile(string path)
        {
            return Task.Run(async () =>
            {
                var info = CreateProjectionInfo(path);
                Logger.Info("Handling proj: " + info.FilePath);
                _queue.TryAdd(info);
            });
        }

        public void OnMonitorFinished()
        {
            _queue.CompleteAdding();
        }

        private TempProjectionInfo CreateProjectionInfo(string path)
        {
            lock (_lock)
            {
                var info = _projectionInfoFactory.CreateTempProjectionInfo(path, _currentFileIndex);
                _currentFileIndex++;
                return info;
            }
        }
    }
}