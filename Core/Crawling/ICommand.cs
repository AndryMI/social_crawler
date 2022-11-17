using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Core.Crawling
{
    /// <summary>High-level crawling command. Creates low-level crawling tasks</summary>
    public interface ICommand
    {
        [JsonProperty("_id")]
        string Id { get; }

        [JsonProperty("type")]
        string Type { get; }

        IEnumerable<CrawlerTask> CreateTasks();
    }

    [Serializable]
    public class CancelCommand : ICommand
    {
        public string Id { get; private set; }
        public string Type { get; private set; }
        public IEnumerable<CrawlerTask> CreateTasks() => new CrawlerTask[0];
    }
}
