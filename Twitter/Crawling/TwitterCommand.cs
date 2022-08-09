using Core.Crawling;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;

namespace Twitter.Crawling
{
    public class TwitterCommand : ICommand
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("uid")]
        public string Uid;

        [JsonProperty("link")]
        public string Link;

        [JsonProperty("keywords")]
        public string[] Keywords;

        public IEnumerable<CrawlerTask> CreateTasks()
        {
            if (Link != null)
            {
                yield return new TwitterTask(Link, CrawlerTask.DefaultPriority, this);
            }
            if (Keywords != null)
            {
                var link = "https://twitter.com/search?q=" + HttpUtility.UrlEncode(string.Join(" ", Keywords));
                yield return new TwitterTask(link, CrawlerTask.DefaultPriority, this);

                link += "&f=user";
                yield return new TwitterTask(link, CrawlerTask.DefaultPriority, this);
            }
        }
    }
}
