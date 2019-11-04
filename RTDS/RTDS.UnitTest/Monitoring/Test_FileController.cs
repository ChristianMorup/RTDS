using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using RTDS.DTO;
using RTDS.Monitoring;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Monitors;

namespace RTDS.UnitTest.Monitoring
{
    [TestFixture]
    public class Test_FileController
    {
        private FileController _uut;
        private IMonitorFactory _fakeMonitorFactory;
        private IFileMonitor _fakeFileMonitor;
        private IProjectionFolderCreator _fakeProjectionFolderCreator;


        [SetUp]
        public void SetUp()
        {
            _fakeMonitorFactory = Substitute.For<IMonitorFactory>();
            _fakeFileMonitor = Substitute.For<IFileMonitor>();
            _fakeMonitorFactory.CreateFileMonitor().Returns(_fakeFileMonitor);
            _fakeProjectionFolderCreator = Substitute.For<IProjectionFolderCreator>();
            _uut = new FileController(_fakeProjectionFolderCreator, _fakeMonitorFactory);
        }

        [Test]
        public void StartNewFileMonitorInNewFolderAsync_CreatesFileMonitor_NewFileMonitorIsCreated()
        {
            //Act: 
            Task task = _uut.StartNewFileMonitorInNewFolderAsync("path", "folder");
            Task.WaitAll(task);

            //Assert:
            _fakeMonitorFactory.Received(1).CreateFileMonitor();
        }

        [Test]
        public void StartNewFileMonitorInNewFolderAsync_SubscribesANewListener_FinishedListenerIsSubscribed()
        {
            //Arrange: 
            IFileMonitorListener _fakeListener = Substitute.For<IFileMonitorListener>();
            _fakeMonitorFactory.CreateFileMonitorListener(Arg.Any<PermStorageFolderStructure>()).Returns(_fakeListener);

            //Act: 
            Task task = _uut.StartNewFileMonitorInNewFolderAsync("path", "folder");
            Task.WaitAll(task);

            _fakeFileMonitor.Finished +=
                Raise.EventWith<FileMonitorFinishedArgs>(new object(), new FileMonitorFinishedArgs(_fakeFileMonitor));

            //Assert:
            _fakeListener.Received(1).OnMonitorFinished(Arg.Any<object>(), Arg.Any<FileMonitorFinishedArgs>());
        }

        [Test]
        public void StartNewFileMonitorInNewFolderAsync_SubscribesANewListener_CreatedListenerIsSubscribed()
        {
            //Arrange: 
            IFileMonitorListener _fakeListener = Substitute.For<IFileMonitorListener>();
            _fakeMonitorFactory.CreateFileMonitorListener(Arg.Any<PermStorageFolderStructure>()).Returns(_fakeListener);

            //Act: 
            Task task = _uut.StartNewFileMonitorInNewFolderAsync("path", "folder");
            Task.WaitAll(task);

            _fakeFileMonitor.Created += Raise.EventWith(new object(),
                new SearchDirectoryArgs("path", "name", _fakeFileMonitor));

            //Assert:
            _fakeListener.Received(1).OnNewFileDetected(Arg.Any<object>(), Arg.Any<SearchDirectoryArgs>());
        }

        [Test]
        public void MonitorNewFolderAsync_StartsMonitor_FileMonitorIsStarted()
        {
            //Arrange:
            var path = "Some path";
            _fakeMonitorFactory.CreateFileMonitor().Returns(_fakeFileMonitor);

            //Act:
            Task task = _uut.StartNewFileMonitorInNewFolderAsync(path, "Some name");

            Task.WaitAll(task);

            //Assert:
            _fakeFileMonitor.Received(1).StartMonitoringAsync(path);
        }

        [Test]
        public void StartNewFileMonitorInNewFolderAsync_StartsMonitor_MonitorIsStarted()
        {
            //Arrange:
            var path = "Some path";

            //Act: 
            Task task = _uut.StartNewFileMonitorInNewFolderAsync(path, "folder");
            Task.WaitAll(task);

            //Assert:
            _fakeFileMonitor.Received(1).StartMonitoringAsync(path);
        }
    }
}