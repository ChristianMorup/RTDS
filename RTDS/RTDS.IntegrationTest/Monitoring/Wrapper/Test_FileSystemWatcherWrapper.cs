using System.IO;
using System.Threading;
using NUnit.Framework;
using RTDS.Monitoring.Wrapper;

namespace RTDS.IntegrationTest.Monitoring.Wrapper
{
    [TestFixture]
    public class TestFileSystemWatcherWrapper
    {
        private readonly string _testDataFolder = "TestData";
        private string _testDataPath;
        private FileSystemWatcherWrapper _uut;
        private FileSystemWatcher _watcher;
        private bool _eventReceived;

        [SetUp]
        public void SetUp()
        {
            _testDataPath = CreateFolderInDirectory(Directory.GetCurrentDirectory(), _testDataFolder);
            _watcher = new FileSystemWatcher();
            _uut = new FileSystemWatcherWrapper(_watcher);
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

        [Test]
        public void SetParameters_ParametersAreGiven_FileWatcherUsesCorrectParameters()
        {
            //Act:
            _uut.Path = _testDataPath;
            _uut.Filter = "*.test";
            _uut.NotifyFilters = NotifyFilters.CreationTime;
            _uut.EnableRaisingEvents = true;

            //Assert:
            Assert.That(_watcher.Path, Is.EqualTo(_testDataPath));
            Assert.That(_uut.Path, Is.EqualTo(_testDataPath));

            Assert.That(_watcher.Filter, Is.EqualTo("*.test"));
            Assert.That(_uut.Filter, Is.EqualTo("*.test"));

            Assert.That(_watcher.NotifyFilter, Is.EqualTo(NotifyFilters.CreationTime));
            Assert.That(_uut.NotifyFilters, Is.EqualTo(NotifyFilters.CreationTime));

            Assert.That(_watcher.EnableRaisingEvents, Is.True);
            Assert.That(_uut.EnableRaisingEvents, Is.True);
        }

        [Test]
        public void Dispose_DisposesTimer_TimerIsDisposed()
        {
            //Arrange: 
            bool isDisposed = false;
            _watcher.Disposed += (sender, args) => { isDisposed = true; };

            //Act:
            _uut.Dispose();

            //Assert:
            Assert.That(isDisposed, Is.True);
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            _eventReceived = true;
        }
    }
}