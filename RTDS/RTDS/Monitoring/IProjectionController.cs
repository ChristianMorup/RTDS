using System.Collections.Concurrent;
using System.Threading.Tasks;
using RTDS.DTO;
using RTDS.Monitoring.Monitors;

namespace RTDS.Monitoring
{
    internal interface IProjectionController
    {
        Task HandleNewFile(string path);

        ConcurrentQueue<ProjectionInfo> GetQueue();
    }
}