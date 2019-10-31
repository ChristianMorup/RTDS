using System;
using NSubstitute;
using NUnit.Framework;
using RTDS.DTO;
using RTDS.Monitoring.Factory;
using RTDS.Utility;

namespace RTDS.UnitTest.Monitoring.Factory
{
    [TestFixture]
    public class Test_ProjectonInfoFactory
    {
        private ProjectionInfoFactory _uut;

        [SetUp]
        public void SetUp()
        {
            _uut = new ProjectionInfoFactory();
        }

        [TestCase("target", "source", 0)]
        [TestCase("target2", "source2", 1)]
        [TestCase("target3", "source3", 2)]
        [TestCase("target", "source", 99)]
        public void CreateProjectionInfo_ValidCases_InfoIsCorrect(string target, string source, int index)
        {
            //Act:
            var info = _uut.CreateProjectionInfo(target, source, index);

            //Assert:
            Assert.That(info.Id, Is.EqualTo(index));
            Assert.That(info.FileName, Is.EqualTo("proj" + index + ".xim"));
            Assert.That(info.TempStoragePath, Is.EqualTo(source));
            Assert.That(info.PermanentStoragePath.Contains(target));
        }

        [TestCase("", "source", 0)]
        [TestCase("target", "", 0)]
        [TestCase(null, "source", 0)]
        [TestCase("target", null, 0)]
        [TestCase("target", "source", -1)]
        public void CreateProjectionInfo_InvalidCases_ThrowsException(string target, string source, int index)
        {
            //Act + Assert:
            Assert.Throws<ArgumentException>(() =>_uut.CreateProjectionInfo(target, source, index));
        }
    }
}