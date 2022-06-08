using OpenQA.Selenium.Chrome;
using System;

namespace RD_Team_TweetMonitor
{
    public class TwitterCrawler
    {
        private UniqueFilter<TweetInfo> unique = new UniqueFilter<TweetInfo>(64, tweet => tweet.Link);

        public event Action<ProfileInfo> OnProfile;
        public event Action<string, TweetInfo[]> OnTweets;

        public void Run(Task task)
        {
            var exception = default(Exception);
            var driver = new ChromeDriver();
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
                        OnProfile?.Invoke(profile);
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
                        OnTweets?.Invoke(task.Url, tweets);
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
                driver.Quit();
            }
            throw exception;
        }

        public struct Task
        {
            public string Url;
            public bool Profile;
            public bool Tweets;
        }
    }
}
