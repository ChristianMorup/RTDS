namespace RTDS.DTO
{
    internal class TempProjectionInfo
    {
        public TempProjectionInfo(int id, string filePath)
        {
            Id = id;
            FilePath = filePath;
        }
        public int Id { get; set; }
        
        public string FilePath { get; set; }
    }
}