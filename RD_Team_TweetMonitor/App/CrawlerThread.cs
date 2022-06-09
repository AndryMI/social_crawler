using OpenQA.Selenium.Chrome;
using System.Collections.Concurrent;
using System.Threading;

namespace RD_Team_TweetMonitor
{
    public class CrawlerThread
    {
        private readonly ConcurrentStack<TwitterCrawler.Task> tasks;
        private readonly IStorage storage;
        private readonly Thread thread;
        private volatile bool working = true;

        public CrawlerThread(ConcurrentStack<TwitterCrawler.Task> tasks, IStorage storage)
        {
            this.tasks = tasks;
            this.storage = storage;
            thread = new Thread(Run);
            thread.Start();
        }

        public void Stop(int timeout = -1)
        {
            working = false;
            thread.Join(timeout);
        }

        private void Run()
        {
            var crawler = new TwitterCrawler(storage);
            var driver = default(ChromeDriver);
            while (working)
            {
                if (tasks.TryPop(out var task))
                {
                    if (driver == null)
                    {
                        driver = new ChromeDriver();
                    }
                    try
                    {
                        task.Driver = driver;
                        crawler.Run(task);
                    }
                    catch (CrawlingException ex)
                    {
                        if (driver != null)
                        {
                            driver.Quit();
                            driver = null;
                        }
                        storage.StoreException(ex);
                    }
                }
                else
                {
                    if (driver != null)
                    {
                        driver.Quit();
                        driver = null;
                    }
                    Thread.Sleep(1000);
                }
            }
            if (driver != null)
            {
                driver.Quit();
            }
        }
    }
}
