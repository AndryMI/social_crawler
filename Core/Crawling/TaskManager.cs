using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Crawling
{
    public class TaskManager
    {
        private readonly object locker = new object();
        private readonly List<CrawlerTask> tasks = new List<CrawlerTask>();
        private readonly TaskFactory factory;

        public TaskManager(TaskFactory factory)
        {
            this.factory = factory;
        }

        public int Count
        {
            get
            {
                lock (locker)
                {
                    return tasks.Count;
                }
            }
        }

        public void AddUrl(string url, string priority = null)
        {
            if (priority == null)
            {
                priority = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.000Z");
            }
            Add(factory.CreateTask(url, priority));
        }

        public void Add(CrawlerTask task)
        {
            lock (locker)
            {
                tasks.Add(task);
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
            }
        }
    }
}
