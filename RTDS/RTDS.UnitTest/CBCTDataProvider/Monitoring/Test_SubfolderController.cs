using System.Collections.Concurrent;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using RTDS.CBCTDataProvider.Monitoring;
using RTDS.CBCTDataProvider.Monitoring.Args;
using RTDS.CBCTDataProvider.Monitoring.Factory;
using RTDS.CBCTDataProvider.Monitoring.Monitors;
using RTDS.CBCTDataProvider.ProjectionProcessing;
using RTDS.CBCTDataProvider.ProjectionProcessing.Factory;
using RTDS.DTO;

namespace RTDS.UnitTest.CBCTDataProvider.Monitoring
{
    [TestFixture]
    public class Test_SubfolderController
    {
        private SubfolderController _uut;
        private IMonitorFactory _fakeMonitorFactory;
        private IFileMonitor _fakeFileMonitor;
        private IProjectionFolderCreator _fakeProjectionFolderCreator;
        private IProjectionPipelineFactory _fakeProjectionPipelineFactory;

        [SetUp]
        public void SetUp()
        {
            _fakeMonitorFactory = Substitute.For<IMonitorFactory>();
            _fakeFileMonitor = Substitute.For<IFileMonitor>();
            _fakeMonitorFactory.CreateFileMonitor().Returns(_fakeFileMonitor);
            _fakeProjectionFolderCreator = Substitute.For<IProjectionFolderCreator>();
            _fakeProjectionPipelineFactory = Substitute.For<IProjectionPipelineFactory>();
            _uut = new SubfolderController(_fakeProjectionFolderCreator, _fakeMonitorFactory,
                _fakeProjectionPipelineFactory);
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
        public void StartNewFileMonitorInNewFolderAsync_SubscribesANewListener_CreatedListenerIsSubscribed()
        {
            //Arrange: 
            ISubfolderMonitorListener _fakeListener = Substitute.For<ISubfolderMonitorListener>();
            _fakeProjectionPipelineFactory.CreateFileMonitorListener(Arg.Any<PermStorageFolderStructure>(),
                Arg.Any<BlockingCollection<TempProjectionInfo>>()).Returns(_fakeListener);

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