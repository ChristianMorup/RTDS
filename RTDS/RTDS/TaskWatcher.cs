using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace RTDS
{
    internal class TaskWatcher
    {
        private static readonly BlockingCollection<Task> Tasks = new BlockingCollection<Task>();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly BlockingCollection<AbstractErrorHandler> ErrorHandlers = new BlockingCollection<AbstractErrorHandler>();

        public static Task WatchTask(Task t)
        {
            Task task = new Task(async () =>
            {
                Tasks.Add(t);

                try
                {
                    await Task.WhenAll(Tasks);
                }
                catch (Exception e)
                {
                    Logger.Fatal(e);

                    foreach (var handler in ErrorHandlers)
                    {
                        handler.OnFatalError(e.Message);
                    }
                }

            }, TaskCreationOptions.LongRunning);

            task.Start();
            return task;
        }

        public static void AddTask(Task t)
        {
            Tasks.Add(t);
        }

        public static void AddErrorListener(AbstractErrorHandler errorHandler)
        {
            ErrorHandlers.Add(errorHandler);
        }

        public static void RemoveErrorListener(AbstractErrorHandler errorHandler)
        {
            if (errorHandler == null) throw new ArgumentNullException(nameof(errorHandler));
            while (ErrorHandlers.TryTake(out errorHandler)) ;
        }

        public static bool HasSubscriber()
        {
            return ErrorHandlers.Count > 0;
        }
    }
}
