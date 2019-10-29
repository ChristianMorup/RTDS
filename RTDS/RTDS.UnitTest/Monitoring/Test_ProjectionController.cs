using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using RTDS.DTO;
using RTDS.Monitoring;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Monitors;
using RTDS.Utility;

namespace RTDS.UnitTest.Monitoring
{
    [TestFixture]
    public class Test_ProjectionController
    {
        private IProjectionFactory _fakeProjectionFactory;
        private IFileUtil _fakeFileUtil;
        private IMonitor _fakeRelatedMonitor;

        private ProjectionController _uut;

        [SetUp]
        public void SetUp()
        {
            _fakeProjectionFactory = Substitute.For<IProjectionFactory>();
            _fakeFileUtil = Substitute.For<IFileUtil>();
            _fakeRelatedMonitor = Substitute.For<IFileMonitor>();

            _uut = new ProjectionController(_fakeProjectionFactory, _fakeFileUtil);
        }

        private MonitorInfo CreateDefaultMonitorInfo()
        {
            var structure = new ProjectionFolderStructure("basePath", "ximPath", "mhaPath");
            return new MonitorInfo(structure, _fakeRelatedMonitor, Guid.NewGuid());
        }

        [Test]
        public void HandleNewFile_FilesAreIndexed_IndexesAreCorrect()
        {
            for (int i = 0; i < 5; i++)
            {
                //Act:
                Task task = _uut.HandleNewFile(CreateDefaultMonitorInfo(), "Some path");
                Task.WaitAll(task);

                //Assert:
                _fakeProjectionFactory.Received(1).CreateProjectionInfo(Arg.Any<string>(), Arg.Any<string>(), i);
            }
        }

        [Test]
        public void HandleNewFile_FilesGetsCopied_FileUtilIsCalled()
        {
            //Arrange:
            var projectionInfo = new ProjectionInfo(1, "perm storage", "temp storage", "some name");
            _fakeProjectionFactory.CreateProjectionInfo(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>())
                .Returns(projectionInfo);

            //Act:
            Task task = _uut.HandleNewFile(CreateDefaultMonitorInfo(), "Some path");
            Task.WaitAll(task);

            //Assert:
            _fakeFileUtil.Received(1)
                .CopyFileAsync(projectionInfo.TempStoragePath, projectionInfo.PermanentStoragePath);
        }

        [Test]
        public void HandleNewFile_FilesAreInQueue_QueueContainsFiles()
        {
            //Arrange: 
            var projectionInfo = new ProjectionInfo(1, "perm storage", "temp storage", "some name");
            _fakeProjectionFactory.CreateProjectionInfo(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>())
                .Returns(projectionInfo);

            //Act:
            Task task = _uut.HandleNewFile(CreateDefaultMonitorInfo(), "Some path");
            Task.WaitAll(task);

            //Assert:
            Assert.That(_uut.GetQueue().Count, Is.EqualTo(1));
        }
    }
}