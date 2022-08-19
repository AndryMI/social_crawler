using Core.Crawling;
using Core.Storages;
using System;

namespace Instagram.Crawling
{
    public class PostCommentsTask : InstagramTask
    {
        public PostCommentsTask(string url, string priority, InstagramTask parent) : base(url, priority, parent) { }
    }

    public class InstagramTask : CrawlerTask
    {
        public InstagramTask(string url, string priority, InstagramTask parent) : this(url, priority, parent.Command)
        {
            Parent = parent;
        }

        public InstagramTask(string url, string priority, ICommand command) : base(url, priority, command)
        {
            var uri = new Uri(url);
            if (uri.LocalPath.StartsWith("/explore/"))
            {
                CrawlPosts = true;
                return;
            }
            if (uri.LocalPath.StartsWith("/p/"))
            {
                CrawlComments = true;
                return;
            }
            CrawlProfile = true;
            CrawlStories = false; //TODO update collect and enable
            CrawlPosts = true;
        }

        public readonly InstagramTask Parent;

        public bool CrawlProfile { get; protected set; }
        public bool CrawlStories { get; protected set; }
        public bool CrawlPosts { get; protected set; }
        public bool CrawlComments { get; protected set; }

        public override void Run(Browser browser, IDataStorage storage, TaskManager tasks)
        {
            new InstagramCrawler(browser, new InstagramStorage(storage, tasks), this).Run();
        }
    }
}
