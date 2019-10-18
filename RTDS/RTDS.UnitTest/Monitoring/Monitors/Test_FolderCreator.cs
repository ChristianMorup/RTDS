using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using RTDS.Configuration;
using RTDS.Configuration.Data;
using RTDS.DTO;
using RTDS.Monitoring;
using RTDS.Utility;

namespace RTDS.UnitTest.Monitoring.Monitors
{
    //Se https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/configuration/index?highlight=settings&tabs=basicconfiguration&view=aspnetcore-3.0

    [TestFixture]
    public class Test_FolderCreator
    {
        private FolderCreator _uut;
        private IFileUtil _fakeFileUtil;

        [SetUp]
        public void SetUp()
        {
            _fakeFileUtil = Substitute.For<IFileUtil>();
            _uut = new FolderCreator(_fakeFileUtil);
        }

        [Test]
        public void CreateFolderStructureForProjectionsAsync_CreatesStructure_PathsAreCorrect()
        {
            //Arrange: 
            var baseTargetPath = "Test base path";
            var baseSourcePath = "Test target path";

            RTDSConfiguration configuration = new RTDSConfiguration
            {
                Paths = new RTDSPaths
                {
                    BaseTargetPath = baseTargetPath,
                    BaseSourcePath = baseSourcePath
                }
            };

            ConfigurationManager.OverrideConfiguration(configuration, false);

            //Act: 
            var structure = _uut.CreateFolderStructureForProjectionsAsync().Result;

            //Arrange: 
            Assert.That(structure.BasePath.Contains(baseTargetPath));
            Assert.That(structure.MhaPath.Contains(baseTargetPath));
            Assert.That(structure.XimPath.Contains(baseTargetPath));
            Assert.That(structure.MhaPath.Contains("mha"));
            Assert.That(structure.XimPath.Contains("xim"));
        }

        [Test]
        public void CreateFoldersAsync_CreatesFolderStructure_FileUtilIsCalled()
        {
            //Arrange: 
            var basePath = "base";
            var ximPath = "xim";
            var mhaPath = "mha";
            ProjectionFolderStructure structure = new ProjectionFolderStructure(basePath, ximPath, mhaPath);

            //Act: 
            var task = _uut.CreateFoldersAsync(structure);

            Task.WaitAll(task);
            _fakeFileUtil.Received().CreateFolderAsync(basePath);
            _fakeFileUtil.Received().CreateFolderAsync(ximPath);
            _fakeFileUtil.Received().CreateFolderAsync(mhaPath);

        }
    }
}