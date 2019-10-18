using System.Threading.Tasks;

namespace RTDS.Monitoring
{
    internal interface IFileController
    {
        Task MonitorNewFolderAsync(string path, string folderName);
    }
}