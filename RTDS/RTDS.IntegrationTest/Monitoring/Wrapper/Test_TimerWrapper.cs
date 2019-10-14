using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Internal;
using RTDS.Monitoring.Wrapper;
using Timer = System.Timers.Timer;

namespace RTDS.IntegrationTest.Monitoring.Wrapper
{
    [TestFixture]
    public class Test_TimerWrapper
    {
        private TimerWrapper _uut;
        private Timer _timer;

        [SetUp]
        public void SetUp()
        {
            _timer = new Timer();
            _uut = new TimerWrapper(_timer);
        }

        [Test]
        public void SetParameters_ParametersAreGiven_TimerUsesCorrectParameters()
        {
            //Arrange: 
            double interval = 5000;
            bool enabled = true;

            //Act:
            _uut.Interval = interval;
            _uut.Enabled = enabled;

            //Assert:
            Assert.That(_timer.Interval, Is.EqualTo(interval));
            Assert.That(_uut.Interval, Is.EqualTo(interval));

            Assert.That(_timer.Enabled, Is.EqualTo(enabled));
            Assert.That(_uut.Enabled, Is.EqualTo(enabled));
        }

        [Test]
        public void Enabled_TimerIsStarted_EventIsRaisedAtTimeout()
        {
            //Arrange:
            bool isExpired = false;
            _uut.Elapsed += (sender, args) => { isExpired = true; };
            _uut.Interval = 100;
            
            //Act: 
            _uut.Reset();
            Thread.Sleep(200); 
            
            //Assert:
            Assert.That(isExpired, Is.True);
        }

        [Test]
        public void Dispose_DisposesTimer_TimerIsDisposed()
        {
            //Arrange: 
            bool isDisposed = false;
            _timer.Disposed += (sender, args) => { isDisposed = true; };

            //Act:
            _uut.Dispose();

            //Assert:
            Assert.That(isDisposed, Is.True);
        }
    }
}
