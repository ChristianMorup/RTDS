using RTDS.DTO;

namespace RTDS.CBCTDataProvider.Monitoring.Factory
{
    internal interface IProjectionFactory
    {
        ProjectionInfo CreateProjectionInfo(string baseTargetPath, string sourcePath, int index);
    }
}