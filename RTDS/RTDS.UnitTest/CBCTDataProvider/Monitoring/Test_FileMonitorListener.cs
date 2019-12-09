using System;
using NSubstitute;
using NUnit.Framework;
using RTDS.CBCTDataProvider.Monitoring;
using RTDS.CBCTDataProvider.Monitoring.Args;
using RTDS.CBCTDataProvider.Monitoring.Monitors;
using RTDS.CBCTDataProvider.ProjectionProcessing;
using RTDS.DTO;

namespace RTDS.UnitTest.Monitoring
{
    [TestFixture]
    public class Test_FileMonitorListener
    {
        private IProjectionEventHandler _fakeProjectionEventHandler;
        private SubfolderMonitorListener _uut;

        [SetUp]
        public void SetUp()
        {
            _fakeProjectionEventHandler = Substitute.For<IProjectionEventHandler>();
            _uut = new SubfolderMonitorListener(_fakeProjectionEventHandler);
        }

        [Test]
        public void OnNewFileDetected_HandlesNewFile_ProjectionControllerIsCalled()
        {
            //Arrange:
            var path = "Path";
            var fileName = "FileName";
            var arg = new SearchDirectoryArgs(path, fileName, null);

            //Act: 
            _uut.OnNewFileDetected(new object(), arg);

            //Assert:
            _fakeProjectionEventHandler.Received(1).HandleNewFile(path);
        }

        [Test]
        public void OnMonitorFinished_Unsubscribes_ProjectionControllerIsNotCalled()
        {
            //Arrange:
            var path = "Path";
            var fileName = "FileName";
            var arg = new SearchDirectoryArgs(path, fileName, null);

            var fakeMonitor = Substitute.For<IFileMonitor>();
            fakeMonitor.Finished += _uut.OnMonitorFinished;
            fakeMonitor.Created += _uut.OnNewFileDetected;

            //Act: 
            fakeMonitor.Finished += Raise.EventWith(new object(), new FileMonitorFinishedArgs(fakeMonitor));
            fakeMonitor.Created += Raise.EventWith(new object(), new SearchDirectoryArgs(path, fileName));

            //Assert:
            _fakeProjectionEventHandler.Received(0).HandleNewFile(path);
        }
    }
}