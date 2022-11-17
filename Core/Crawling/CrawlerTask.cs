using Core.Storages;
using Newtonsoft.Json;
using System;

namespace Core.Crawling
{
    [Serializable]
    public abstract class CrawlerTask
    {
        public readonly ICommand Command;
        public readonly string Priority;
        public readonly string Url;

        public string Type => GetType().Name;

        public DateTimeOffset RunAt = DateTimeOffset.UtcNow;
        public uint RunFails = 0;

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

        public override int GetHashCode()
        {
            return Command.GetHashCode() ^ Url.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is CrawlerTask task)
            {
                return task.Command == Command && task.Url == Url;
            }
            return false;
        }

        public static string DefaultPriority => DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.000Z");
    }
}
