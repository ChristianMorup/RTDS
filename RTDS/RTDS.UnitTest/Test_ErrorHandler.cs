using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RTDS;

namespace RTDS.UnitTest
{
    [TestFixture]
    public class Test_ErrorHandler
    {
        private AbstractErrorHandler errorHandler;
        private BlockingCollection<AbstractErrorHandler> blockingCollection;

        [Test]
        public void AddErrorListener_AddSubscriber_SubscriberIsSuccessfullyAdded()
        {
            //Act
            TaskWatcher.AddErrorListener(errorHandler);

            //Assert
            Assert.That(TaskWatcher.HasSubscriber,Is.EqualTo(true));

        }
        //[Test]
        //public void RemoveSubscriber_RemoveErrorListener_SubscriberIsSuccessfullyRemoved()
        //{
        //    //Arrange
        //    TaskWatcher.AddErrorListener(errorHandler);

        //    //Act
        //    TaskWatcher.RemoveErrorListener(errorHandler);

        //    //Assert
        //    //Assert.That(TaskWatcher.RemoveErrorListener(errorHandler), Is.EqualTo(0));

        //}

        [Test]
        public void AddErrorListener_AddMultipleSubscribers_SubscribersIsSuccessfullyAdded()
        {
            //
            string message = "ERROR";

           
        }

    }
}
