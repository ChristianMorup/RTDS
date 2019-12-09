using System.Threading.Tasks;

namespace RTDS.CBCTDataProvider.ProjectionProcessing
{
    internal interface IProjectionCopier
    {
        Task StartCopyingFiles();
    }
}