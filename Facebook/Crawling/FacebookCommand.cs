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
            //TODO asd
            yield break;
            //if (Link != null && Link.StartsWith("https://www.instagram.com/"))
            //{
            //    yield return new FacebookTask(Link, CrawlerTask.DefaultPriority, this);
            //}
            //if (Keywords != null)
            //{
            //    foreach (var keyword in Keywords)
            //    {
            //        var link = "https://www.instagram.com/explore/tags/" + HttpUtility.UrlEncode(keyword);
            //        yield return new FacebookTask(keyword, CrawlerTask.DefaultPriority, this);
            //    }
            //}
        }
    }
}
