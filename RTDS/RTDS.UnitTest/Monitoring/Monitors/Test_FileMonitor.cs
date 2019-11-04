using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using NSubstitute;
using NUnit.Framework;
using RTDS.DTO;
using RTDS.Monitoring.Monitors;
using RTDS.Monitoring.Wrappers;

namespace RTDS.UnitTest.Monitoring.Monitors
{
    [TestFixture]
    public class Test_FileMonitor
    {
        private FileMonitor _uut;
        private IFileSystemWatcherWrapper _fakeWatcher;
        private ITimerWrapper _fakeTimer;

        [SetUp]
        public void Setup()
        {
            _fakeTimer = Substitute.For<ITimerWrapper>();
            _fakeWatcher = Substitute.For<IFileSystemWatcherWrapper>();
            _uut = new FileMonitor(_fakeWatcher, _fakeTimer);
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
            _fakeWatcher.Received().NotifyFilters = NotifyFilters.FileName;
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
        public void StartMonitoringAsync_FileIsCreated_EventIsRaised()
        {
            //Arrange:
            bool wasCalled = false;
            _uut.Created += (sender, args) => { wasCalled = true; };

            //Act:
            _uut.StartMonitoringAsync("ValidPath");
            _fakeWatcher.Created += Raise.Event<FileSystemEventHandler>(_fakeWatcher,
                new FileSystemEventArgs(WatcherChangeTypes.Created, "Test", "Test"));

            //Assert:
            Assert.That(wasCalled, Is.EqualTo(true));
        }

        [Test]
        public void StartMonitoringAsync_StartsMonitoring_TimerIsStarted()
        {
            //Act: 
            Task task = _uut.StartMonitoringAsync("ValidPath");

            //Assert:
            Task.WaitAll(task);
            _fakeTimer.Received().Enabled = true;
            _fakeTimer.Received().Interval = Arg.Any<double>();
            _fakeTimer.Received().Elapsed += Arg.Any<ElapsedEventHandler>();
        }

        [Test]
        public void StartMonitoringAsync_TimerExpires_MonitoringIsFinished()
        {
            //Arrange: 
            bool isFinished = false;

            _uut.Finished += (sender, args) => { isFinished = true; };
            Task task = _uut.StartMonitoringAsync("ValidPath");
            Task.WaitAll(task);

            //Act: 
            _fakeTimer.Elapsed += Raise.Event<ElapsedEventHandler>(new object(),
                new EventArgs() as ElapsedEventArgs);

            //Assert:
            Assert.That(isFinished, Is.EqualTo(true));
        }
    }
}