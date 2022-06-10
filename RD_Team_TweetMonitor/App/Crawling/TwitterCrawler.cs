using OpenQA.Selenium.Chrome;
using System;

namespace RD_Team_TweetMonitor
{
    public class TwitterCrawler
    {
        private readonly UniqueFilter<TweetInfo> tweet = new UniqueFilter<TweetInfo>(64, tweet => tweet.Link);
        private readonly UniqueFilter<FollowersInfo> follower = new UniqueFilter<FollowersInfo>(64, follower => follower.Link);
        private readonly IStorage storage;

        public TwitterCrawler(IStorage storage)
        {
            this.storage = storage;
        }

        public void Run(ChromeDriver driver, CrawlerTask task)
        {
            try
            {
                driver.Url = task.Url;
                driver.InitTimeouts();
                driver.WaitForLoading();

                if (task.Profile)
                {
                    var profile = ProfileInfo.Collect(driver);
                    if (profile != null)
                    {
                        storage.StoreProfile(profile);
                    }
                }

                while (task.Tweets)
                {
                    var tweets = tweet.Filter(TweetInfo.Collect(driver));
                    if (tweets != null && tweets.Length == 0)
                    {
                        return;
                    }
                    if (tweets != null && tweets.Length > 0)
                    {
                        storage.StoreTweets(task.Url, tweets);
                    }
                    driver.ScrollToLastArticle();
                    driver.WaitForLoading();
                }

                while (task.Followers)
                {
                    var followers = follower.Filter(FollowersInfo.Collect(driver));
                    if (followers != null && followers.Length == 0)
                    {
                        return;
                    }
                    if (followers != null && followers.Length > 0)
                    {
                        storage.StoreFollowers(task.Url, followers);
                    }
                    driver.ScrollToLastFollower();
                    driver.WaitForLoading();
                }
            }
            catch (Exception e)
            {
                throw new CrawlingException(e, task, driver);
            }
        }
    }
}
