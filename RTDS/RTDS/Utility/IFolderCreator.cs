using System.Threading.Tasks;
using RTDS.DTO;
using RTDS.Monitoring;

namespace RTDS.Utility
{
    internal interface IFolderCreator
    {
        Task<ProjectionFolderStructure> CreateFolderStructureForProjectionsAsync();

        Task CreateFoldersAsync(ProjectionFolderStructure structure);
    }
}