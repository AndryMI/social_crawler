using OpenQA.Selenium.Chrome;
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
                Url = "https://twitter.com/elonmusk",
                Tweets = true,
                Profile = true,
            });

            var driver = new ChromeDriver();

            while (main.IsAlive | queue.TryDequeue(out var tweet))
            {
                if (tweet != null)
                {
                    reply.Run(new TwitterCrawler.Task
                    {
                        Driver = driver,
                        Url = tweet,
                        Tweets = true,
                    });
                }
                main.Join(1000);
            }

            driver.Quit();
        }
    }
}
