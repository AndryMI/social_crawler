using Core.Crawling;
using Core;
using OpenQA.Selenium.Chrome;
using System;

namespace Instagram.Crawling
{
    public class InstagramSearch
    {
        private readonly Browser browser;
        private readonly InstagramStorage storage;
        private readonly InstagramSearchTask task;

        public InstagramSearch(Browser browser, InstagramStorage storage, InstagramSearchTask task)
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
                driver = browser.Driver<InstagramAccount>();

                if (!driver.Url.StartsWith(task.Url))
                {
                    driver.Url = task.Url;
                    driver.WaitForMain();
                    Crawler.Sleep(this, "open");
                }

                foreach (var keyword in task.Keywords)
                {
                    var search = driver.TryFindSearchPanel();
                    driver.FocusToWindow();
                    driver.TryUntilExec(() =>
                    {
                        search.Click();
                        search.Clear();
                        search.SendKeys(keyword);
                    });
                    Crawler.Sleep(this, "search");

                    storage.StoreSearchResults(task, driver.CollectSearchLinks());
                }
            }
            catch (Exception e)
            {
                throw new CrawlingException(e, task, driver);
            }
        }
    }
}
