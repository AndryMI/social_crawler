using System;
using System.Collections.Generic;

namespace Core.Crawling
{
    public class TaskProgress
    {
        private readonly Dictionary<ICommand, Item> items = new Dictionary<ICommand, Item>();

        public List<Item> Items => new List<Item>(items.Values);

        public void Add(CrawlerTask task)
        {
            if (!items.TryGetValue(task.Command, out var item))
            {
                items.Add(task.Command, item = new Item(task.Command));
            }
            item.Add(task);
        }

        public void Activate(CrawlerTask task)
        {
            if (items.TryGetValue(task.Command, out var item))
            {
                item.Activate(task);
            }
        }

        public void Complete(CrawlerTask task)
        {
            if (items.TryGetValue(task.Command, out var item))
            {
                item.Complete(task);
            }
        }

        public class Item
        {
            private readonly HashSet<CrawlerTask> active = new HashSet<CrawlerTask>();

            public ICommand Command { get; private set; }
            public DateTimeOffset AddedAt { get; private set; } = DateTimeOffset.UtcNow;
            public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;

            public int ActiveTasks { get; private set; }
            public int CompleteTasks { get; private set; }
            public int TotalTasks { get; private set; }

            public Item(ICommand command)
            {
                Command = command;
            }

            public void Add(CrawlerTask task)
            {
                TotalTasks++;
                UpdatedAt = DateTimeOffset.UtcNow;
            }

            public void Activate(CrawlerTask task)
            {
                active.Add(task);
                ActiveTasks = active.Count;
                UpdatedAt = DateTimeOffset.UtcNow;
            }

            public void Complete(CrawlerTask task)
            {
                active.Remove(task);
                CompleteTasks++;
                ActiveTasks = active.Count;
                UpdatedAt = DateTimeOffset.UtcNow;
            }
        }
    }
}
