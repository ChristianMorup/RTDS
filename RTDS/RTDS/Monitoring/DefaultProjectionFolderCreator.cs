using System.Threading.Tasks;
using RTDS.DTO;
using RTDS.Utility;

namespace RTDS.Monitoring
{
    internal class DefaultProjectionFolderCreator : IProjectionFolderCreator
    {
        private IFolderCreator _folderCreator;

        public DefaultProjectionFolderCreator(IFolderCreator folderCreator)
        {
            _folderCreator = folderCreator;
        }

        public async Task<ProjectionFolderStructure> CreateFolderStructure()
        {
            return await Task.Run(async () =>
            {
                var structure = await _folderCreator.CreateFolderStructureForProjectionsAsync();
                _folderCreator.CreateFoldersAsync(structure);
                return structure;
            });
        }
    }
}