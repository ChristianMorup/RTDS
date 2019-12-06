using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using RTDS.DTO;
using ConfigurationManager = RTDS.Configuration.ConfigurationManager;

namespace RTDS.Utility
{
    internal class FolderCreator : IFolderCreator
    {
        private readonly IFileUtil _fileUtil;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public FolderCreator(IFileUtil fileUtil)
        {
            _fileUtil = fileUtil;
        }

        public Task<PermStorageFolderStructure> CreateFolderStructureForProjectionsAsync()
        {
            return Task.Run(() =>
            {
                var basePath = CreateBasePath();
                var ximPath = CreateXimFolderPath(basePath);
                var mhaPath = CreateMhaFolderPath(basePath);
                var ctPath = CreateCtFolderPath(basePath);

                return new PermStorageFolderStructure(basePath, ximPath, mhaPath, ctPath);
            });
        }

        public Task CreateFoldersAsync(PermStorageFolderStructure structure)
        {
            if (structure == null) throw new ArgumentNullException(nameof(structure));
            return Task.Run(async () =>
            {
                Logger.Info("Creates target folders.");
                await _fileUtil.CreateFolderAsync(structure.BasePath);
                TaskWatcher.AddTask(_fileUtil.CreateFolderAsync(structure.XimPath));
                TaskWatcher.AddTask(_fileUtil.CreateFolderAsync(structure.MhaPath));
                TaskWatcher.AddTask(_fileUtil.CreateFolderAsync(structure.CtPath));
            });
        }

        private string CreateBasePath()
        {
            string basePath = ConfigurationManager.GetConfigurationPaths().BaseTargetPath;
            return Path.Combine(basePath, CreateBaseFolderName());
        }

        private string CreateBaseFolderName()
        {
            DateTime timeStamp = DateTime.Now;
            return timeStamp.ToString("dd/MM/yyyy-hh-mm-ss", CultureInfo.CurrentCulture);
        }

        private string CreateXimFolderPath(string basePath)
        {
            return Path.Combine(basePath, "xim");
        }

        private string CreateMhaFolderPath(string basePath)
        {
            return Path.Combine(basePath, "mha");
        }

        private string CreateCtFolderPath(string basePath)
        {
            return Path.Combine(basePath, "CT");
        }
    }
}