using Core.Crawling;
using Core.Storages;

namespace Twitter.Crawling
{
    public class TwitterTask : CrawlerTask
    {
        public TwitterTask(string url, string priority) : base(url, priority)
        {
            if (url.Contains("/followers") || url.Contains("/following"))
            {
                NeedAuthorization = true;
                CrawlFollowers = true;
                return;
            }
            if (!url.Contains("/status/"))
            {
                CrawlProfile = true;
            }
            CrawlTweets = true;
        }

        public readonly bool NeedAuthorization;
        public readonly bool CrawlProfile;
        public readonly bool CrawlTweets;
        public readonly bool CrawlFollowers;

        public override void Run(Browser browser, IStorage storage, TaskManager tasks)
        {
            new TwitterCrawler(browser, new TwitterStorage(storage, tasks), this).Run();
        }
    }
}
