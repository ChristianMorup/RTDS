using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;
using RTDS.DTO;
using RTDS.Monitoring.Factory;
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
        private readonly PermStorageFolderStructure _folderStructure;

        public ProjectionController(IProjectionFactory projectionFactory, IFileUtil fileUtil, PermStorageFolderStructure relatedFolderStructure)
        {
            _projectionFactory = projectionFactory;
            _fileUtil = fileUtil;
            _queue = new ConcurrentQueue<ProjectionInfo>();
            _lock = new object();
            _currentFileIndex = 0;
            _folderStructure = relatedFolderStructure;
        }

        public Task HandleNewFile(string path)
        {
            Task task = new Task(async () =>
            {
                var info = CreateProjectionInfo(path);
                var destPath = await _fileUtil.CopyFileAsync(info.TempStoragePath, info.PermanentStoragePath);
                _queue.Enqueue(info);

                Logger.Info(CultureInfo.CurrentCulture, info.FileName + " has been moved to " + destPath);
            });

            task.Start();
            return task;
        }

        public ConcurrentQueue<ProjectionInfo> GetQueue()
        {
            return _queue;
        }

        private ProjectionInfo CreateProjectionInfo(string path)
        {
            lock (_lock)
            {
                var info = _projectionFactory.CreateProjectionInfo(_folderStructure.XimPath,
                    path, _currentFileIndex);
                _currentFileIndex++;
                return info;
            }
        }
    }
}