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
            return await CopyFileAsyncImpl(sourceFile, destinationFile);
        }

        private async Task<string> CopyFileAsyncImpl(string sourceFile, string destinationFile)
        {
            AwaitFile(sourceFile);

            using (FileStream sourceStream = File.Open(sourceFile, FileMode.Open))
            {
                using (FileStream destinationStream = File.Create(destinationFile))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                    return destinationFile;
                }
            }
        }

        private static void AwaitFile(string path)
        {
            var file = new FileInfo(path);
            //While File is not accesable because of writing process
            while (IsFileLocked(file))
            {
            }
        }

        private static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }


        public async Task<string> CreateFolderAsync(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return await CreateFolderAsyncImpl(path);
        }

        private async Task<string> CreateFolderAsyncImpl(string path)
        {
            return await Task.Run(() =>
            {
                Directory.CreateDirectory(path);
                return path;
            });
        }
    }
}