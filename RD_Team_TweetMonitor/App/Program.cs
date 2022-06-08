using System.Collections.Concurrent;
using System.Threading;

namespace RD_Team_TweetMonitor
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var storage = new FileStorage();
            var queue = new ConcurrentQueue<string>();

            var profile = new ProfileCrawler();
            profile.OnProfile += storage.StoreProfile;
            profile.OnTweets += storage.StoreTweets;

            profile.OnTweets += (url, tweets) =>
            {
                foreach (var tweet in tweets)
                {
                    queue.Enqueue(tweet.Link);
                }
            };

            var reply = new ReplyCrawler();
            reply.OnTweets += storage.StoreReplies;

            var main = new Thread(arg => profile.Run(arg as string));

            main.Start("https://twitter.com/simonschreibt");

            while (main.IsAlive)
            {
                if (queue.TryDequeue(out var tweet))
                {
                    reply.Run(tweet);
                }
                main.Join(1000);
            }

            // crawler.Run("https://twitter.com/elonmusk");
            // profile.Run("https://twitter.com/simonschreibt");
        }
    }
}
