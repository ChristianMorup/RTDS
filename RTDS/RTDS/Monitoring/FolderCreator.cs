using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using ConfigurationManager = RTDS.Configuration.ConfigurationManager;

namespace RTDS.Monitoring
{
    internal class FolderCreator : IFolderCreator
    {
        private readonly IFileUtil _fileUtil;

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
                await _fileUtil.CreateFolderAsync(structure.BasePath).ConfigureAwait(false);
                _fileUtil.CreateFolderAsync(structure.XimPath).ConfigureAwait(false);
                _fileUtil.CreateFolderAsync(structure.MhaPath).ConfigureAwait(false);
            });
        }

        private string CreateBasePath()
        {
            string basePath = ConfigurationManager.GetConfiguration("BaseTargetPath");
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

    internal class ProjectionFolderStructure
    {
        public ProjectionFolderStructure(string basePath, string ximPath, string mhaPath)
        {
            BasePath = basePath;
            XimPath = ximPath;
            MhaPath = mhaPath;
        }

        public string BasePath { get; }
        public string XimPath { get; }
        public string MhaPath { get; }
    }
}