using Core.Crawling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Instagram.Crawling
{
    [Serializable]
    public class InstagramCommand : ICommand
    {
        public string Id { get; private set; }
        public string Type { get; private set; }

        [JsonProperty("link", NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Link;

        [JsonProperty("keywords", NullValueHandling = NullValueHandling.Ignore)]
        public readonly string[] Keywords;

        public IEnumerable<CrawlerTask> CreateTasks()
        {
            if (Link != null && Link.StartsWith("https://www.instagram.com/"))
            {
                yield return new InstagramTask(Link, CrawlerTask.DefaultPriority, this);
            }
            if (Keywords != null)
            {
                yield return new InstagramSearchTask("https://www.instagram.com/", Keywords, CrawlerTask.DefaultPriority, this);
            }
        }
    }
}
