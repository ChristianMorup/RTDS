using System.Threading.Tasks;
using RTDS.DTO;

namespace RTDS.CBCTDataProvider.Monitoring.Factory
{
    internal interface IProjectionFolderCreator
    {
        Task<PermStorageFolderStructure> CreateFolderStructure();
    }
}