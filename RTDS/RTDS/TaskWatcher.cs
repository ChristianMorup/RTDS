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
        private static readonly BlockingCollection<IErrorHandler> ErrorHandlers = new BlockingCollection<IErrorHandler>();

        public static Task WatchTask(Task t)
        {
            Tasks.Add(t);
            Task exceptionHandlingTask = new Task(async () =>
            {
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

            exceptionHandlingTask.Start();
            StartTaskRemover();
            return exceptionHandlingTask;
        }

        private static void StartTaskRemover()
        {
            Task task = new Task(async () =>
            {
                while (Tasks.Count > 0)
                {
                    Task finishedTask = await Task.WhenAny(Tasks);
                    while (Tasks.TryTake(out finishedTask)) ;
                }
            }, TaskCreationOptions.LongRunning);

            task.Start();
        }

        public static void AddTask(Task t)
        {
            Tasks.Add(t);
        }

        public static void AddErrorListener(IErrorHandler errorHandler)
        {
            ErrorHandlers.Add(errorHandler);
        }

        public static void RemoveErrorListener(IErrorHandler errorHandler)
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
