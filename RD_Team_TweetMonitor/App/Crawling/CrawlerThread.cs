using System.Threading;

namespace RD_Team_TweetMonitor
{
    public class CrawlerThread
    {
        private readonly IDriver driver;
        private readonly TaskManager tasks;
        private readonly IStorage storage;
        private readonly Thread thread;
        private volatile bool working = true;

        public CrawlerThread(IDriver driver, TaskManager tasks, IStorage storage)
        {
            this.driver = driver;
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
            while (working)
            {
                if (tasks.TryGet(driver.Type, out var task))
                {
                    try
                    {
                        crawler.Run(driver.Instance(), task);
                    }
                    catch (CrawlingException ex)
                    {
                        driver.Destroy();
                        storage.StoreException(ex);
                    }
                    Thread.Sleep(0);
                }
                else
                {
                    driver.Suspend();
                    Thread.Sleep(1000);
                }
            }
            driver.Destroy();
        }
    }
}
