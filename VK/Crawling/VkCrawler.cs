using Core;
using Core.Crawling;
using VK.Data;
using OpenQA.Selenium.Chrome;
using System;

namespace VK.Crawling
{
    public class VkCrawler
    {
        private readonly UniqueFilter<PostInfo> post = new UniqueFilter<PostInfo>(64, post => post.Link);
        private readonly UniqueFilter<CommentInfo> comment = new UniqueFilter<CommentInfo>(64, comment => comment.Link);

        private readonly Browser browser;
        private readonly VkStorage storage;
        private readonly VkTask task;

        public VkCrawler(Browser browser, VkStorage storage, VkTask task)
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
                    var account = Accounts<VkAccount>.Instance.Get(task.Url);
                    account.Login(driver = browser.Driver(account.BrowserProfile));
                }
                else
                {
                    driver = browser.Driver();
                }

                driver.Url = task.Url;
                driver.SwitchToEnglish();
                driver.WaitForPageLayout();

                if (driver.IsPrivatePage())
                {
                    storage.OnPrivatePage(task);
                    return;
                }

                if (task.CrawlProfile)
                {
                    var profile = ProfileInfo.Collect(driver);
                    if (profile != null)
                    {
                        storage.StoreProfile(task, profile);
                    }
                }

                while (task.CrawlPosts)
                {
                    var posts = post.Filter(PostInfo.Collect(driver));
                    if (posts != null && posts.Length == 0)
                    {
                        return;
                    }
                    if (posts != null && posts.Length > 0)
                    {
                        storage.StorePosts(task, posts);
                    }
                    driver.ScrollToLoadMore();
                    driver.WaitForPostsLoading();
                }

                while (task.CrawlComments)
                {
                    var comments = comment.Filter(CommentInfo.Collect(driver));
                    if (comments != null && comments.Length == 0)
                    {
                        return;
                    }
                    if (comments != null && comments.Length > 0)
                    {
                        storage.StoreComments(task, comments);
                    }
                    driver.ScrollToNextReplies();
                    driver.WaitForRepliesLoading();
                }
            }
            catch (Exception e)
            {
                throw new CrawlingException(e, task, driver);
            }
        }
    }
}
