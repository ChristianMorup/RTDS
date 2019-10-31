using System.Threading.Tasks;
using RTDS.DTO;

namespace RTDS.Utility
{
    internal interface IFolderCreator
    {
        Task<PermStorageFolderStructure> CreateFolderStructureForProjectionsAsync();

        Task CreateFoldersAsync(PermStorageFolderStructure structure);
    }
}