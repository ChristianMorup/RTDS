namespace RTDS.DTO
{
    internal class ProjectionInfo
    {
        public ProjectionInfo(int id, string permanentStoragePath, string tempStoragePath, string fileName)
        {
            Id = id;
            PermanentStoragePath = permanentStoragePath;
            TempStoragePath = tempStoragePath;
            FileName = fileName;
        }

        public int Id { get; set; }

        public string PermanentStoragePath { get; set; }

        public string TempStoragePath { get; set; }

        public string FileName { get; set; }
    }
}