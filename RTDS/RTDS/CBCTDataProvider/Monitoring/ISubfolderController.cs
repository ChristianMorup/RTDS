using System;
using System.Threading.Tasks;
using RTDS.CBCTDataProvider.Monitoring.Args;
using RTDS.CBCTDataProvider.ProjectionProcessing.Args;

namespace RTDS.CBCTDataProvider.Monitoring
{
    internal interface ISubfolderController
    {
        Task StartNewFileMonitorInNewFolderAsync(string path, string folderName);

        event EventHandler<PipelineStartedArgs> FolderDetected;

    }
}