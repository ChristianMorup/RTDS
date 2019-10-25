using System.Threading.Tasks;
using RTDS.DTO;

namespace RTDS.Monitoring
{
    internal interface IProjectionFolderCreator
    {
        Task<ProjectionFolderStructure> CreateFolderStructure();
    }
}