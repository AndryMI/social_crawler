using Core;
using Core.Crawling;
using Core.Storages;
using Twitter.Data;

namespace Twitter.Crawling
{
    public class TwitterStorage
    {
        private readonly IStorage storage;
        private readonly TaskManager tasks;

        public TwitterStorage(IStorage storage, TaskManager tasks)
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
            if (!task.Url.Contains("/status/"))
            {
                foreach (var tweet in tweets)
                {
                    storage.StorePost(task, tweet);
                    tasks.Add(new TwitterTask(tweet.Link, tweet.Time) { ProfileLink = task.Url });
                }
            }
            else
            {
                foreach (var tweet in tweets)
                {
                    tweet.ProfileLink = task.ProfileLink;
                    storage.StoreComment(task, tweet);
                }
            }
        }

        public void StoreFollowers(TwitterTask task, FollowersInfo[] followers)
        {
            //TODO store data
        }

        public void StoreException(CrawlingException exception)
        {
            storage.StoreException(exception);
        }
    }
}
