using System.IO;
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
            _fakeFileController.Received(1).StartNewFileMonitorInNewFolderAsync(filePath, fileName);
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