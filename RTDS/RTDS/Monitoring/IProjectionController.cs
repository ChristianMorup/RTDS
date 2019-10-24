using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RTDS.DTO;
using RTDS.Monitoring.Monitors;

namespace RTDS.Monitoring
{
    internal interface IProjectionController
    {
        Task HandleNewFile(MonitorInfo relatedMonitorInfo, string path);

        Task<ProjectionFolderStructure> CreateProjectionFolderStructure();
    }
}