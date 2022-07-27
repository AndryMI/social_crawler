using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Crawling
{
    public class TaskManager
    {
        private readonly object locker = new object();
        private readonly List<CrawlerTask> tasks = new List<CrawlerTask>();
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
                    tasks.Add(task);
                    progress.Add(task);
                }
            }
        }

        public void Add(CrawlerTask task)
        {
            lock (locker)
            {
                tasks.Add(task);
                progress.Add(task);
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
                tasks.Add(task);
                progress.Add(task);
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
