using System;
using System.Collections.Generic;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using RTDS.Configuration;
using RTDS.Monitoring;

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
            var expectedBasePath = ConfigurationManager.GetConfiguration("BaseTargetPath");

            //Act: 
            var structure = _uut.CreateFolderStructureForProjectionsAsync().Result;

            //Arrange: 
            Assert.That(structure.BasePath.Contains(expectedBasePath));
            Assert.That(structure.MhaPath.Contains(expectedBasePath));
            Assert.That(structure.XimPath.Contains(expectedBasePath));
            Assert.That(structure.MhaPath.Contains("mha"));
            Assert.That(structure.XimPath.Contains("xim"));
        }
    }
}
