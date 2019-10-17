using System.Threading.Tasks;

namespace RTDS.Monitoring
{
    internal interface IFolderCreator
    {
        Task<ProjectionFolderStructure> CreateFolderStructureForProjectionsAsync();

        Task CreateFoldersAsync(ProjectionFolderStructure structure);
    }
}