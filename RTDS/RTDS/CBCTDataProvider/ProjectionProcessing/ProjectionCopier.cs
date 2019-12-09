using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;
using RTDS.CBCTDataProvider.Monitoring.Factory;
using RTDS.DTO;
using RTDS.ExceptionHandling;
using RTDS.Utility;

namespace RTDS.CBCTDataProvider.ProjectionProcessing
{
    internal class ProjectionCopier : IProjectionCopier
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IProjectionInfoFactory _projectionInfoFactory;
        private readonly IFileUtil _fileUtil;
        private readonly BlockingCollection<TempProjectionInfo> _inQueue;
        private readonly BlockingCollection<PermProjectionInfo> _outQueue;
        private readonly PermStorageFolderStructure _folderStructure;

        public ProjectionCopier(IProjectionInfoFactory projectionInfoFactory, IFileUtil fileUtil, BlockingCollection<TempProjectionInfo> inQueue,
            BlockingCollection<PermProjectionInfo> outQueue, PermStorageFolderStructure folderStructure)
        {
            _projectionInfoFactory = projectionInfoFactory;
            _fileUtil = fileUtil;
            _inQueue = inQueue;
            _outQueue = outQueue;
            _folderStructure = folderStructure;
        }

        public Task StartCopyingFiles()
        {
            Task task = new Task(async () =>
            {
                bool hasFailed = false;
                try
                {
                    foreach (TempProjectionInfo tempInfo in _inQueue.GetConsumingEnumerable())
                    {
                        var permInfo = _projectionInfoFactory.CreatePermProjectionInfo(_folderStructure, tempInfo);
                        
                        var destPath = await _fileUtil.CopyFileAsync(tempInfo.FilePath, permInfo.FilePath);
                        _outQueue.TryAdd(permInfo);
                        Logger.Info(CultureInfo.CurrentCulture, permInfo.FileName + " has been moved to " + destPath);
                    }
                }
                catch (Exception e)
                {
                    hasFailed = true;
                    Logger.Fatal("Stops consuming projections due to: " + e.Message);
                    throw;
                }
                finally
                {
                    if (!hasFailed)
                    {
                        _outQueue.CompleteAdding();
                    }
                }
            }, TaskCreationOptions.LongRunning);
            task.Start();
            return task;
        }
    }
}