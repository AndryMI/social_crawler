using Core.Storages;
using System;

namespace Core.Crawling
{
    public abstract class CrawlerTask
    {
        public readonly ICommand Command;
        public readonly string Priority;
        public readonly string Url;

        public DateTimeOffset RunAt = DateTimeOffset.UtcNow;

        public abstract void Run(Browser browser, IStorage storage, TaskManager tasks);

        public CrawlerTask(string url, string priority, ICommand command)
        {
            Command = command;
            Priority = priority;
            Url = url;
        }
    }
}
