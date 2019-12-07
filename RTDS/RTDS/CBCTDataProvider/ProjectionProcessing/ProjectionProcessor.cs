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
    internal class ProjectionProcessor : IProjectionProcessor
    {
        public event EventHandler<string> ImageReconstructed;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly BlockingCollection<ProjectionInfo> _projectionInfos;
        private readonly string _targetPath;
        private readonly IRTKWrapper _rtkWrapper;
        private readonly object _lock = new object();
        private bool _hasFailed = false;

        public ProjectionProcessor(BlockingCollection<ProjectionInfo> projectionInfos,
            PermStorageFolderStructure folderStructure, IRTKWrapper rtkWrapper)
        {
            _projectionInfos = projectionInfos;
            _rtkWrapper = rtkWrapper;
            _targetPath = folderStructure.MhaPath + "ReconstructedImage.mha";
        }

        public void StartConsumingProjections()
        {
            TaskWatcher.AddTask(Task.Run(() =>
            {
                try
                {
                    int numberOfProjections = 0;
                    List<ProjectionInfo> infos = new List<ProjectionInfo>();
                    foreach (ProjectionInfo info in _projectionInfos.GetConsumingEnumerable())
                    {
                        if (numberOfProjections == 0)
                        {
                            infos = new List<ProjectionInfo>();
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
            }));
        }

        private void PerformReconstruction(List<ProjectionInfo> infos)
        {
            TaskWatcher.AddTask(Task.Run(() =>
            {
                lock (_lock)
                {
                    try
                    {
                        _rtkWrapper.PerformReconstruction(infos[0].TempStoragePath, infos[1].TempStoragePath,
                            infos[2].TempStoragePath, _targetPath);
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