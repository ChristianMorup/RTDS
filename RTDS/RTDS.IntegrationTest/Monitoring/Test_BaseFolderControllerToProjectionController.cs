using System;
using System.Collections.Generic;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using RTDS.Monitoring;
using RTDS.Monitoring.Factory;
using RTDS.Utility;

namespace RTDS.IntegrationTest.Monitoring
{
    [TestFixture()]
    public class Test_BaseFolderControllerToProjectionController
    {
        private BaseFolderController _baseFolderController;
        private FileController _fileController;
        private ProjectionController _projectionController;
        private IMonitorFactory _fakeMonitorFactory;
        private IProjectionFolderCreator _fakeFolderCreator;

        public void SetUp()
        {
            _fakeMonitorFactory = Substitute.For<IMonitorFactory>();
            _fakeFolderCreator = Substitute.For<IProjectionFolderCreator>();
            _fileController = new FileController(_fakeFolderCreator, _fakeMonitorFactory);
            _baseFolderController = new BaseFolderController(_fakeMonitorFactory, _fileController);
        }

        private void SetUpFakes()
        {
            _fakeMonitorFactory.CreateFileMonitorListener().Returns(CreateFileMonitorListener());
        }

        private FileMonitorListener CreateFileMonitorListener()
        {
            var factory = new ProjectionInfoFactory();
            var fileUtil = Substitute.For<IFileUtil>();
            _projectionController = new ProjectionController(factory, fileUtil);
            return new FileMonitorListener(_projectionController);
        }


    }
}
