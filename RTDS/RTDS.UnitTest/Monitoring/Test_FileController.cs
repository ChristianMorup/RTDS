using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
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
        private IProjectionController _fakeProjectionController;

        [SetUp]
        public void SetUp()
        {
            _fakeMonitorFactory = Substitute.For<IMonitorFactory>();
            _fakeFileMonitor = Substitute.For<IFileMonitor>();
            _fakeMonitorFactory.CreateFileMonitor().Returns(_fakeFileMonitor);
            _fakeProjectionController = Substitute.For<IProjectionController>();

            _uut = new FileController(_fakeProjectionController, _fakeMonitorFactory);
        }

        [Test]
        public void MonitorNewFolderAsync_CreatesFileMonitor_NewFileMonitorIsCreated()
        {
            //Act: 
            Task task = _uut.MonitorNewFolderAsync("Path", "Folder");

            //Assert:
            _fakeMonitorFactory.Received(1).CreateFileMonitor();
        }

        [Test]
        public void MonitorNewFolderAsync_GetsProjectionInfo_ProjectionControllerWasCalled()
        {
            //Arrange: 
            var structure = new ProjectionFolderStructure("base", "xim", "mha");
            _fakeProjectionController.CreateProjectionInfo().Returns(new ProjectionInfo(structure));

            //Act: 
            _uut.MonitorNewFolderAsync("Path", "Folder");

            //Assert:
            _fakeProjectionController.Received(1).CreateProjectionInfo();
        }

        [Test]
        public void MonitorNewFolderAsync_StartsMonitor_FileMonitorIsStarted()
        {
            //Arrange:
            var path = "Some path";
            _fakeMonitorFactory.CreateFileMonitor().Returns(_fakeFileMonitor);

            //Act:
            Task task = _uut.MonitorNewFolderAsync(path, "Some name");

            Task.WaitAll(task);

            //Assert:
            _fakeFileMonitor.Received(1).StartMonitoringAsync(path);
        }

        [Test]
        public void MonitorNewFolderAsync_NewFileIsDetected_HandlesNewFile()
        {
            //Arrange:
            var path = "Some path";
            var name = "Some name";
            _fakeMonitorFactory.CreateFileMonitor().Returns(_fakeFileMonitor);

            Task task = _uut.MonitorNewFolderAsync(path, "Some name");
            Task.WaitAll(task);

            //Act:
            _fakeFileMonitor.Created += Raise.EventWith<SearchDirectoryArgs>(_fakeFileMonitor,
                new SearchDirectoryArgs(path, name, _fakeFileMonitor));

            //Assert:
            _fakeProjectionController.Received(1)
                .HandleNewFile(_fakeFileMonitor, path, Arg.Any<Dictionary<Guid, ProjectionInfo>>());
        }

        [Test]
        public void MonitorNewFolderAsync_MonitorIsFinished_MonitorIsRemovedFromDictionary()
        {
            //Arrange:
            var path = "Some path";
            var name = "Some name";
            Guid guid = Guid.NewGuid();
            _fakeMonitorFactory.CreateFileMonitor().Returns(_fakeFileMonitor);
            _fakeFileMonitor.Guid.Returns(guid);
            _fakeProjectionController = new FakeProjectionController();
            _uut = new FileController(_fakeProjectionController, _fakeMonitorFactory);

            Task task = _uut.MonitorNewFolderAsync(path, "Some name");
            Task.WaitAll(task);

            //Act:
            _fakeFileMonitor.Finished += Raise.EventWith<FileMonitorFinishedArgs>(_fakeFileMonitor,
                new FileMonitorFinishedArgs(_fakeFileMonitor));

            _fakeFileMonitor.Created += Raise.EventWith<SearchDirectoryArgs>(_fakeFileMonitor,
                new SearchDirectoryArgs(path, name, _fakeFileMonitor));

            //Assert: 
            var controller = (FakeProjectionController) _fakeProjectionController;
            Assert.That(controller.MonitorByQueueMap.ContainsKey(guid), Is.False);
            
        }


        internal class FakeProjectionController : IProjectionController
        {
            public Dictionary<Guid, ProjectionInfo> MonitorByQueueMap { get; set; }
            public Task HandleNewFile(IMonitor relatedMonitor, string path,
                Dictionary<Guid, ProjectionInfo> monitorByQueueMap)
            {
                MonitorByQueueMap = monitorByQueueMap;
                return new Task(() => {});
            }

            public Task<ProjectionInfo> CreateProjectionInfo()
            {
                return Task.Run(() => new ProjectionInfo(new ProjectionFolderStructure("test", "test", "test")));
            }
        }
    }
}