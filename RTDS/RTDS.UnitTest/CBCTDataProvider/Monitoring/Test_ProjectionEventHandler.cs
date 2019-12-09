using System.Collections.Concurrent;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using RTDS.CBCTDataProvider.Monitoring.Factory;
using RTDS.CBCTDataProvider.Monitoring.Monitors;
using RTDS.CBCTDataProvider.ProjectionProcessing;
using RTDS.DTO;
using RTDS.Utility;

namespace RTDS.UnitTest.CBCTDataProvider.Monitoring
{
    [TestFixture]
    public class Test_ProjectionEventHandler
    {
        private IProjectionInfoFactory _fakeProjectionInfoFactory;
        private IMonitor _fakeRelatedMonitor;
        private BlockingCollection<TempProjectionInfo> _queue;

        private ProjectionEventHandler _uut;

        [SetUp]
        public void SetUp()
        {
            _fakeProjectionInfoFactory = Substitute.For<IProjectionInfoFactory>();
            _queue = new BlockingCollection<TempProjectionInfo>();
            _fakeRelatedMonitor = Substitute.For<IFileMonitor>();

            _uut = new ProjectionEventHandler(_fakeProjectionInfoFactory, new PermStorageFolderStructure("base", "xim", "mha", "Ct"), _queue);
        }

        [Test]
        public void HandleNewFile_FilesAreIndexed_IndexesAreCorrect()
        {
            for (int i = 0; i < 5; i++)
            {
                //Arrange:
                var projectionInfo = new TempProjectionInfo(1, "temp storage");
                _fakeProjectionInfoFactory.CreateTempProjectionInfo(Arg.Any<string>(), Arg.Any<int>())
                    .Returns(projectionInfo);
                
                //Act:
                Task task = _uut.HandleNewFile("Some path");
                Task.WaitAll(task);

                //Assert:
                _fakeProjectionInfoFactory.Received(1).CreateTempProjectionInfo(Arg.Any<string>(), i);
            }
        }

        [Test]
        public void HandleNewFile_FilesAreInQueue_QueueContainsFiles()
        {
            //Arrange: 
            var projectionInfo = new TempProjectionInfo(1, "temp storage");
            _fakeProjectionInfoFactory.CreateTempProjectionInfo(Arg.Any<string>(), Arg.Any<int>())
                .Returns(projectionInfo);

            //Act:
            Task task = _uut.HandleNewFile("Some path");
            Task.WaitAll(task);

            //Assert:
            Assert.That(_queue.Count, Is.EqualTo(1));
        }
    }
}