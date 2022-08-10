﻿using Core.Crawling;
using Core.Storages;
using Twitter.Data;

namespace Twitter.Crawling
{
    public class TwitterStorage
    {
        private readonly IDataStorage storage;
        private readonly TaskManager tasks;

        public TwitterStorage(IDataStorage storage, TaskManager tasks)
        {
            this.storage = storage;
            this.tasks = tasks;
        }

        public void StoreProfile(TwitterTask task, ProfileInfo profile)
        {
            storage.StoreProfile(task, profile);
        }

        public void StoreTweets(TwitterTask task, TweetInfo[] tweets)
        {
            if (task.Parent != null)
            {
                foreach (var tweet in tweets)
                {
                    tweet.ProfileLink = task.ProfileLink;
                    storage.StoreComment(task, tweet);
                }
            }
            else
            {
                foreach (var tweet in tweets)
                {
                    storage.StorePost(task, tweet);
                    tasks.Add(new TwitterTask(tweet.Link, tweet.Time, task));

                    if (task.IsSearch)
                    {
                        tasks.Add(new TwitterTask(tweet.ProfileLink, tweet.Time, task)
                        {
                            CrawlTweetsOnce = true
                        });
                    }
                }
            }
        }

        public void StoreFollowers(TwitterTask task, FollowersInfo[] followers)
        {
            //TODO store data
        }
    }
}
