using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RTDS.Monitoring;
using RTDS.Monitoring.Wrapper;


namespace RTDS.IntegrationTest
{
    [TestFixture]
    public class TestFileSystemWatcherWrapper
    {
        private readonly string _testDataFolder = "TestData";
        private string _testDataPath;
        private FileSystemWatcherWrapper _uut;
        private bool _eventReceived; 

        [SetUp]
        public void SetUp()
        {
            _testDataPath = CreateFolderInDirectory(Directory.GetCurrentDirectory(), _testDataFolder);
            _uut = new FileSystemWatcherWrapper(new FileSystemWatcher());
            _eventReceived = false;
        }

        [TearDown]
        public void TearDown()
        {
            DirectoryInfo testDirectory = new DirectoryInfo(_testDataPath);

            foreach (FileInfo file in testDirectory.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo directory in testDirectory.GetDirectories())
            {
                directory.Delete(true);
            }
        }

        private string CreateFolderInDirectory(string path, string folderName)
        {
            if (!Directory.Exists(Path.Combine(path, folderName)))
            {
                Directory.CreateDirectory(Path.Combine(path, folderName));
            }
            return Path.Combine(path, folderName);
        }

        [Test]
        public void EnableRaisingEvents_FolderGetsCreated_NotifiesThatFolderHasBeenCreated()
        {
            //Arrange:
            _uut.NotifyFilters = NotifyFilters.DirectoryName;
            _uut.Path = _testDataPath;
            _uut.Created += OnCreated;
            _uut.EnableRaisingEvents = true;

            //Act:
            CreateFolderInDirectory(_testDataPath, "Test");

            //Waiting due to asynchronous detection of the folder being created. 
            Thread.Sleep(2000);

            //Assert:
            Assert.That(_eventReceived, Is.EqualTo(true));
        }
        

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            _eventReceived = true;
        }

    }
}