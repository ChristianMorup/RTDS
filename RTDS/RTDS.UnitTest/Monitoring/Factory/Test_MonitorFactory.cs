using NUnit.Framework;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Monitors;

namespace RTDS.UnitTest.Monitoring.Factory
{
    [TestFixture]
    public class Test_MonitorFactory
    {
        private MonitorFactory _uut;

        [SetUp]
        public void SetUp()
        {
            _uut = new MonitorFactory();
        }

        [Test]
        public void CreateFileMonitor_Creates_FileMonitorIsCreated()
        {
            //Act:
            var monitor = _uut.CreateFileMonitor(null);

            //Assert:
            Assert.That(monitor, Is.InstanceOf<FileMonitor>());
        }

        [Test]
        public void CreateFolderMonitor_Creates_FolderMonitorIsCreated()
        {
            //Act:
            var monitor = _uut.CreateFolderMonitor();

            //Assert:
            Assert.That(monitor, Is.InstanceOf<FolderMonitor>());
        }
    }
}