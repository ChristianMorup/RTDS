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
        private string _basePath = "base";
        private string _ximPath = "xim";
        private string _mhaPath = "mha";
        private IProjectionFactory _fakeProjectionFactory;
        private IFolderCreator _fakeFolderCreator;
        private IFileUtil _fakeFileUtil;
        private IMonitor _fakeRelatedMoniter;
        private Dictionary<Guid, ProjectionInfo> _dictionary;
        private ProjectionController _uut;

        [SetUp]
        public void SetUp()
        {
            _fakeProjectionFactory = Substitute.For<IProjectionFactory>();
            _fakeFolderCreator = Substitute.For<IFolderCreator>();
            _fakeFileUtil = Substitute.For<IFileUtil>();
            _fakeRelatedMoniter = Substitute.For<IMonitor>();
            _dictionary = new Dictionary<Guid, ProjectionInfo>();
            _uut = new ProjectionController(_fakeFolderCreator, _fakeProjectionFactory, _fakeFileUtil);
        }

        [Test]
        public void HandleNewFile_CopiesFile_FileUtilIsCalledWithCorrectParameters()
        {
            //Arrange:
            var path = "Some/path";
            var info = CreateProjectionInfo();
            var guid = Guid.NewGuid();
            _fakeRelatedMoniter.Guid.Returns(guid);
            _dictionary.Add(guid, info);

            //Act:
            Task task = _uut.HandleNewFile(_fakeRelatedMoniter, path, _dictionary);
            Task.WaitAll(task);

            //Assert:
            var destFilePath = Path.Combine(_ximPath, Path.GetFileName(path));
            _fakeFileUtil.Received().CopyFileAsync(path, destFilePath);
        }

        [Test]
        public void HandleNewFile_NoMonitorInMap_FileUtilIsNotCalled()
        {
            //Arrange:
            var path = "Some/path";
            var info = CreateProjectionInfo();
            var guid = Guid.NewGuid();
            _fakeRelatedMoniter.Guid.Returns(guid);


            //Act:
            Task task = _uut.HandleNewFile(_fakeRelatedMoniter, path, _dictionary);
            Task.WaitAll(task);

            //Assert:
            var destFilePath = Path.Combine(_ximPath, Path.GetFileName(path));
            _fakeFileUtil.DidNotReceiveWithAnyArgs().CopyFileAsync(Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public async Task CreateProjectionInfo_CreatesFolderStructure_FolderCreatorIsCalled()
        {
            //Act: 
            var projectionInfo = await _uut.CreateProjectionInfo();

            //Assert:
            _fakeFolderCreator.Received(1).CreateFolderStructureForProjectionsAsync();
            _fakeFolderCreator.Received(1).CreateFoldersAsync(Arg.Any<ProjectionFolderStructure>());
        }

        [Test]
        public void CreateProjectionInfo_CreatesProjectionInfo_ProjectionInfoIsCorrect()
        {
            //Arrange: 
            var info = CreateProjectionInfo();
            _fakeFolderCreator.CreateFolderStructureForProjectionsAsync().Returns(info.Structure);
            _fakeProjectionFactory.CreateProjectionInfo(info.Structure);

            //Act:
            var result = _uut.CreateProjectionInfo().Result;

            //Assert:
            Assert.That(result, Is.EqualTo(result));
        }


        private ProjectionInfo CreateProjectionInfo()
        {
            var structure = new ProjectionFolderStructure(_basePath, _ximPath, _mhaPath);

            return new ProjectionInfo(structure);
        }
    }
}