using System;
using System.Threading.Tasks;
using RTDS.Monitoring.Args;

namespace RTDS.Monitoring
{
    internal interface IFileController
    {
        Task StartNewFileMonitorInNewFolderAsync(string path, string folderName);

        event EventHandler<PermFolderCreatedArgs> FolderDetected;

    }
}