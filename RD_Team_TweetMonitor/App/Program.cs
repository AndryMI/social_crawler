using System.Diagnostics;
using Newtonsoft.Json;

namespace RD_Team_TweetMonitor
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var crawler = new ProfileCrawler();
            crawler.OnProfile += profile =>
            {
                Debug.WriteLine("-----------");
                Debug.WriteLine(JsonConvert.SerializeObject(profile));
            };
            crawler.OnTweets += tweets =>
            {
                Debug.WriteLine("-----------");
                foreach (var tweet in tweets)
                {
                    Debug.WriteLine(JsonConvert.SerializeObject(tweet));
                }
            };
            // crawler.Run("https://twitter.com/elonmusk");
            crawler.Run("https://twitter.com/simonschreibt");
        }
    }
}
