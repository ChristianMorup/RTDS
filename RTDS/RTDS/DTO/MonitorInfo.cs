using System;
using RTDS.Monitoring.Monitors;

namespace RTDS.DTO
{
    internal class MonitorInfo
    {
        public MonitorInfo(ProjectionFolderStructure relatedStructure, IMonitor relatedMonitor, Guid monitorGuid)
        {
            RelatedStructure = relatedStructure;
            RelatedMonitor = relatedMonitor;
            MonitorGuid = monitorGuid;
        }

        public ProjectionFolderStructure RelatedStructure { get; }

        public IMonitor RelatedMonitor { get; }

        public Guid MonitorGuid { get; }
    }
}