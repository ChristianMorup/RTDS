using System.IO;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using RTDS.Configuration;
using RTDS.Configuration.Data;
using RTDS.DTO;
using RTDS.Utility;

namespace RTDS.UnitTest.Monitoring.Monitors
{
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
            var baseTargetPath = Directory.GetCurrentDirectory();
            var baseSourcePath = Directory.GetCurrentDirectory();

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
            var ctPath = "ct";
            PermStorageFolderStructure structure = new PermStorageFolderStructure(basePath, ximPath, mhaPath, ctPath);

            //Act: 
            var task = _uut.CreateFoldersAsync(structure);

            Task.WaitAll(task);
            _fakeFileUtil.Received().CreateFolderAsync(basePath);
            _fakeFileUtil.Received().CreateFolderAsync(ximPath);
            _fakeFileUtil.Received().CreateFolderAsync(mhaPath);
            _fakeFileUtil.Received().CreateFolderAsync(ctPath);

        }
    }
}