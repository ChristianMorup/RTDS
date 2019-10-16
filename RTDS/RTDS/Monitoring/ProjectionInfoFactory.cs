using System;
using System.Collections.Concurrent;

namespace RTDS.Monitoring
{
    internal class ProjectionInfoFactory : IProjectionFactory
    {
        public ProjectionInfo CreateProjectionInfo(string permanentStoragePath)
        {
            return new ProjectionInfo(permanentStoragePath, new BlockingCollection<string>(),
                new BlockingCollection<string>());
        }

        private string CreateXimFolderName()
        {
            return "xim projections";
        }
    }
}