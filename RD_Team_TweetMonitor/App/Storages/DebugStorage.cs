using Newtonsoft.Json;
using System.Diagnostics;

namespace RD_Team_TweetMonitor
{
    public class DebugStorage : IStorage
    {
        public void StoreException(CrawlingException exception)
        {
            throw exception;
        }

        public void StoreProfile(ProfileInfo profile)
        {
            Debug.WriteLine("--- --- ---");
            Debug.WriteLine(JsonConvert.SerializeObject(profile));
        }

        public void StoreTweets(string url, TweetInfo[] tweets)
        {
            Debug.WriteLine("--- --- ---");
            Debug.WriteLine("URL: " + url);
            foreach (var tweet in tweets)
            {
                Debug.WriteLine(JsonConvert.SerializeObject(tweet));
            }
        }

        public void StoreFollowers(string url, FollowersInfo[] followers)
        {
            Debug.WriteLine("--- --- ---");
            Debug.WriteLine("URL: " + url);
            foreach (var follower in followers)
            {
                Debug.WriteLine(JsonConvert.SerializeObject(follower));
            }
        }
    }
}
