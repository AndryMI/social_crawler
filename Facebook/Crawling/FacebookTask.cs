using Core.Crawling;
using Core.Storages;
using System;

namespace Facebook.Crawling
{
    [Serializable]
    public class RelationsTask : FacebookTask
    {
        public RelationsTask(string url, string priority, FacebookTask parent) : base(url, priority, parent)
        {
            CrawlRelations = true;
            CrawlProfile = false;
            CrawlPosts = false;
        }

        public static string ToUrl(string profileUrl)
        {
            var uri = new Uri(profileUrl);
            var separator = string.IsNullOrEmpty(uri.Query) ? "?" : "&";
            return "view-source:https://m.facebook.com" + uri.PathAndQuery + separator + "v=info";
        }
    }

    [Serializable]
    public class PostCommentsTask : FacebookTask
    {
        public PostCommentsTask(string url, string priority, FacebookTask parent) : base(url, priority, parent) { }
    }

    [Serializable]
    public class PostProfileTask : FacebookTask
    {
        public PostProfileTask(string url, string priority, FacebookTask parent) : base(url, priority, parent)
        {
            CrawlPostsOnce = true;
        }
    }

    [Serializable]
    public class FacebookTask : CrawlerTask
    {
        public FacebookTask(string url, string priority, FacebookTask parent) : this(url, priority, parent.Command)
        {
            Parent = parent;
        }

        public FacebookTask(string url, string priority, ICommand command) : base(url, priority, command)
        {
            var uri = new Uri(url);
            IsViewSource = uri.Scheme == "view-source";

            if (uri.LocalPath.StartsWith("/search/"))
            {
                CrawlPosts = true;
                IsSearch = true;
                return;
            }
            if (uri.LocalPath.Contains("/posts/"))
            {
                CrawlComments = true;
                return;
            }
            CrawlProfile = true;
            CrawlPosts = true;
        }

        public readonly FacebookTask Parent;
        public bool IsSearch { get; protected set; }
        public bool IsViewSource { get; protected set; }

        public bool CrawlProfile { get; protected set; }
        public bool CrawlRelations { get; protected set; }
        public bool CrawlPosts { get; protected set; }
        public bool CrawlPostsOnce { get; protected set; }
        public bool CrawlComments { get; protected set; }

        public override void Run(Browser browser, IDataStorage storage, TaskManager tasks)
        {
            new FacebookCrawler(browser, new FacebookStorage(storage, tasks), this).Run();
        }
    }
}
