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
        private readonly UniqueFilter<CommentInfo> comment = new UniqueFilter<CommentInfo>(comment => comment.Link);
        private const int MaxPosts = 800;

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
                driver = browser.Driver<InstagramAccount>(task.Url);

                driver.Url = task.Url;
                driver.WaitForMain();
                Crawler.Sleep(this, "open");

                if (task.CrawlProfile)
                {
                    var profile = ProfileInfo.Collect(browser);
                    if (profile != null)
                    {
                        storage.StoreProfile(task, profile);
                    }
                }

                if (task.CrawlStories)
                {
                    driver.TryOpenStories();

                    while (driver.Url.Contains("/stories/"))
                    {
                        driver.WaitForStoryLoading();

                        var story = StoryInfo.Collect(browser);
                        if (story != null)
                        {
                            storage.StoreStory(task, story);
                        }

                        driver.FindElement(By.TagName("body")).SendKeys(Keys.ArrowRight);
                        Crawler.Sleep(this, "next story");
                    }
                }

                var postCount = 0;
                var postUrl = driver.Url;
                if (task.CrawlPosts)
                {
                    var post = driver.TryFindElement(By.CssSelector("article a"));
                    if (post == null)
                    {
                        //TODO posts not found
                        throw new Exception("Something wrong");
                    }
                    var href = post.GetDomAttribute("href");
                    if (href == null || !href.StartsWith("/p/"))
                    {
                        //TODO posts not found
                        throw new Exception("Something wrong");
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

                    var post = PostInfo.Collect(browser);
                    if (post != null)
                    {
                        storage.StorePost(task, post);
                    }

                    while (task.CrawlComments)
                    {
                        driver.WaitForCommentsLoading();

                        var comments = comment.Filter(CommentInfo.Collect(browser));
                        if (comments != null && comments.Length == 0)
                        {
                            break;
                        }
                        if (comments != null && comments.Length > 0)
                        {
                            storage.StoreComments(task, comments);
                        }
                        driver.ScrollToLastComment();
                        driver.LoadMoreComments();
                        Crawler.Sleep(this, "next comments");
                    }

                    if (postCount++ > MaxPosts)
                    {
                        break;
                    }
                    driver.FindElement(By.TagName("body")).SendKeys(Keys.ArrowRight);
                    driver.WaitForDialogLoading();
                    Crawler.Sleep(this, "next post");
                }
            }
            catch (Exception e)
            {
                throw new CrawlingException(e, task, driver);
            }
        }
    }
}
