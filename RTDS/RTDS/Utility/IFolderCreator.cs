using System.Threading.Tasks;
using RTDS.DTO;

namespace RTDS.Utility
{
    internal interface IFolderCreator
    {
        Task<ProjectionFolderStructure> CreateFolderStructureForProjectionsAsync();

        Task CreateFoldersAsync(ProjectionFolderStructure structure);
    }
}