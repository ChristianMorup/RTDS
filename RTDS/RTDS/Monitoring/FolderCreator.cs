using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace RTDS.Monitoring
{
    internal class FolderCreator : IFolderCreator
    {
        public string CreateFolderAsync()
        {
            var path = CreatePermanentStoragePath();
            var ximPath = CreateXimFolderPath(path);
            CreateFoldersAsync(path, ximPath);
            return ximPath;
        }

        private void CreateFoldersAsync(string path, string ximPath)
        {
            if (path == null || ximPath == null) throw new ArgumentNullException(nameof(path));
            Task task = Task.Run(() =>
            {
                Directory.CreateDirectory(path);
                Directory.CreateDirectory(ximPath);
            });
        }

        private string CreatePermanentStoragePath()
        {
            string basePath = "C:\\Users\\chrmo\\Desktop\\RTDS_Dest";
            return Path.Combine(basePath, CreateBaseFolderName());
        }

        private string CreateBaseFolderName()
        {
            DateTime timeStamp = DateTime.Now;
            return timeStamp.ToString("yyyy-MM-dd-hh-mm-ss", CultureInfo.CurrentCulture);
        }

        private string CreateXimFolderPath(string basePath)
        {
            return Path.Combine(basePath, CreateXimFolderName());
        }

        private string CreateXimFolderName()
        {
            return "xim files";
        }
    }
}