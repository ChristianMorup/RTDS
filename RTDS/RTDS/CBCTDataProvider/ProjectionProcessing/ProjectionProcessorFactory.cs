using System.Collections.Concurrent;
using RTDS.CBCTDataProvider.ProjectionProcessing.Wrappers;
using RTDS.DTO;

namespace RTDS.CBCTDataProvider.ProjectionProcessing
{
    internal class ProjectionProcessorFactory : IProjectionProcessorFactory
    {
        public IProjectionProcessor CreateProjectionProcessor(BlockingCollection<ProjectionInfo> queue, PermStorageFolderStructure folderStructure)
        {
            return new ProjectionProcessor(queue, folderStructure, new RTKWrapper());
        }
    }
}