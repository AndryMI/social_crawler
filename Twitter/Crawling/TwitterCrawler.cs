using Core;
using Core.Crawling;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using Twitter.Data;

namespace Twitter.Crawling
{
    public class TwitterCrawler
    {
        private readonly UniqueFilter<TweetInfo> tweet = new UniqueFilter<TweetInfo>(64, tweet => tweet.Link);
        private readonly UniqueFilter<FollowersInfo> follower = new UniqueFilter<FollowersInfo>(64, follower => follower.Link);

        private readonly Browser browser;
        private readonly TwitterSotrage storage;
        private readonly TwitterTask task;

        public TwitterCrawler(Browser browser, TwitterSotrage storage, TwitterTask task)
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
                if (task.NeedAuthorization)
                {
                    var account = Accounts<TwitterAccount>.Instance.Get(task.Url);
                    account.Login(driver = browser.Driver(account.BrowserProfile));
                }
                else
                {
                    driver = browser.Driver();
                }

                driver.Url = task.Url;
                driver.WaitForLoading();

                if (task.CrawlProfile)
                {
                    var profile = ProfileInfo.Collect(driver);
                    if (profile != null)
                    {
                        storage.StoreProfile(profile);
                    }
                }

                while (task.CrawlTweets)
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

                while (task.CrawlFollowers)
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
