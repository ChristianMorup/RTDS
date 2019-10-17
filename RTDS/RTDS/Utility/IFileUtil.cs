using System.Threading.Tasks;

namespace RTDS.Monitoring
{
    internal interface IFileUtil
    {
        Task<string> CopyFileAsync(string sourceFile, string destinationFile);

        Task<string> CreateFolderAsync(string path);
    }
}