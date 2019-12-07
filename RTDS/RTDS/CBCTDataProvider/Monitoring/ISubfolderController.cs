using System;
using System.Threading.Tasks;
using RTDS.CBCTDataProvider.Monitoring.Args;

namespace RTDS.CBCTDataProvider.Monitoring
{
    internal interface ISubfolderController
    {
        Task StartNewFileMonitorInNewFolderAsync(string path, string folderName);

        event EventHandler<PermFolderCreatedArgs> FolderDetected;

    }
}