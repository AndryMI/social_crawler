using Core;
using Core.Crawling;
using Core.Storages;
using System;
using Twitter.Data;

namespace Twitter.Crawling
{
    public class TwitterSotrage
    {
        private readonly IStorage storage;
        private readonly TaskManager tasks;

        public TwitterSotrage(IStorage storage, TaskManager tasks)
        {
            this.storage = storage;
            this.tasks = tasks;
        }

        public void StoreProfile(ProfileInfo profile)
        {
            storage.StoreData(new Uri(profile.Link), profile);
        }

        public void StoreTweets(string url, TweetInfo[] tweets)
        {
            var uri = new Uri(url);
            foreach (var tweet in tweets)
            {
                storage.StoreData(uri, tweet);
            }

            if (!url.Contains("/status/"))
            {
                foreach (var tweet in tweets)
                {
                    tasks.AddUrl(tweet.Link, tweet.Time);
                }
            }
        }

        public void StoreFollowers(string url, FollowersInfo[] followers)
        {
            var uri = new Uri(url);
            foreach (var follower in followers)
            {
                storage.StoreData(uri, follower);
            }
        }

        public void StoreException(CrawlingException exception)
        {
            storage.StoreException(exception);
        }
    }
}
