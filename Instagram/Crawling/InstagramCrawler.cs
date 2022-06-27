using Core;
using Core.Crawling;
using Instagram.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Instagram.Crawling
{
    public class InstagramCrawler
    {
        private readonly UniqueFilter<CommentInfo> comment = new UniqueFilter<CommentInfo>(64, comment => comment.Link);

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

                if (task.CrawlProfile)
                {
                    var profile = ProfileInfo.Collect(driver);
                    if (profile != null)
                    {
                        storage.StoreProfile(profile);
                    }
                }

                var postUrl = driver.Url;
                if (task.CrawlPosts)
                {
                    var post = driver.TryFindElement(By.CssSelector("article a"));
                    if (post == null)
                    {
                        //TODO posts not found
                        return;
                    }
                    var href = post.GetDomAttribute("href");
                    if (href == null || !href.StartsWith("/p/"))
                    {
                        //TODO posts not found
                        return;
                    }
                    post.Click();
                    driver.WaitForDialogLoading();
                }
                while (task.CrawlPosts)
                {
                    if (postUrl == driver.Url)
                    {
                        break;
                    }
                    postUrl = driver.Url;

                    var post = PostInfo.Collect(driver);
                    if (post != null)
                    {
                        storage.StorePost(post);
                    }

                    while (task.CrawlComments)
                    {
                        driver.WaitForLoading();

                        var comments = comment.Filter(CommentInfo.Collect(driver));
                        if (comments != null && comments.Length == 0)
                        {
                            break;
                        }
                        if (comments != null && comments.Length > 0)
                        {
                            storage.StoreComments(comments);
                        }
                        driver.ScrollToLastComment();
                        driver.LoadMoreComments();
                    }

                    driver.FindElement(By.TagName("body")).SendKeys(Keys.ArrowRight);
                    driver.WaitForDialogLoading();
                }
            }
            catch (Exception e)
            {
                throw new CrawlingException(e, task, driver);
            }
        }
    }
}
