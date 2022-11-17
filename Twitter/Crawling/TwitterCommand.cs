using Core.Crawling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;

namespace Twitter.Crawling
{
    [Serializable]
    public class TwitterCommand : ICommand
    {
        public string Id { get; private set; }
        public string Type { get; private set; }

        [JsonProperty("link", NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Link;

        [JsonProperty("keywords", NullValueHandling = NullValueHandling.Ignore)]
        public readonly string[] Keywords;

        public IEnumerable<CrawlerTask> CreateTasks()
        {
            if (Link != null && Link.StartsWith("https://twitter.com/"))
            {
                yield return new TwitterTask(Link, CrawlerTask.DefaultPriority, this);
            }
            if (Keywords != null)
            {
                var link = "https://twitter.com/search?q=" + HttpUtility.UrlEncode(string.Join(" ", Keywords));
                yield return new TwitterTask(link, CrawlerTask.DefaultPriority, this);

                //TODO after followers
                //yield return new TwitterTask(link + "&f=user", CrawlerTask.DefaultPriority, this);
            }
        }
    }
}
