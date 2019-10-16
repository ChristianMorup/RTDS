using System.Collections.Concurrent;

namespace RTDS.Monitoring
{
    internal class ProjectionInfo
    {
        public ProjectionInfo(string permanentStorageXimPath,
            BlockingCollection<string> filesToBeTransferred, BlockingCollection<string> filesToBeConverted)
        {
            PermanentStorageXimPath = permanentStorageXimPath;
            FilesToBeTransferred = filesToBeTransferred;
            FilesToBeConverted = filesToBeConverted;
        }

        public string PermanentStorageXimPath { get; set; }
        public BlockingCollection<string> FilesToBeTransferred { get; set; }
        public BlockingCollection<string> FilesToBeConverted { get; set; }
    }
}