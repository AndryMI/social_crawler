using Core;
using Core.Crawling;
using Core.Storages;
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
        }

        public void StoreTweets(string url, TweetInfo[] tweets)
        {
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
        }

        public void StoreException(CrawlingException exception)
        {
            storage.StoreException(exception);
        }
    }
}
