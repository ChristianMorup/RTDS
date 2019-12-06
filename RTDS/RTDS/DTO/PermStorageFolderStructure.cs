using System.IO;

namespace RTDS.DTO
{
    internal class PermStorageFolderStructure
    {
        public PermStorageFolderStructure(string basePath, string ximPath, string mhaPath, string ctPath)
        {
            BasePath = basePath;
            XimPath = ximPath;
            MhaPath = mhaPath;
            CtPath = ctPath;
        }

        public string BasePath { get; }
        public string XimPath { get; }
        public string MhaPath { get; }
        public string CtPath { get; }
        public string BaseFolderName => Path.GetDirectoryName(BasePath);
    }
}