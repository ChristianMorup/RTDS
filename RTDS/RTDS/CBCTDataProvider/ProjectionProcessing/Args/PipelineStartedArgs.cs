using System;
using RTDS.DTO;

namespace RTDS.CBCTDataProvider.ProjectionProcessing.Args
{
    internal class PipelineStartedArgs : EventArgs
    {
        public PermStorageFolderStructure PermStorageFolderStructure { get; }
        public IReconstructionProcessor Processor { get; }

        public string Id => PermStorageFolderStructure.BaseFolderName;

        public PipelineStartedArgs(PermStorageFolderStructure permStorageFolderStructure,
            IReconstructionProcessor processor)
        {
            PermStorageFolderStructure = permStorageFolderStructure;
            Processor = processor;
        }
    }
}
