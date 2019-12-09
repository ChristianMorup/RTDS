using System;
using NUnit.Framework;
using RTDS.CBCTDataProvider.Monitoring.Factory;
using RTDS.DTO;

namespace RTDS.UnitTest.CBCTDataProvider.Monitoring.Factory
{
    [TestFixture]
    public class Test_ProjectonInfoFactory
    {
        private ProjectionInfoInfoFactory _uut;

        [SetUp]
        public void SetUp()
        {
            _uut = new ProjectionInfoInfoFactory();
        }

        [TestCase("source", 0)]
        [TestCase("source2", 1)]
        [TestCase("source3", 2)]
        [TestCase("source", 99)]
        public void CreateTempProjectionInfo_ValidCases_InfoIsCorrect(string source, int index)
        {
            //Act:
            var info = _uut.CreateTempProjectionInfo(source, index);

            //Assert:
            Assert.That(info.Id, Is.EqualTo(index));
            Assert.That(info.FilePath, Is.EqualTo(source));
        }

        [TestCase("target", 0)]
        [TestCase("target2", 1)]
        [TestCase("target3", 2)]
        [TestCase("target", 99)]
        public void CreatePermProjectionInfo_ValidCases_InfoIsCorrect(string target, int index)
        {
            //Act:
            var tempInfo = _uut.CreateTempProjectionInfo("Random source", index);

            var folderStructure = new PermStorageFolderStructure("base", target, "mhaPath", "ctPath");

            var permInfo = _uut.CreatePermProjectionInfo(folderStructure, tempInfo);

            //Assert:
            Assert.That(permInfo.Id, Is.EqualTo(index));
            Assert.That(permInfo.FileName, Is.EqualTo("proj" + index + ".xim"));
            Assert.That(permInfo.FilePath, Is.EqualTo(target + "\\" + "proj" + index + ".xim"));
        }

        [TestCase("", 0)]
        [TestCase(null, 0)]
        [TestCase("source", -1)]
        public void CreateProjectionInfo_InvalidCases_ThrowsException(string source, int index)
        {
            //Act + Assert:
            Assert.Throws<ArgumentException>(() => _uut.CreateTempProjectionInfo(source, index));
        }
    }
}