using System.Collections.Concurrent;
using System.Threading;

namespace RD_Team_TweetMonitor
{
    public static class Program
    {
        public static void Main()
        {
            var storage = new FileStorage();
            var queue = new ConcurrentQueue<string>();

            var profile = new TwitterCrawler();
            profile.OnProfile += storage.StoreProfile;
            profile.OnTweets += storage.StoreTweets;

            profile.OnTweets += (url, tweets) =>
            {
                foreach (var tweet in tweets)
                {
                    queue.Enqueue(tweet.Link);
                }
            };

            var reply = new TwitterCrawler();
            reply.OnTweets += storage.StoreReplies;

            var main = new Thread(arg => profile.Run((TwitterCrawler.Task)arg));

            main.Start(new TwitterCrawler.Task
            {
                Url = "https://twitter.com/simonschreibt",
                Tweets = true,
                Profile = true,
            });

            while (main.IsAlive)
            {
                if (queue.TryDequeue(out var tweet))
                {
                    reply.Run(new TwitterCrawler.Task
                    {
                        Url = tweet,
                        Tweets = true,
                    });
                }
                main.Join(1000);
            }

            // crawler.Run("https://twitter.com/elonmusk");
            // profile.Run("https://twitter.com/simonschreibt");
        }
    }
}
