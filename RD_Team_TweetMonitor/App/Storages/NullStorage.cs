
namespace RD_Team_TweetMonitor
{
    public class NullStorage : IStorage
    {
        public void StoreException(CrawlingException exception)
        {
        }

        public void StoreProfile(ProfileInfo profile)
        {
        }

        public void StoreTweets(string url, TweetInfo[] tweets)
        {
        }

        public void StoreFollowers(string url, FollowersInfo[] followers)
        {
        }
    }
}
