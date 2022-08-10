using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Crawling
{
    public class TaskManager
    {
        private readonly object locker = new object();

        private readonly HashSet<CrawlerTask> tasks = new HashSet<CrawlerTask>();
        private readonly TaskProgress progress = new TaskProgress();

        public List<TaskProgress.Item> Progress
        {
            get { lock (locker) { return progress.Items; } }
        }

        public void Add(ICommand command)
        {
            lock (locker)
            {
                foreach (var task in command.CreateTasks())
                {
                    if (tasks.Add(task))
                    {
                        progress.Add(task);
                    }
                }
            }
        }

        public void Add(CrawlerTask task)
        {
            lock (locker)
            {
                if (tasks.Add(task))
                {
                    progress.Add(task);
                }
            }
        }

        public bool TryGet(Browser browser, out CrawlerTask task)
        {
            lock (locker)
            {
                task = tasks.Where(x => x.RunAt <= DateTimeOffset.UtcNow).OrderByDescending(x => x.Priority).FirstOrDefault();
                if (task != null)
                {
                    tasks.Remove(task);
                    progress.Activate(task);
                    return true;
                }
                return false;
            }
        }

        public void Retry(CrawlerTask task)
        {
            lock (locker)
            {
                task.RunAt = DateTimeOffset.UtcNow.AddSeconds(Config.Instance.RetryTimeout);
                if (tasks.Add(task))
                {
                    progress.Add(task);
                }
            }
        }

        public void Complete(CrawlerTask task)
        {
            lock (locker)
            {
                progress.Complete(task);
            }
        }
    }
}
