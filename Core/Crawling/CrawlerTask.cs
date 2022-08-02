using Core.Storages;
using Newtonsoft.Json;
using System;

namespace Core.Crawling
{
    public abstract class CrawlerTask
    {
        public readonly ICommand Command;
        public readonly string Priority;
        public readonly string Url;

        public DateTimeOffset RunAt = DateTimeOffset.UtcNow;

        public abstract void Run(Browser browser, IDataStorage storage, TaskManager tasks);

        public CrawlerTask(string url, string priority, ICommand command)
        {
            Command = command;
            Priority = priority;
            Url = url;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static string DefaultPriority => DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.000Z");
    }
}
