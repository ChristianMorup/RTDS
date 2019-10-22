using System.IO;
using NUnit.Framework;

namespace RTDS.IntegrationTest
{
    public abstract class AbstractFileSystemTest
    {
        protected string TestDataPath;

        [SetUp]
        public void BasicSetUp()
        {
            TestDataPath = CreateFolderInDirectory(Directory.GetCurrentDirectory(), TestGlobals.TestDataFolder);
        }

        protected string CreateFolderInDirectory(string path, string folderName)
        {
            var fullPath = Path.Combine(path, folderName);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            return fullPath;
        }

        protected string CreateFileInDirectory(string path, string fileName)
        {
            var fullPath = Path.Combine(path, fileName);
            if (!File.Exists(fullPath))
            {
                using (FileStream stream = File.Create(fullPath)) ;
            }

            return fullPath;
        }

        protected string CreateFileInDirectory(string path, string fileName, string content)
        {
            var fullPath = Path.Combine(path, fileName);
            if (!File.Exists(fullPath))
            {
                File.WriteAllText(fullPath, content);
            }

            return fullPath;
        }

        [TearDown]
        public void BasicTearDown()
        {
            DirectoryInfo testDirectory = new DirectoryInfo(TestDataPath);

            foreach (FileInfo file in testDirectory.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo directory in testDirectory.GetDirectories())
            {
                directory.Delete(true);
            }
        }
    }
}