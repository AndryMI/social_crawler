using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RD_Team_TweetMonitor
{
    public class TaskManager
    {
        private readonly Dictionary<TaskType, ConcurrentStack<CrawlerTask>> tasks;

        public TaskManager()
        {
            tasks = new Dictionary<TaskType, ConcurrentStack<CrawlerTask>>();
            foreach (TaskType type in Enum.GetValues(typeof(TaskType)))
            {
                tasks[type] = new ConcurrentStack<CrawlerTask>();
            }
        }

        public int Count => tasks.Values.Sum(x => x.Count);

        public void Add(CrawlerTask task)
        {
            tasks[task.Type].Push(task);
        }

        public bool TryGet(TaskType type, out CrawlerTask task)
        {
            return tasks[type].TryPop(out task);
        }
    }
}
