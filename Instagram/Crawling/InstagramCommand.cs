using Core.Crawling;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace Instagram.Crawling
{
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
                //TODO Создать специальную задачу: открыть инсту, набрать keyword в поиск (мб в случайном порядке), собрать ссылки на результаты
                foreach (var keyword in Keywords)
                {
                    var query = HttpUtility.UrlEncode(Regex.Replace(keyword, @"\s+", ""));
                    yield return new InstagramTask("https://www.instagram.com/explore/tags/" + query, CrawlerTask.DefaultPriority, this);
                }
            }
        }
    }
}
