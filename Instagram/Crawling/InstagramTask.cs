﻿using Core.Crawling;
using Core.Storages;
using System;

namespace Instagram.Crawling
{
    [Serializable]
    public class InstagramSearchTask : InstagramTask
    {
        public readonly string[] Keywords;

        public InstagramSearchTask(string url, string[] keywords, string priority, ICommand command) : base(url, priority, command)
        {
            Keywords = keywords;
            NeedAuthorization = true;
        }

        public override void Run(Browser browser, IDataStorage storage, TaskManager tasks)
        {
            new InstagramSearch(browser, new InstagramStorage(storage, tasks), this).Run();
        }
    }

    [Serializable]
    public class PostCommentsTask : InstagramTask
    {
        public PostCommentsTask(string url, string priority, InstagramTask parent) : base(url, priority, parent) { }
    }

    [Serializable]
    public class PostProfileTask : InstagramTask
    {
        public PostProfileTask(string url, string priority, InstagramTask parent) : base(url, priority, parent)
        {
            CrawlPostsOnce = true;
        }
    }

    [Serializable]
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
                IsExplore = true;
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
        public bool IsExplore { get; protected set; }

        public bool NeedAuthorization { get; set; }

        public bool CrawlProfile { get; protected set; }
        public bool CrawlStories { get; protected set; }
        public bool CrawlPosts { get; protected set; }
        public bool CrawlPostsOnce { get; protected set; }
        public bool CrawlComments { get; protected set; }

        public override void Run(Browser browser, IDataStorage storage, TaskManager tasks)
        {
            new InstagramCrawler(browser, new InstagramStorage(storage, tasks), this).Run();
        }
    }
}
