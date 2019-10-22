using System;
using System.IO;
using System.Threading.Tasks;

namespace RTDS.Utility
{
    internal class FileUtil : IFileUtil
    {
        public async Task<string> CopyFileAsync(string sourceFile, string destinationFile)
        {
            if (sourceFile == null || destinationFile == null) throw new ArgumentNullException();
            return await CopyFileAsyncImpl(sourceFile, destinationFile).ConfigureAwait(false);
        }

        private async Task<string> CopyFileAsyncImpl(string sourceFile, string destinationFile)
        {
            using (FileStream sourceStream = File.Open(sourceFile, FileMode.Open))
            {
                using (FileStream destinationStream = File.Create(destinationFile))
                {
                    await sourceStream.CopyToAsync(destinationStream).ConfigureAwait(false);
                    return destinationFile;
                }
            }
        }

        public async Task<string> CreateFolderAsync(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return await CreateFolderAsyncImpl(path).ConfigureAwait(false);

        }

        private async Task<string> CreateFolderAsyncImpl(string path)
        {
            return await Task.Run(() =>
            {
                Directory.CreateDirectory(path);
                return path;
            }).ConfigureAwait(false);
        }
    }
}