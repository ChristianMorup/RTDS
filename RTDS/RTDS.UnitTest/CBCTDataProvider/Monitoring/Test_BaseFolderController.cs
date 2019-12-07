using System.IO;
using NSubstitute;
using NUnit.Framework;
using RTDS.CBCTDataProvider.Monitoring;
using RTDS.CBCTDataProvider.Monitoring.Args;
using RTDS.CBCTDataProvider.Monitoring.Factory;
using RTDS.CBCTDataProvider.Monitoring.Monitors;
using RTDS.Configuration.Data;

namespace RTDS.UnitTest.CBCTDataProvider.Monitoring
{
    [TestFixture]
    public class Test_BaseFolderController
    {
        private BaseFolderController _uut;
        private IMonitor _fakeFolderMonitor;
        private IMonitorFactory _fakeMonitorFactory;
        private ISubfolderController _fakeSubfolderController;
        private RTDSPaths _paths;

        [SetUp]
        public void SetUp()
        {
            _fakeFolderMonitor = Substitute.For<IMonitor>();
            _fakeMonitorFactory = Substitute.For<IMonitorFactory>();
            _fakeSubfolderController = Substitute.For<ISubfolderController>();
            _fakeMonitorFactory.CreateFolderMonitor().Returns(_fakeFolderMonitor);

            var configuration = CreateDefaultConfiguration();
            _paths = configuration.Paths;
            Configuration.ConfigurationManager.OverrideConfiguration(configuration, false);

            _uut = new BaseFolderController(_fakeMonitorFactory, _fakeSubfolderController);
        }

        [Test]
        public void StartFolderMonitor_StartsFolderMonitor_FolderMonitorIsStarted()
        {
            //Act: 
            _uut.StartFolderMonitor();

            //Assert:
            _fakeFolderMonitor.Received(1).StartMonitoringAsync(_paths.BaseSourcePath);
        }

        [Test]
        public void StartFolderMonitor_NewFolderIsAdded_FileControllerIsCalled()
        {
            //Arrange: 
            var filePath = "Some path";
            var fileName = "Some file name";

            //Act:
            _uut.StartFolderMonitor();

            _fakeFolderMonitor.Created +=
                Raise.EventWith<SearchDirectoryArgs>(new object(),
                     new SearchDirectoryArgs(filePath, fileName));

            //Assert:
            _fakeSubfolderController.Received(1).StartNewFileMonitorInNewFolderAsync(filePath, fileName);
        }

        private RTDSConfiguration CreateDefaultConfiguration()
        {
            return new RTDSConfiguration
            {
                Paths = new RTDSPaths
                {
                    BaseSourcePath = Directory.GetCurrentDirectory(),
                    BaseTargetPath = Directory.GetCurrentDirectory()
                }
            };
        }
    }
}