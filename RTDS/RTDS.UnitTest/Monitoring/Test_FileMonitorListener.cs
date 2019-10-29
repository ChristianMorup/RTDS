using System;
using NSubstitute;
using NUnit.Framework;
using RTDS.DTO;
using RTDS.Monitoring;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Monitors;

namespace RTDS.UnitTest.Monitoring
{
    [TestFixture]
    public class Test_FileMonitorListener
    {
        private IProjectionController _fakeProjectionController;
        private FileMonitorListener _uut;

        [SetUp]
        public void SetUp()
        {
            _fakeProjectionController = Substitute.For<IProjectionController>();
            _uut = new FileMonitorListener(_fakeProjectionController);
        }

        [Test]
        public void OnNewFileDetected_HandlesNewFile_ProjectionControllerIsCalled()
        {
            //Arrange:
            var path = "Path";
            var fileName = "FileName";
            var monitorInfo = new MonitorInfo(null, null, Guid.Empty);
            var arg = new SearchDirectoryArgs(path, fileName, monitorInfo);

            //Act: 
            _uut.OnNewFileDetected(new object(), arg);

            //Assert:
            _fakeProjectionController.Received(1).HandleNewFile(Arg.Any<MonitorInfo>(), path);
        }

        [Test]
        public void OnMonitorFinished_Unsubscribes_ProjectionControllerIsNotCalled()
        {
            //Arrange:
            var path = "Path";
            var fileName = "FileName";
            var monitorInfo = new MonitorInfo(null, null, Guid.Empty);
            var arg = new SearchDirectoryArgs(path, fileName, monitorInfo);

            var fakeMonitor = Substitute.For<IFileMonitor>();
            fakeMonitor.Finished += _uut.OnMonitorFinished;
            fakeMonitor.Created += _uut.OnNewFileDetected;

            //Act: 
            fakeMonitor.Finished += Raise.EventWith(new object(), new FileMonitorFinishedArgs(fakeMonitor));
            fakeMonitor.Created += Raise.EventWith(new object(), new SearchDirectoryArgs(path, fileName));

            //Assert:
            _fakeProjectionController.Received(0).HandleNewFile(Arg.Any<MonitorInfo>(), path);
        }
    }
}