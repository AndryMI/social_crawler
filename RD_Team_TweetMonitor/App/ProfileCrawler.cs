using OpenQA.Selenium.Chrome;
using System;

namespace RD_Team_TweetMonitor
{
    public class ProfileCrawler
    {
        private UniqueFilter<TweetInfo> unique = new UniqueFilter<TweetInfo>(64, tweet => tweet.Link);

        public event Action<ProfileInfo> OnProfile;
        public event Action<string, TweetInfo[]> OnTweets;

        public void Run(string url)
        {
            var exception = default(Exception);
            var driver = new ChromeDriver();
            try
            {
                driver.Url = url;
                driver.InitTimeouts();
                driver.WaitForLoading();

                var profile = ProfileInfo.Collect(driver);
                if (profile != null)
                {
                    OnProfile?.Invoke(profile);
                }

                while (true)
                {
                    var tweets = unique.Filter(TweetInfo.Collect(driver));
                    if (tweets != null && tweets.Length == 0)
                    {
                        return;
                    }
                    if (tweets != null && tweets.Length > 0)
                    {
                        OnTweets?.Invoke(url, tweets);
                    }
                    driver.ScrollToLastArticle();
                    driver.WaitForLoading();
                }
            }
            catch (Exception e)
            {
                // Close browser before rethrow exception
                exception = new CrawlingException(e, url, driver);
            }
            finally
            {
                driver.Quit();
            }
            throw exception;
        }
    }
}
