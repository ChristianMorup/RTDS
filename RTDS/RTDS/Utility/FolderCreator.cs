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

        public Task<ProjectionFolderStructure> CreateFolderStructureForProjectionsAsync()
        {
            return Task.Run(() =>
            {
                var basePath = CreateBasePath();
                var ximPath = CreateXimFolderPath(basePath);
                var mhaPath = CreateMhaFolderPath(basePath);

                return new ProjectionFolderStructure(basePath, ximPath, mhaPath);
            });
        }

        public Task CreateFoldersAsync(ProjectionFolderStructure structure)
        {
            if (structure == null) throw new ArgumentNullException(nameof(structure));
            return Task.Run(async () =>
            {
                Logger.Info("Creates target folders.");
                await _fileUtil.CreateFolderAsync(structure.BasePath).ConfigureAwait(false);
                _fileUtil.CreateFolderAsync(structure.XimPath).ConfigureAwait(false);
                _fileUtil.CreateFolderAsync(structure.MhaPath).ConfigureAwait(false);
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
            return Path.Combine(basePath, CreateXimFolderName());
        }

        private string CreateXimFolderName()
        {
            return "xim";
        }

        private string CreateMhaFolderPath(string basePath)
        {
            return Path.Combine(basePath, CreateMhaFolderName());
        }

        private string CreateMhaFolderName()
        {
            return "mha";
        }
    }
}