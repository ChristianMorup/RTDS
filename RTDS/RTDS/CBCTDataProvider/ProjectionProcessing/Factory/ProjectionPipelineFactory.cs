using System.Collections.Concurrent;
using RTDS.CBCTDataProvider.Monitoring.Factory;
using RTDS.CBCTDataProvider.Monitoring.Monitors;
using RTDS.CBCTDataProvider.ProjectionProcessing.Wrappers;
using RTDS.DTO;
using RTDS.Utility;

namespace RTDS.CBCTDataProvider.ProjectionProcessing.Factory
{
    internal class ProjectionPipelineFactory : IProjectionPipelineFactory
    {
        public IReconstructionProcessor CreateReconstructionProcessor(BlockingCollection<PermProjectionInfo> queue, PermStorageFolderStructure folderStructure)
        {
            return new ReconstructionProcessor(queue, folderStructure, new RTKWrapper());
        }

        public IProjectionCopier CreateProjectionCopier(BlockingCollection<TempProjectionInfo> queueIn,
            BlockingCollection<PermProjectionInfo> queueOut, PermStorageFolderStructure folderStructure)
        {
            return new ProjectionCopier(new ProjectionInfoInfoFactory(), new FileUtil(), queueIn, queueOut, folderStructure);
        }

        public ISubfolderMonitorListener CreateFileMonitorListener(PermStorageFolderStructure structure, BlockingCollection<TempProjectionInfo> queue)
        {
            var factory = new ProjectionInfoInfoFactory();
            var projectionController = new ProjectionEventHandler(factory, structure, queue);

            return new SubfolderMonitorListener(projectionController);
        }
    }
}