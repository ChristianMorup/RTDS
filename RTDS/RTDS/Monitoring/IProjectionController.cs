using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RTDS.DTO;
using RTDS.Monitoring.Monitors;

namespace RTDS.Monitoring
{
    internal interface IProjectionController
    {
        Task HandleNewFile(IMonitor relatedMonitor, string path,
            Dictionary<Guid, ProjectionInfo> monitorGuidByQueueMap);

        Task<ProjectionInfo> CreateProjectionInfo();
    }
}