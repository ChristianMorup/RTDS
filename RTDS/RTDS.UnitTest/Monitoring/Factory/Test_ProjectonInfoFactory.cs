using NUnit.Framework;
using RTDS.DTO;
using RTDS.Monitoring.Factory;

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

        [Test]
        public void CreateProjectionInfo_GivesStructure_CreatesInfoWithStructure()
        {
            //Arrange:
            ProjectionFolderStructure structure = new ProjectionFolderStructure("base", "xim", "mha");

            //Act:
            var info = _uut.CreateProjectionInfo(structure);

            //Arrange:
            Assert.That(info.Structure, Is.EqualTo(structure));
        }
    }
}