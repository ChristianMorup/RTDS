using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using NSubstitute;
using NUnit.Framework;
using RTDS.Monitoring;
using RTDS.Monitoring.Wrapper;

namespace RTDS.UnitTest.Monitoring
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
            AssertCompletion(task);
            _fakeWatcher.Received().Path = path;
        }

        [Test]
        public void StartMonitoringAsync_SetsFilters_CorrectFiltersAreSet()
        {
            //Act:
            Task task = _uut.StartMonitoringAsync("ValidPath");

            //Assert:
            AssertCompletion(task);
            _fakeWatcher.Received().NotifyFilters = NotifyFilters.FileName;
        }

        [Test]
        public void StartMonitoringAsync_AddingHandlers_CorrectHandlersAreAdded()
        {
            //Act:
            Task task = _uut.StartMonitoringAsync("ValidPath");

            //Assert:
            AssertCompletion(task);
            _fakeWatcher.Received().Created += Arg.Any<FileSystemEventHandler>();
        }

        [Test]
        public void StartMonitoringAsync_EnablingRaisingEvents_EventsAreEnabled()
        {
            //Act:
            Task task = _uut.StartMonitoringAsync("ValidPath");

            //Assert:
            AssertCompletion(task);
            _fakeWatcher.Received().EnableRaisingEvents = true;
        }

        [Test]
        public void StartMonitoringAsync_FileIsCreated_EventIsRaised()
        {
            //Arrange:
            bool wasCalled = false;
            _uut.Created += (sender, args) =>
            {
                wasCalled = true;
            };

            //Act:
            Task task = _uut.StartMonitoringAsync("ValidPath");
            _fakeWatcher.Created += Raise.Event<FileSystemEventHandler>(_fakeWatcher,
                new FileSystemEventArgs(WatcherChangeTypes.Created, "Test", "Test"));

            //Assert:
            AssertCompletion(task);
            Assert.That(wasCalled, Is.EqualTo(true));
        }

        [Test]
        public void StartMonitoringAsync_StartsMonitoring_TimerIsStarted()
        {
            //Act: 
            _uut.StartMonitoringAsync("ValidPath");

            //Assert:
            _fakeTimer.Received().Enabled = true;
            _fakeTimer.Received().Interval = Arg.Any<double>();
            _fakeTimer.Received().Elapsed += Arg.Any<ElapsedEventHandler>();
        }

        [Test]
        public void StartMonitoringAsync_TimerExpires_MonitoringIsFinished()
        {
            //Arrange: 
            bool isFinished = false;
            var waithandle = new ManualResetEvent(false);

            _uut.Finished += (sender, args) =>
            {
                isFinished = true;
                waithandle.Set();
            };
            _uut.StartMonitoringAsync("ValidPath");

            //Act: 
            _fakeTimer.Elapsed += Raise.Event<ElapsedEventHandler>(new object(),
                new EventArgs() as ElapsedEventArgs);

            var eventFired = waithandle.WaitOne(100);
            
            //Assert:
            Assert.That(isFinished, Is.EqualTo(true));
        }

        private void AssertCompletion(Task task)
        {
            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
        }
    }
}