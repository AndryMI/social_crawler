using Core;
using Core.Crawling;
using Facebook.Data;
using OpenQA.Selenium.Chrome;
using System;

namespace Facebook.Crawling
{
    public class FacebookCrawler
    {
        private readonly UniqueFilter<RelationInfo> relation = new UniqueFilter<RelationInfo>(relation => relation.TargetLink);
        private readonly UniqueFilter<CommentInfo> comment = new UniqueFilter<CommentInfo>(comment => comment.Link);
        private readonly UniqueFilter<PostInfo> post = new UniqueFilter<PostInfo>(post => post.Link);
        private const int PostsTreshold = 50;

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

                var requests = browser.DumpRequests(url => url.Contains("/graphql/"));

                driver.Url = task.Url;
                driver.WaitForMain(task.IsViewSource);
                Crawler.Sleep(this, "open");

                driver.DumpPrefetchedRequests();
                requests.WaitForComplete();

                if (task.CrawlProfile)
                {
                    var profile = ProfileInfo.Collect(browser);
                    if (profile != null)
                    {
                        storage.StoreProfile(task, profile);
                    }
                }

                while (task.CrawlRelations)
                {
                    var relations = relation.Filter(RelationInfo.Collect(browser));
                    if (relations != null && relations.Length == 0)
                    {
                        break;
                    }
                    if (relations != null && relations.Length > 0)
                    {
                        storage.StoreRelations(task, relations);
                    }
                    requests.ClearDump();
                    driver.FocusToWindow();
                    driver.ScrollToPageBottom();
                    requests.WaitForComplete();
                    Crawler.Sleep(this, "next relations");
                }

                var totalPosts = 0;
                while (task.CrawlPosts)
                {
                    var posts = post.Filter(PostInfo.Collect(browser));
                    if (posts != null && posts.Length == 0)
                    {
                        break;
                    }
                    if (posts != null && posts.Length > 0)
                    {
                        storage.StorePosts(task, posts);
                    }
                    totalPosts += posts.Length;
                    if (task.CrawlPostsOnce || totalPosts > PostsTreshold)
                    {
                        break;
                    }
                    requests.ClearDump();
                    driver.FocusToWindow();
                    driver.ScrollToPageBottom();
                    requests.WaitForComplete();
                    Crawler.Sleep(this, "next posts");
                }

                while (task.CrawlComments)
                {
                    var comments = comment.Filter(CommentInfo.Collect(browser));
                    if (comments != null && comments.Length == 0)
                    {
                        break;
                    }
                    if (comments != null && comments.Length > 0)
                    {
                        storage.StoreComments(task, comments);
                    }
                    requests.ClearDump();
                    driver.FocusToWindow();
                    driver.ScrollToPageBottom();
                    driver.LoadMoreComments();
                    requests.WaitForComplete();
                    Crawler.Sleep(this, "next comments");
                }
            }
            catch (Exception e)
            {
                throw new CrawlingException(e, task, driver);
            }
        }
    }
}
