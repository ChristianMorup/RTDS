using RTDS.DTO;

namespace RTDS.Monitoring.Factory
{
    internal interface IProjectionFactory
    {
        ProjectionInfo CreateProjectionInfo(ProjectionFolderStructure structure);
    }
}