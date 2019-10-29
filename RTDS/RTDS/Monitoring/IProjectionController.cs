using System.Collections.Concurrent;
using System.Threading.Tasks;
using RTDS.DTO;

namespace RTDS.Monitoring
{
    internal interface IProjectionController
    {
        Task HandleNewFile(MonitorInfo relatedMonitorInfo, string path);

        ConcurrentQueue<ProjectionInfo> GetQueue();
    }
}