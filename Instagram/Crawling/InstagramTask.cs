using Core.Crawling;
using Core.Storages;
using System;

namespace Instagram.Crawling
{
    public class InstagramTask : CrawlerTask
    {
        public InstagramTask(string url, string priority, ICommand command) : base(url, priority, command)
        {
            var uri = new Uri(url);
            if (uri.LocalPath.StartsWith("/explore/"))
            {
                CrawlPosts = true;
                CrawlComments = true;
                return;
            }
            if (uri.LocalPath.StartsWith("/p/"))
            {
                //TODO not implemented (crawl post comments)
                return;
            }
            CrawlProfile = true;
            CrawlStories = true;
            CrawlPosts = true;
            CrawlComments = true;
        }

        public readonly bool CrawlProfile;
        public readonly bool CrawlStories;
        public readonly bool CrawlPosts;
        public readonly bool CrawlComments;

        public override void Run(Browser browser, IDataStorage storage, TaskManager tasks)
        {
            new InstagramCrawler(browser, new InstagramStorage(storage), this).Run();
        }
    }
}
