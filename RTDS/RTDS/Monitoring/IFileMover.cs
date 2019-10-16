using System.Threading.Tasks;

namespace RTDS.Monitoring
{
    internal interface IFileMover
    {
        Task<string> MoveFileAsync(string sourceFile, string destinationFile);
    }
}