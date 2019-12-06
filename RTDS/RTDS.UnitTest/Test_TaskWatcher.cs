using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace RTDS.UnitTest
{
    [TestFixture]
    public class Test_TaskWatcher
    {
        [Test]
        public void WatchTask_ThrowsExceptionOnFatalError_ExceptionIsCaught()
        {
            //Arrange:
            var fakeErrorHandler = Substitute.For<AbstractErrorHandler>();
            TaskWatcher.AddErrorListener(fakeErrorHandler);
            string message = "Test";

            //Act: 
            Task t = new Task(() => throw new Exception(message));
            Task watcherTask = TaskWatcher.WatchTask(t);
            t.Start();
            Task.WaitAll(watcherTask);
            
            //Assert:
            fakeErrorHandler.Received(1).OnFatalError(message);
        }
    }
}
