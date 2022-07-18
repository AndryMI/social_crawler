using Core.Crawling;
using Core.Storages;
using System;

namespace VK.Crawling
{
    public class VkTask : CrawlerTask
    {
        public VkTask(string url, string priority) : base(url, priority)
        {
            var uri = new Uri(url);
            if (uri.LocalPath.StartsWith("/wall"))
            {
                CrawlComments = true;
                return;
            }
            CrawlProfile = true;
            CrawlPosts = true;
        }

        public bool NeedAuthorization;
        public readonly bool CrawlProfile;
        public readonly bool CrawlPosts;
        public readonly bool CrawlComments;

        public override void Run(Browser browser, IStorage storage, TaskManager tasks)
        {
            new VkCrawler(browser, new VkStorage(storage, tasks), this).Run();
        }
    }
}
