using Core.Crawling;
using Core.Storages;
using System;

namespace Facebook.Crawling
{
    public class PostCommentsTask : FacebookTask
    {
        public PostCommentsTask(string url, string priority, FacebookTask parent) : base(url, priority, parent) { }
    }

    public class FacebookTask : CrawlerTask
    {
        public FacebookTask(string url, string priority, FacebookTask parent) : this(url, priority, parent.Command)
        {
            Parent = parent;
        }

        public FacebookTask(string url, string priority, ICommand command) : base(url, priority, command)
        {
            //TODO asd
            //var uri = new Uri(url);
            //if (uri.LocalPath.StartsWith("/explore/"))
            //{
            //    CrawlPosts = true;
            //    return;
            //}
            //if (uri.LocalPath.StartsWith("/p/"))
            //{
            //    CrawlComments = true;
            //    return;
            //}
            //CrawlProfile = true;
            //CrawlStories = false; //TODO update collect and enable
            //CrawlPosts = true;
        }

        public readonly FacebookTask Parent;

        public bool CrawlProfile { get; protected set; }
        public bool CrawlStories { get; protected set; }
        public bool CrawlPosts { get; protected set; }
        public bool CrawlComments { get; protected set; }

        public override void Run(Browser browser, IDataStorage storage, TaskManager tasks)
        {
            new FacebookCrawler(browser, new FacebookStorage(storage, tasks), this).Run();
        }
    }
}
