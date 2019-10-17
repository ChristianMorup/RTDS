using NSubstitute;
using NUnit.Framework;
using RTDS.Monitoring;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Monitors;

namespace RTDS.UnitTest.Monitoring
{
    [TestFixture]
    public class Test_MonitorController
    {
        private MonitorController _uut;
        private IMonitor _fakeFolderMonitor;
        private IMonitorFactory _fakeFactory;
        private IMonitor _fakeFileMonitor;

        [SetUp]
        public void SetUp()
        {
            _fakeFileMonitor = Substitute.For<IMonitor>();
            _fakeFolderMonitor = Substitute.For<IMonitor>();
            _fakeFactory = Substitute.For<IMonitorFactory>();
            //_uut = new MonitorController(_fakeFolderMonitor, _fakeFactory);
        }

        [Test]
        public void StartMonitoring_NewFolderIsAdded_NewFileMonitorIsCreated()
        {
            //Act:
            _uut.StartMonitoring("SomeValidPath");
            _fakeFolderMonitor.Created +=
                Raise.EventWith<SearchDirectoryArgs>(new object(), new SearchDirectoryArgs("SomePath", "SomeName", _fakeFileMonitor));

            //Assert:
            _fakeFactory.Received(1).CreateFileMonitor();
        }

        [Test]
        public void StartMonitoring_NoFolderIsAdded_FileMonitorIsNotCreated()
        {
            //Act:
            _uut.StartMonitoring("SomeValidPath");

            //Assert:
            _fakeFactory.DidNotReceive().CreateFileMonitor();
        }
    }
}   