using System.Collections.Concurrent;
using RTDS.CBCTDataProvider.Monitoring.Monitors;
using RTDS.DTO;

namespace RTDS.CBCTDataProvider.ProjectionProcessing.Factory
{
    internal interface IProjectionPipelineFactory
    {
        IReconstructionProcessor CreateReconstructionProcessor(BlockingCollection<PermProjectionInfo> queue,
            PermStorageFolderStructure folderStructure);

        IProjectionCopier CreateProjectionCopier(BlockingCollection<TempProjectionInfo> queueIn,
            BlockingCollection<PermProjectionInfo> queueOut, PermStorageFolderStructure folderStructure);

        ISubfolderMonitorListener CreateFileMonitorListener(PermStorageFolderStructure structure,
            BlockingCollection<TempProjectionInfo> queue);
    }
}