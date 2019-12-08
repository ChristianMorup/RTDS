using System.IO;
using NSubstitute;
using NUnit.Framework;
using RTDS.Configuration.Data;
using RTDS.ExceptionHandling;

namespace RTDS.UnitTest
{
    [TestFixture]
    public class Test_RTDSImpl
    {
        private RTDSImpl _uut;
        private RTDSPaths _paths;

        [SetUp]
        public void SetUp()
        {
            _uut = new RTDSImpl();
            var configuration = CreateDefaultConfiguration();
            _paths = configuration.Paths;
            Configuration.ConfigurationManager.OverrideConfiguration(configuration, false);
        }

        [Test]
        public void StartMonitoring_NoSubscribers_ReturnsFalse()
        {
            //Act:
            var result = _uut.StartMonitoring();

            //Assert:
            Assert.That(result, Is.False);
        }
        
        [Test]
        public void StartMonitoring_HasSubscribers_ReturnsTrue()
        {
            //Arrannge:
            var fakeErrorHandler = Substitute.For<IErrorHandler>(); 

            //Act:
            _uut.SubscribeErrorHandler(fakeErrorHandler);
            var result = _uut.StartMonitoring();
            
            //Assert: 
            Assert.That(result, Is.True);

            //Clean up: 
            _uut.UnsubscribeErrorHandler(fakeErrorHandler);
        }

        [Test]
        public void StartMonitoring_UnsubscribeHandler_ErrorHandlerIsUnsubscribed()
        {
            //Arrange:
            var fakeErrorHandler = Substitute.For<IErrorHandler>();

            //Act:
            _uut.SubscribeErrorHandler(fakeErrorHandler);
            _uut.UnsubscribeErrorHandler(fakeErrorHandler);
            var result = _uut.StartMonitoring();

            //Assert:
            Assert.That(result, Is.False);
            Assert.That(TaskWatcher.HasSubscriber, Is.False);
        }

        private RTDSConfiguration CreateDefaultConfiguration()
        {
            return new RTDSConfiguration
            {
                Paths = new RTDSPaths
                {
                    BaseSourcePath = Directory.GetCurrentDirectory(),
                    BaseTargetPath = Directory.GetCurrentDirectory()
                }
            };
        }
    }
}
