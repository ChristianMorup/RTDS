using System;
using System.IO;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using RTDS.Monitoring;
using RTDS.Monitoring.Wrapper;

//Note: https://github.com/jskeet/DemoCode/blob/master/AsyncIntro/Code/Testing.NUnit/StockBroker.cs
//Note: https://stackoverflow.com/questions/33254493/unit-testing-filesystemwatcher-how-to-programatically-fire-a-changed-event

namespace RTDS.UnitTest.Monitoring
{
    [TestFixture]
    public class Test_FolderMonitor
    {
        private FolderMonitor _uut;
        private IFileSystemWatcherWrapper _fakeWatcher;

        [SetUp]
        public void Setup()
        {
            _fakeWatcher = Substitute.For<IFileSystemWatcherWrapper>();
            _uut = new FolderMonitor(_fakeWatcher);
        }

        [Test]
        public void StartMonitoringAsync_NullArgumentIsPassed_ThrowsException()
        {
            //Act + Assert:
            Assert.Throws<ArgumentNullException>(() => _uut.StartMonitoringAsync(null));
        }

        [TestCase("ThisIsAValidPath")]
        [TestCase("ThisIsAlsoAValidPath")]
        public void StartMonitoringAsync_ValidPathIsPassed_StartsMonitoringOnRightPath(string path)
        {
            //Act:
            Task task = _uut.StartMonitoringAsync(path);

            //Assert:
            Task.WaitAll(task);
            _fakeWatcher.Received().Path = path;
            Assert.That(_uut.MonitoredPath, Is.EqualTo(path));
        }

        [Test]
        public void StartMonitoringAsync_SetsFilters_CorrectFiltersAreSet()
        {
            //Act:
            Task task = _uut.StartMonitoringAsync("ValidPath");

            //Assert:
            Task.WaitAll(task);
            _fakeWatcher.Received().NotifyFilters = NotifyFilters.DirectoryName;
        }

        [Test]
        public void StartMonitoringAsync_AddingHandlers_CorrectHandlersAreAdded()
        {
            //Act:
            Task task = _uut.StartMonitoringAsync("ValidPath");

            //Assert:
            Task.WaitAll(task);
            _fakeWatcher.Received().Created += Arg.Any<FileSystemEventHandler>();
        }

        [Test]
        public void StartMonitoringAsync_EnablingRaisingEvents_EventsAreEnabled()
        {
            //Act:
            Task task = _uut.StartMonitoringAsync("ValidPath");

            //Assert:
            Task.WaitAll(task);
            _fakeWatcher.Received().EnableRaisingEvents = true;
        }

        [Test]
        public void StartMonitoringAsync_FolderIsCreated_EventIsRaised()
        {
            //Arrange:
            bool wasCalled = false;
            _uut.Created += (sender, args) => wasCalled = true;

            //Act:
            Task task = _uut.StartMonitoringAsync("ValidPath");
            Task.WaitAll(task);
            _fakeWatcher.Created += Raise.Event<FileSystemEventHandler>(_fakeWatcher,
                new FileSystemEventArgs(WatcherChangeTypes.Created, "Test", "Test"));

            //Assert:
            Assert.That(wasCalled, Is.EqualTo(true));
        }


    }
}