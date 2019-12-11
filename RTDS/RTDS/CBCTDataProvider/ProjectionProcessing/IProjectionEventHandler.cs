using System.Threading.Tasks;

namespace RTDS.CBCTDataProvider.ProjectionProcessing
{
    internal interface IProjectionEventHandler
    {
        Task HandleNewFile(string path);
        void OnMonitorFinished();
    }
}