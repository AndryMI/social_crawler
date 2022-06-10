using System;
using System.Threading.Tasks;

namespace RD_Team_TweetMonitor
{
    public class AutoTasks : ProxyStorage
    {
        private readonly TaskManager tasks;

        public AutoTasks(TaskManager tasks, IStorage storage) : base(storage)
        {
            this.tasks = tasks;
        }

        public override void StoreException(CrawlingException exception)
        {
            base.StoreException(exception);

            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(Config.Instance.RetryTimeout));
                tasks.Add(exception.Task);
            });
        }

        public override void StoreTweets(string url, TweetInfo[] tweets)
        {
            base.StoreTweets(url, tweets);

            if (!url.Contains("/status/"))
            {
                foreach (var tweet in tweets)
                {
                    tasks.Add(CrawlerTask.FromUrl(tweet.Link));
                }
            }
        }
    }
}
