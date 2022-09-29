using Core.Crawling;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;

namespace Facebook.Crawling
{
    public class FacebookCommand : ICommand
    {
        public string Id { get; private set; }
        public string Type { get; private set; }

        [JsonProperty("link", NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Link;

        [JsonProperty("keywords", NullValueHandling = NullValueHandling.Ignore)]
        public readonly string[] Keywords;

        public IEnumerable<CrawlerTask> CreateTasks()
        {
            if (Link != null && Link.StartsWith("https://www.facebook.com/"))
            {
                yield return new FacebookTask(Link, CrawlerTask.DefaultPriority, this);
            }
            if (Keywords != null)
            {
                var link = "https://www.facebook.com/search/posts/?q=" + HttpUtility.UrlEncode(string.Join(" ", Keywords));
                yield return new FacebookTask(link, CrawlerTask.DefaultPriority, this);
            }
        }
    }
}
