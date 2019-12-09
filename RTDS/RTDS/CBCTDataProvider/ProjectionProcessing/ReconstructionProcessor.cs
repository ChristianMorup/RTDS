using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using RTDS.CBCTDataProvider.ProjectionProcessing.Wrappers;
using RTDS.DTO;
using RTDS.ExceptionHandling;

namespace RTDS.CBCTDataProvider.ProjectionProcessing
{
    internal class ReconstructionProcessor : IReconstructionProcessor
    {
        public event EventHandler<string> ImageReconstructed;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly BlockingCollection<PermProjectionInfo> _projectionInfos;
        private readonly string _targetPath;
        private readonly IRTKWrapper _rtkWrapper;
        private readonly object _lock = new object();
        private bool _hasFailed = false;

        public ReconstructionProcessor(BlockingCollection<PermProjectionInfo> projectionInfos,
            PermStorageFolderStructure folderStructure, IRTKWrapper rtkWrapper)
        {
            _projectionInfos = projectionInfos;
            _rtkWrapper = rtkWrapper;
            _targetPath = folderStructure.MhaPath + "ReconstructedImage.mha";
        }

        public Task StartConsumingProjections()
        {
            Task task = new Task(() =>
            {
                try
                {
                    int numberOfProjections = 0;
                    List<PermProjectionInfo> infos = new List<PermProjectionInfo>();
                    foreach (PermProjectionInfo info in _projectionInfos.GetConsumingEnumerable())
                    {
                        if (numberOfProjections == 0)
                        {
                            infos = new List<PermProjectionInfo>();
                        }

                        infos.Add(info);
                        numberOfProjections++;

                        if (numberOfProjections == 3)
                        {
                            PerformReconstruction(infos);
                        }
                    }
                }
                catch (Exception e)
                {
                    _hasFailed = true;
                    Logger.Fatal("Stops consuming projections due to: " + e.Message);
                    throw;
                }
                finally
                {
                    if (!_hasFailed)
                    {
                        ImageReconstructed?.Invoke(this, _targetPath);
                    }
                }
            }, TaskCreationOptions.LongRunning);
            task.Start();
            return task;
        }

        private void PerformReconstruction(List<PermProjectionInfo> infos)
        {
            TaskWatcher.AddTask(Task.Run(() =>
            {
                lock (_lock)
                {
                    try
                    {
                        _rtkWrapper.PerformReconstruction(infos[0].FilePath, infos[1].FilePath,
                            infos[2].FilePath, _targetPath);
                    }
                    catch (Exception e)
                    {
                        Logger.Fatal(e);
                        throw;
                    }

                }
            }));
        }
    }
}