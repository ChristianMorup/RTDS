using System.Collections.Concurrent;
using RTDS.DTO;

namespace RTDS.Monitoring.Factory
{
    internal class ProjectionInfoFactory : IProjectionFactory
    {
        public ProjectionInfo CreateProjectionInfo(ProjectionFolderStructure structure)
        {
            return new ProjectionInfo(structure);
        }
    }
}