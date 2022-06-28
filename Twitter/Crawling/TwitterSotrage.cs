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

        public void StoreProfile(TwitterTask task, ProfileInfo profile)
        {
            storage.StoreData(task, new Uri(profile.Link), profile);
        }

        public void StoreTweets(TwitterTask task, TweetInfo[] tweets)
        {
            var uri = new Uri(task.Url);
            foreach (var tweet in tweets)
            {
                storage.StoreData(task, uri, tweet);
            }

            if (!task.Url.Contains("/status/"))
            {
                foreach (var tweet in tweets)
                {
                    tasks.AddUrl(tweet.Link, tweet.Time);
                }
            }
        }

        public void StoreFollowers(TwitterTask task, FollowersInfo[] followers)
        {
            var uri = new Uri(task.Url);
            foreach (var follower in followers)
            {
                storage.StoreData(task, uri, follower);
            }
        }

        public void StoreException(CrawlingException exception)
        {
            storage.StoreException(exception);
        }
    }
}
