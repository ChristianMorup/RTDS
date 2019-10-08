using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using RTDS.Monitoring;

//Note: https://github.com/jskeet/DemoCode/blob/master/AsyncIntro/Code/Testing.NUnit/StockBroker.cs
//Note: https://stackoverflow.com/questions/33254493/unit-testing-filesystemwatcher-how-to-programatically-fire-a-changed-event

namespace RTDS.UnitTest
{
    [TestFixture]
    public class TestFolderMonitor
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
            Assert.Throws<ArgumentNullException>(() => _uut.StarMonitoringAsync(null));
        }

        [TestCase("ThisIsAValidPath")]
        [TestCase("ThisIsAlsoAValidPath")]
        public void StartMonitoringAsync_ValidPathIsPassed_StartsMonitoringOnRightPath(string path)
        {
            //Act:
            Task task = _uut.StarMonitoringAsync(path);

            //Assert:
            AssertCompletion(task);
            _fakeWatcher.Received().Path = path;
        }

        [Test]
        public void StartMonitoringAsync_SetsFilters_CorrectFiltersAreSet()
        {
            //Act:
            Task task = _uut.StarMonitoringAsync("ValidPath");
            
            //Assert:
            AssertCompletion(task);
            _fakeWatcher.Received().NotifyFilters = NotifyFilters.DirectoryName;
        }

        [Test]
        public void StartMonitoringAsync_AddingHandlers_CorrectHandlersAreAdded()
        {
            //Act:
            Task task = _uut.StarMonitoringAsync("ValidPath");

            //Assert:
            AssertCompletion(task);
            _fakeWatcher.Received().Created += Arg.Any<FileSystemEventHandler>();
        }

        [Test]
        public void StartMonitoringAsync_EnablingRaisingEvents_EventsAreEnabled()
        {
            //Act:
            Task task = _uut.StarMonitoringAsync("ValidPath");

            //Assert:
            AssertCompletion(task);
            _fakeWatcher.Received().EnableRaisingEvents = true;
        }

        private void AssertCompletion(Task task)
        {
            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
        }
    }
}
