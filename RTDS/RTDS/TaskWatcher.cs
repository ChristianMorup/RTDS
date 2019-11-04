using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace RTDS
{
    public class TaskWatcher
    {
        private static readonly BlockingCollection<Task> Tasks = new BlockingCollection<Task>();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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
                }

            }, TaskCreationOptions.LongRunning);

            task.Start();
            return task;
        }

        public static void AddTask(Task t)
        {
            Tasks.Add(t);
        }
    }
}
