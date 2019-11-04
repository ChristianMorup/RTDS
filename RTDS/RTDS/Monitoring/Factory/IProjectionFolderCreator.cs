using System.Threading.Tasks;
using RTDS.DTO;

namespace RTDS.Monitoring.Factory
{
    internal interface IProjectionFolderCreator
    {
        Task<PermStorageFolderStructure> CreateFolderStructure();
    }
}