using System;
using RTDS.CBCTDataProvider.ProjectionProcessing;
using RTDS.DTO;

namespace RTDS.CBCTDataProvider.Monitoring.Args
{
    internal class PermFolderCreatedArgs : EventArgs
    {
        public PermStorageFolderStructure PermStorageFolderStructure { get; }
        public IProjectionProcessor Processor { get; }

        public string Id => PermStorageFolderStructure.BaseFolderName;

        public PermFolderCreatedArgs(PermStorageFolderStructure permStorageFolderStructure,
            IProjectionProcessor processor)
        {
            PermStorageFolderStructure = permStorageFolderStructure;
            Processor = processor;
        }
    }
}
