using System.Diagnostics;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using RTDS.Monitoring;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Monitors;
using RTDS.Utility;

namespace RTDS.UnitTest.Monitoring
{
    [TestFixture]
    public class Test_BaseFolderController
    {
        private BaseFolderController _uut;
        private IMonitor _fakeFolderMonitor;
        private IMonitorFactory _fakeMonitorFactory;
        private IFileController _fakeFileController;

        [SetUp]
        public void SetUp()
        {
            _fakeFolderMonitor = Substitute.For<IMonitor>();
            _fakeMonitorFactory = Substitute.For<IMonitorFactory>();
            _fakeFileController = Substitute.For<IFileController>();
            _fakeMonitorFactory.CreateFolderMonitor().Returns(_fakeFolderMonitor);

            _uut = new BaseFolderController(_fakeMonitorFactory, _fakeFileController);
        }

        [Test]
        public void StartMonitoring_StartsFolderMonitor_FolderMonitorIsStarted()
        {
            //Arrange: 
            var path = "SomeValidPath";

            //Act: 
            _uut.StartMonitoring(path);

            //Assert:
            _fakeFolderMonitor.Received(1).StartMonitoringAsync(path);
        }


        [Test]
        public void StartMonitoring_NewFolderIsAdded_NewFileMonitorIsCreated()
        {
            //Arrange: 
            var basePath = "SomeValidBasePath";
            var filePath = "SomeValidFilePath";
            var name = "SomeName";

            //Act:
            _uut.StartMonitoring(basePath);

            _fakeFolderMonitor.Created +=
                Raise.EventWith<SearchDirectoryArgs>(new object(),
                    new SearchDirectoryArgs(filePath, name, _fakeFolderMonitor));

            //Assert:
            _fakeFileController.Received(1).MonitorNewFolderAsync(filePath, name);
        }

        [Test]
        public void StartMonitoring_NoFolderIsAdded_FileMonitorIsNotCreated()
        {
            //Arrange: 
            var path = "SomeValidPath";

            //Act:
            _uut.StartMonitoring(path);

            //Assert:
            _fakeMonitorFactory.DidNotReceive().CreateFileMonitor();
        }
    }
}