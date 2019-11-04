namespace RTDS.DTO
{
    internal class PermStorageFolderStructure
    {
        public PermStorageFolderStructure(string basePath, string ximPath, string mhaPath)
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