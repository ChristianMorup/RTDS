namespace RTDS.Monitoring
{
    internal class ProjectionInfo
    {
        public ProjectionInfo(int id, string permanentStoragePath, string tempStoragePath, string name)
        {
            Id = id;
            PermanentStoragePath = permanentStoragePath;
            TempStoragePath = tempStoragePath;
            Name = name;
        }

        public int Id { get; set; }

        public string PermanentStoragePath { get; set; }

        public string TempStoragePath { get; set; }

        public string Name { get; set; }
    }
}