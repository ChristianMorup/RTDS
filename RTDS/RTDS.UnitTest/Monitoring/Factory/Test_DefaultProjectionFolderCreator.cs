using System;
using System.Collections.Generic;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using RTDS.DTO;
using RTDS.Monitoring.Factory;
using RTDS.Utility;

namespace RTDS.UnitTest.Monitoring.Factory
{
    [TestFixture]
    public class Test_DefaultProjectionFolderCreator
    {
        private IFolderCreator _fakeFolderCreator;
        private DefaultProjectionFolderCreator _uut;

        [SetUp]
        public void SetUp()
        {
            _fakeFolderCreator = Substitute.For<IFolderCreator>();
            _uut = new DefaultProjectionFolderCreator(_fakeFolderCreator);
        }


        [Test]
        public void CreateFolderStructure_CreatesProjectionFolderStructure_StructureIsReturned()
        {
            //Arrange:
            var defaultStructure = CreateFolderStructureForTestPurpose();
            _fakeFolderCreator.CreateFolderStructureForProjectionsAsync().Returns(defaultStructure);

            //Act:
            var returnedStructure = _uut.CreateFolderStructure().Result;

            //Assert: 
            Assert.That(defaultStructure, Is.EqualTo(returnedStructure));
        }

        [Test]
        public void CreateFolderStructure_CreatesFolderAsync_FolderCreatorReceivesCall()
        {
            //Arrange:
            var defaultStructure = CreateFolderStructureForTestPurpose();
            _fakeFolderCreator.CreateFolderStructureForProjectionsAsync().Returns(defaultStructure);

            //Act:
            var returnedStructure = _uut.CreateFolderStructure().Result;

            //Assert: 
            _fakeFolderCreator.Received().CreateFoldersAsync(defaultStructure);
        }

        private PermStorageFolderStructure CreateFolderStructureForTestPurpose()
        {
            return new PermStorageFolderStructure("Base", "Xim", "Mha", "CT");
        }
    }
}
