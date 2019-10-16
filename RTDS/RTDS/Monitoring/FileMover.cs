using System.IO;
using System.Threading.Tasks;

namespace RTDS.Monitoring
{
    internal class FileMover : IFileMover
    {
        public async Task<string> MoveFileAsync(string sourceFile, string destinationFile)
        {
            return await MoveFileAsyncImpl(sourceFile, destinationFile);
        }

        private async Task<string> MoveFileAsyncImpl(string sourceFile, string destinationFile)
        {
            using (FileStream sourceStream = File.Open(sourceFile, FileMode.Open))
            {
                using (FileStream destinationStream = File.Create(destinationFile))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                    return destinationFile;
                }
            }
        }
    }
}