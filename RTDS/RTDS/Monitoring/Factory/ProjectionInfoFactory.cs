using System;
using System.Collections.Concurrent;
using System.IO;
using RTDS.DTO;

namespace RTDS.Monitoring.Factory
{
    internal class ProjectionInfoFactory : IProjectionFactory
    {
        public ProjectionInfo CreateProjectionInfo(string baseTargetPath, string sourcePath, int index)
        {
            if (string.IsNullOrEmpty(baseTargetPath) || string.IsNullOrEmpty(sourcePath) || index < 0)
            {
                throw new ArgumentException("Input parameters are invalid.");
            }

            var fileName = "proj" + index + ".xim";
            var fullTargetPath = Path.Combine(baseTargetPath, fileName);
            return new ProjectionInfo(index, fullTargetPath, sourcePath, fileName);
        }
    }
}