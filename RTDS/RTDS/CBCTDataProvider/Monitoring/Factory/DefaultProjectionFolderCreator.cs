using System.Threading.Tasks;
using RTDS.DTO;
using RTDS.ExceptionHandling;
using RTDS.Utility;

namespace RTDS.CBCTDataProvider.Monitoring.Factory
{
    internal class DefaultProjectionFolderCreator : IProjectionFolderCreator
    {
        private readonly IFolderCreator _folderCreator;

        public DefaultProjectionFolderCreator(IFolderCreator folderCreator)
        {
            _folderCreator = folderCreator;
        }

        public async Task<PermStorageFolderStructure> CreateFolderStructure()
        {
            return await Task.Run(async () =>
            {
                var structure = await _folderCreator.CreateFolderStructureForProjectionsAsync();
                TaskWatcher.AddTask(_folderCreator.CreateFoldersAsync(structure));
                return structure;
            });
        }
    }
}