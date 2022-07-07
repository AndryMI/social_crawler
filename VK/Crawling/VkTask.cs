using Core.Crawling;
using Core.Storages;

namespace VK.Crawling
{
    public class VkTask : CrawlerTask
    {
        public VkTask(string url, string priority) : base(url, priority)
        {
            NeedAuthorization = false;
            CrawlProfile = true;
            CrawlPosts = true;
        }

        public readonly bool NeedAuthorization;
        public readonly bool CrawlProfile;
        public readonly bool CrawlPosts;

        public override void Run(Browser browser, IStorage storage, TaskManager tasks)
        {
            new VkCrawler(browser, new VkStorage(storage, tasks), this).Run();
        }
    }
}
