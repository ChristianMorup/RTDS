using System.Threading.Tasks;

namespace RTDS.Monitoring
{
    internal interface IFileController
    {
        Task StartNewFileMonitorInNewFolderAsync(string path, string folderName);
    }
}