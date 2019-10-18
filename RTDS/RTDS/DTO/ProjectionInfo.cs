using System.Collections.Concurrent;

namespace RTDS.DTO
{
    internal class ProjectionInfo
    {
        public ProjectionInfo(ProjectionFolderStructure structure)
        {
            Structure = structure;
            FilesToBeTransferred = new BlockingCollection<string>();
            FilesToBeConverted = new BlockingCollection<string>();
        }

        public ProjectionFolderStructure Structure{ get; set; }
        public BlockingCollection<string> FilesToBeTransferred { get; set; }
        public BlockingCollection<string> FilesToBeConverted { get; set; }
    }
}