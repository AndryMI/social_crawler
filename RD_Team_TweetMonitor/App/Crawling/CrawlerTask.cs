
namespace RD_Team_TweetMonitor
{
    public enum TaskType
    {
        Anonymous,
        Authorized
    }

    public struct CrawlerTask
    {
        public TaskType Type;
        public string Url;

        public bool Profile;
        public bool Tweets;
        public bool Followers;

        public static CrawlerTask FromUrl(string url)
        {
            if (url.Contains("/followers") || url.Contains("/following"))
            {
                return new CrawlerTask
                {
                    Type = TaskType.Authorized,
                    Url = url,
                    Followers = true,
                };
            }
            if (url.Contains("/status/"))
            {
                return new CrawlerTask
                {
                    Type = TaskType.Anonymous,
                    Url = url,
                    Tweets = true,
                };
            }
            return new CrawlerTask
            {
                Type = TaskType.Anonymous,
                Url = url,
                Profile = true,
                Tweets = true,
            };
        }
    }
}
