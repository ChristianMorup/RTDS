using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using RTDS.Monitoring;
using RTDS.Utility;

namespace RTDS.IntegrationTest.Utility
{
    [TestFixture]
    public class Test_FileUtil : AbstractFileSystemTest
    {
        private string _sourcePath;
        private string _targetPath;
        private FileUtil _uut;

        [SetUp]
        public void SetUp()
        {
            _sourcePath = CreateFolderInDirectory(TestDataPath, "Source");
            _targetPath = CreateFolderInDirectory(TestDataPath, "Target");
            _uut = new FileUtil();
        }

        [Test]
        public void MoveFileAsync_NullSourceFilePathIsPassed_ThrowsException()
        {
            //Act + Assert:
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _uut.CopyFileAsync(null, "Test"));
        }

        [Test]
        public void MoveFileAsync_NullDestFilePathIsPassed_ThrowsException()
        {
            //Act + Assert:
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _uut.CopyFileAsync("Test", null));
        }

        [Test]
        public void MoveFileAsync_MovesFile_FileIsMoved()
        {
            //Arrange:
            var sourceFile = CreateFileInDirectory(_sourcePath, "test");

            //Act:
            var targetFile = _uut.CopyFileAsync(sourceFile, Path.Combine(_targetPath, "test")).Result;

            //Assert:
            Assert.That(File.Exists(targetFile), Is.True);
        }

        [Test]
        public void MoveFileAsync_MovesFile_FileContainsSameInformation()
        {
            //Arrange:
            var randomContent = "Random text content";
            var sourceFile = CreateFileInDirectory(_sourcePath, "test", randomContent);

            //Act:
            var targetFile = _uut.CopyFileAsync(sourceFile, Path.Combine(_targetPath, "test")).Result;

            //Assert:
            Assert.That(FileEquals(sourceFile, targetFile), Is.True);
        }

        [Test]
        public void CreateFolderAsync_NullPathIsPassed_ThrowsException()
        {
            //Act + Assert:
            Assert.ThrowsAsync<ArgumentNullException>( async () => await _uut.CreateFolderAsync(null));
        }

        [Test]
        public void CreateFolderAsync_CreatesFolder_FolderIsCreatedAtPath()
        {
            //Arrange:
            var randomFolderName = "FolderName";

            //Act:
            var createdFolderPath = _uut.CreateFolderAsync(randomFolderName).Result;

            //Assert: 
            Assert.That(Directory.Exists(createdFolderPath), Is.True);
        }

        private bool FileEquals(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}