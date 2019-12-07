using System.Threading.Tasks;

namespace RTDS.CBCTDataProvider.ProjectionProcessing
{
    internal interface IProjectionHandler
    {
        Task HandleNewFile(string path);
    }
}