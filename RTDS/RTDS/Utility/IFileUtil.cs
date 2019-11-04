using System.Threading.Tasks;

namespace RTDS.Utility
{
    internal interface IFileUtil
    {
        Task<string> CopyFileAsync(string sourceFile, string destinationFile);

        Task<string> CreateFolderAsync(string path);
    }
}