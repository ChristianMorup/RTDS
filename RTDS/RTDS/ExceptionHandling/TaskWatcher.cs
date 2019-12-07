using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RTDS.ExceptionHandling
{
    internal class TaskWatcher
    {
        private static readonly BlockingCollection<Task> Tasks = new BlockingCollection<Task>();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly BlockingCollection<IErrorHandler> ErrorHandlers = new BlockingCollection<IErrorHandler>();
        private static bool _watching = false;
        private static Task _exceptionHandlingTask;

        public static Task WatchTask(Task t)
        {
            Tasks.Add(t);
            if (!_watching)
            {
                _watching = true;
                _exceptionHandlingTask = new Task(async () =>
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

                _exceptionHandlingTask.Start();
                StartTaskRemover();
            }

            return _exceptionHandlingTask;
        }

        private static void StartTaskRemover()
        {
            Task task = new Task(async () =>
            {
                while (Tasks.Count > 0)
                {
                    Task finishedTask = await Task.WhenAny(Tasks);
                    if (finishedTask.IsCompleted)
                    {
                        while (Tasks.TryTake(out finishedTask)) ;
                    }
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
