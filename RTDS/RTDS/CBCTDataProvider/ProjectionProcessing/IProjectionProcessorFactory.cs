using System.Collections.Concurrent;
using RTDS.DTO;

namespace RTDS.CBCTDataProvider.ProjectionProcessing
{
    internal interface IProjectionProcessorFactory
    {
        IProjectionProcessor CreateProjectionProcessor(BlockingCollection<ProjectionInfo> queue,
            PermStorageFolderStructure folderStructure);
    }
}