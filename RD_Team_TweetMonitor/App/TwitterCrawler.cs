using OpenQA.Selenium.Chrome;
using System;

namespace RD_Team_TweetMonitor
{
    public class TwitterCrawler
    {
        private readonly UniqueFilter<TweetInfo> unique = new UniqueFilter<TweetInfo>(64, tweet => tweet.Link);
        private readonly IStorage storage;

        public TwitterCrawler(IStorage storage)
        {
            this.storage = storage;
        }

        public void Run(Task task)
        {
            var exception = default(Exception);
            var driver = task.Driver ?? new ChromeDriver();
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
                    var tweets = unique.Filter(TweetInfo.Collect(driver));
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
            }
            catch (Exception e)
            {
                // Close browser before rethrow exception
                exception = new CrawlingException(e, task, driver);
            }
            finally
            {
                if (task.Driver == null)
                {
                    driver.Quit();
                }
            }
            throw exception;
        }

        public struct Task
        {
            public ChromeDriver Driver;
            public string Url;
            public bool Profile;
            public bool Tweets;
        }
    }
}
