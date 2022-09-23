using Core;
using Core.Crawling;
using Facebook.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Facebook.Crawling
{
    public class FacebookCrawler
    {
        private readonly UniqueFilter<CommentInfo> comment = new UniqueFilter<CommentInfo>(comment => comment.Link);
        private readonly UniqueFilter<PostInfo> post = new UniqueFilter<PostInfo>(post => post.Link);
        private const int PostsTreshold = 300;

        private readonly Browser browser;
        private readonly FacebookStorage storage;
        private readonly FacebookTask task;

        public FacebookCrawler(Browser browser, FacebookStorage storage, FacebookTask task)
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
                driver = browser.Driver<FacebookAccount>(task.Url);

                driver.Url = task.Url;
                driver.WaitForMain();
                Crawler.Sleep(this, "open");

                //if (task.CrawlProfile)
                //{
                //    var profile = ProfileInfo.Collect(browser);
                //    if (profile != null)
                //    {
                //        storage.StoreProfile(task, profile);
                //    }
                //}

                //if (task.CrawlStories)
                //{
                //    driver.TryOpenStories();

                //    while (driver.Url.Contains("/stories/"))
                //    {
                //        driver.WaitForStoryLoading();

                //        var story = StoryInfo.Collect(browser);
                //        if (story != null)
                //        {
                //            storage.StoreStory(task, story);
                //        }

                //        driver.FindElement(By.TagName("body")).SendKeys(Keys.ArrowRight);
                //        Crawler.Sleep(this, "next story");
                //    }
                //}

                //var totalPosts = 0;
                //while (task.CrawlPosts)
                //{
                //    var posts = post.Filter(PostInfo.Collect(browser));
                //    if (posts != null && posts.Length == 0)
                //    {
                //        break;
                //    }
                //    if (posts != null && posts.Length > 0)
                //    {
                //        storage.StorePosts(task, posts);
                //    }
                //    totalPosts += posts.Length;
                //    if (totalPosts > PostsTreshold)
                //    {
                //        break;
                //    }
                //    driver.ScrollToPageBottom();
                //    driver.WaitForPostsLoading();
                //    Crawler.Sleep(this, "next posts");
                //}

                //while (task.CrawlComments)
                //{
                //    var comments = comment.Filter(CommentInfo.Collect(browser));
                //    if (comments != null && comments.Length == 0)
                //    {
                //        break;
                //    }
                //    if (comments != null && comments.Length > 0)
                //    {
                //        storage.StoreComments(task, comments);
                //    }
                //    driver.LoadMoreComments();
                //    driver.WaitForCommentsLoading();
                //    Crawler.Sleep(this, "next comments");
                //}
                //TODO asd
            }
            catch (Exception e)
            {
                throw new CrawlingException(e, task, driver);
            }
        }
    }
}
