using System.Collections.Concurrent;
using System.IO;
using RTDS.DTO;

namespace RTDS.Monitoring.Factory
{
    internal class ProjectionInfoFactory : IProjectionFactory
    {
        public ProjectionInfo CreateProjectionInfo(string baseTargetPath, string sourcePath, int index)
        {
            var fileName = "proj" + index + ".xim";
            var fullTargetPath = Path.Combine(baseTargetPath, fileName);
            return new ProjectionInfo(index, fullTargetPath, sourcePath, fileName);
        }
    }
}