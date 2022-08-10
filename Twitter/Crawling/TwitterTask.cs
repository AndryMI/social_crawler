﻿using Core.Crawling;
using Core.Storages;
using System;

namespace Twitter.Crawling
{
    public class TwitterTask : CrawlerTask
    {
        public TwitterTask(string url, string priority, TwitterTask parent) : this(url, priority, parent.Command)
        {
            Parent = parent;
        }

        public TwitterTask(string url, string priority, ICommand command) : base(url, priority, command)
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
                IsSearch = true;
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
        public readonly bool IsSearch;

        public bool NeedAuthorization;
        public bool CrawlProfile;
        public bool CrawlTweets;
        public bool CrawlTweetsOnce;
        public bool CrawlFollowers;

        public string ProfileLink => Parent != null && !new Uri(Parent.Url).LocalPath.StartsWith("/search") ? TwitterUtils.ExtractProfileLink(Parent.Url) : null;

        public override void Run(Browser browser, IDataStorage storage, TaskManager tasks)
        {
            new TwitterCrawler(browser, new TwitterStorage(storage, tasks), this).Run();
        }
    }
}
