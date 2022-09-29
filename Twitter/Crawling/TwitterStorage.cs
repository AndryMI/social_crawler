using Core.Crawling;
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
            if (task is PostCommentsTask)
            {
                foreach (var tweet in tweets)
                {
                    tweet.ProfileLink = task.ProfileLink;
                    storage.StoreComment(task, tweet);
                }
                return;
            }
            if (task is PostProfileTask)
            {
                foreach (var tweet in tweets)
                {
                    storage.StorePost(task, tweet);
                }
                return;
            }
            foreach (var tweet in tweets)
            {
                storage.StorePost(task, tweet);

                if (tweet.Reply > 0)
                {
                    tasks.Add(new PostCommentsTask(tweet.Link, tweet.Time, task));
                }
                if (task.IsSearch)
                {
                    tasks.Add(new PostProfileTask(tweet.ProfileLink, tweet.Time, task));
                }
            }
        }

        public void StoreFollowers(TwitterTask task, FollowersInfo[] followers)
        {
            //TODO store data
        }
    }
}
