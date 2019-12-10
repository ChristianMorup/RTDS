using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RTDS.ExceptionHandling
{
    internal class TaskWatcher
    {
        private static readonly List<Task> Tasks = new List<Task>();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly List<IErrorHandler> ErrorHandlers =
            new List<IErrorHandler>();

        private static bool _watching = false;
        private static Task _watchingTask;

        public static Task WatchTask(Task t)
        {
            Tasks.Add(t);
            if (!_watching)
            {
                _watching = true;
                StartWatchingTasks();
            }

            return _watchingTask;
        }

        private static void StartWatchingTasks()
        {
            _watchingTask = new Task(async () =>
            {
                while (Tasks.Count > 0)
                {
                    Task finishedTask = await Task.WhenAny(Tasks);

                    if (finishedTask.IsFaulted)
                    {
                        foreach (var handler in ErrorHandlers)
                        {
                            if (finishedTask.Exception != null) handler.OnFatalError(finishedTask.Exception.Message);
                            Logger.Fatal(finishedTask.Exception);
                        }
                        Tasks.Remove(finishedTask);
                    }
                    else if (finishedTask.IsCompleted)
                    {
                        Tasks.Remove(finishedTask);
                    }
                }

                _watching = false;
            }, TaskCreationOptions.LongRunning);

            _watchingTask.Start();
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
            ErrorHandlers.Remove(errorHandler);
        }

        public static bool HasSubscriber()
        {
            return ErrorHandlers.Count > 0;
        }
    }
}