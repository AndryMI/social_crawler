using Core;
using Core.Crawling;
using Instagram.Data;
using OpenQA.Selenium.Chrome;
using System;

namespace Instagram.Crawling
{
    public class InstagramCrawler
    {
        private readonly Browser browser;
        private readonly InstagramStorage storage;
        private readonly InstagramTask task;

        public InstagramCrawler(Browser browser, InstagramStorage storage, InstagramTask task)
        {
            this.browser = browser;
            this.storage = storage;
            this.task = task;
        }

        public void Run()
        {
            ChromeDriver driver = null;
            try
            {
                var account = Accounts<InstagramAccount>.Instance.Get(task.Url);
                account.Login(driver = browser.Driver(account.BrowserProfile));

                driver.Url = task.Url;
                driver.WaitForMain();

                var profile = ProfileInfo.Collect(driver);
                if (profile != null)
                {
                    storage.StoreProfile(profile);
                }

                System.Threading.Thread.Sleep(10000);

                //driver.Url = task.Url;
                //driver.WaitForLoading();

                //if (task.CrawlProfile)
                //{
                //    var profile = ProfileInfo.Collect(driver);
                //    if (profile != null)
                //    {
                //        storage.StoreProfile(profile);
                //    }
                //}

                //while (task.CrawlTweets)
                //{
                //    var tweets = tweet.Filter(TweetInfo.Collect(driver));
                //    if (tweets != null && tweets.Length == 0)
                //    {
                //        return;
                //    }
                //    if (tweets != null && tweets.Length > 0)
                //    {
                //        storage.StoreTweets(task.Url, tweets);
                //    }
                //    driver.ScrollToLastArticle();
                //    driver.WaitForLoading();
                //}

                //while (task.CrawlFollowers)
                //{
                //    var followers = follower.Filter(FollowersInfo.Collect(driver));
                //    if (followers != null && followers.Length == 0)
                //    {
                //        return;
                //    }
                //    if (followers != null && followers.Length > 0)
                //    {
                //        storage.StoreFollowers(task.Url, followers);
                //    }
                //    driver.ScrollToLastFollower();
                //    driver.WaitForLoading();
                //}
            }
            catch (Exception e)
            {
                throw new CrawlingException(e, task, driver);
            }
        }
    }
}
