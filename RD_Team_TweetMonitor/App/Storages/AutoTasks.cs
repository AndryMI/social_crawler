using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RD_Team_TweetMonitor
{
    public class AutoTasks : ProxyStorage
    {
        private readonly ConcurrentStack<TwitterCrawler.Task> tasks;

        public AutoTasks(ConcurrentStack<TwitterCrawler.Task> tasks, IStorage storage) : base(storage)
        {
            this.tasks = tasks;
        }

        public override void StoreException(CrawlingException exception)
        {
            base.StoreException(exception);

            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(Config.Instance.RetryTimeout));
                tasks.Push(exception.Task);
            });
        }

        public override void StoreTweets(string url, TweetInfo[] tweets)
        {
            base.StoreTweets(url, tweets);

            if (!url.Contains("/status/"))
            {
                foreach (var tweet in tweets)
                {
                    tasks.Push(new TwitterCrawler.Task
                    {
                        Url = tweet.Link,
                        Tweets = true,
                    });
                }
            }
        }
    }
}
