using Core.Crawling;
using Core.Storages;
using System;

namespace Twitter.Crawling
{
    public class TwitterTask : CrawlerTask
    {
        public TwitterTask(string url, string priority, TwitterTask parent) : this(url, priority)
        {
            Parent = parent;
        }

        public TwitterTask(string url, string priority) : base(url, priority)
        {
            var uri = new Uri(url);
            if (uri.LocalPath.EndsWith("/followers") || uri.LocalPath.EndsWith("/following"))
            {
                NeedAuthorization = true;
                CrawlFollowers = true;
                return;
            }
            if (uri.LocalPath.StartsWith("/search"))
            {
                CrawlTweets = true;
                CrawlFollowers = true;
                return;
            }
            if (!uri.LocalPath.Contains("/status/"))
            {
                CrawlProfile = true;
            }
            CrawlTweets = true;
        }

        public readonly TwitterTask Parent;

        public readonly bool NeedAuthorization;
        public readonly bool CrawlProfile;
        public readonly bool CrawlTweets;
        public readonly bool CrawlFollowers;

        public string ProfileLink => Parent != null && !new Uri(Parent.Url).LocalPath.StartsWith("/search") ? TwitterUtils.ExtractProfileLink(Parent.Url) : null;

        public override void Run(Browser browser, IStorage storage, TaskManager tasks)
        {
            new TwitterCrawler(browser, new TwitterStorage(storage, tasks), this).Run();
        }
    }
}
