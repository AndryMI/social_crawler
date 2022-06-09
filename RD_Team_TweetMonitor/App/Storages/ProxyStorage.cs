
namespace RD_Team_TweetMonitor
{
    public class ProxyStorage : IStorage
    {
        private readonly IStorage storage;

        public ProxyStorage(IStorage storage)
        {
            this.storage = storage;
        }

        public virtual void StoreException(CrawlingException exception)
        {
            storage.StoreException(exception);
        }

        public virtual void StoreProfile(ProfileInfo profile)
        {
            storage.StoreProfile(profile);
        }

        public virtual void StoreTweets(string url, TweetInfo[] tweets)
        {
            storage.StoreTweets(url, tweets);
        }
    }
}
