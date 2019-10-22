using NSubstitute;
using NUnit.Framework;
using RTDS.Configuration.Data;
using RTDS.Monitoring;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Monitors;

namespace RTDS.UnitTest.Monitoring
{
    [TestFixture]
    public class Test_BaseFolderController
    {
        private BaseFolderController _uut;
        private IMonitor _fakeFolderMonitor;
        private IMonitorFactory _fakeMonitorFactory;
        private IFileController _fakeFileController;
        private RTDSPaths _paths;

        [SetUp]
        public void SetUp()
        {
            _fakeFolderMonitor = Substitute.For<IMonitor>();
            _fakeMonitorFactory = Substitute.For<IMonitorFactory>();
            _fakeFileController = Substitute.For<IFileController>();
            _fakeMonitorFactory.CreateFolderMonitor().Returns(_fakeFolderMonitor);
            var configuration = CreateDefaultConfiguration();
            _paths = configuration.Paths;
            Configuration.ConfigurationManager.OverrideConfiguration(configuration, false);
            _uut = new BaseFolderController(_fakeMonitorFactory, _fakeFileController);
        }

        [Test]
        public void StartMonitoring_StartsFolderMonitor_FolderMonitorIsStarted()
        {
            //Arrange: 
            var path = _paths.BaseSourcePath;

            //Act: 
            _uut.StartMonitoring();

            //Assert:
            _fakeFolderMonitor.Received(1).StartMonitoringAsync(path);
        }


        [Test]
        public void StartMonitoring_NewFolderIsAdded_NewFileMonitorIsCreated()
        {
            //Arrange: 
            var basePath = _paths.BaseSourcePath;
            var filePath = "Some file path";
            var name = "SomeName";

            //Act:
            _uut.StartMonitoring();

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
            var path = _paths.BaseSourcePath;

            //Act:
            _uut.StartMonitoring();

            //Assert:
            _fakeMonitorFactory.DidNotReceive().CreateFileMonitor();
        }

        private RTDSConfiguration CreateDefaultConfiguration()
        {
            return new RTDSConfiguration
            {
                Paths = new RTDSPaths
                {
                    BaseSourcePath = "Test source",
                    BaseTargetPath = "Test target"
                }
            };
        }
    }
}