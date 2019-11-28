using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using RTDS;

namespace RTDS.UnitTest
{
    [TestFixture]
    public class Test_TaskWatcher
    {

        [Test]
        public void WatchTask_ThrowsExceptionOnFatalError_ExceptionIsCatched()
        {
            //Arrange:
            var fakeErrorHandler = Substitute.For<AbstractErrorHandler>();
            TaskWatcher.AddErrorListener(fakeErrorHandler);
            string message = "Test";

            //Act: 
            Task t = new Task(() => throw new Exception("Test"));
            Task watcherTask = TaskWatcher.WatchTask(t);
            t.Start();
            Task.WaitAll(watcherTask);
            
            //Assert:
            fakeErrorHandler.Received(1).OnFatalError(message);
        }



    }
}
