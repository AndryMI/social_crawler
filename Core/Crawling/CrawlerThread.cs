using Core.Storages;
using System.Threading;

namespace Core.Crawling
{
    public class CrawlerThread : Threaded
    {
        private readonly TaskManager tasks;
        private readonly IStorage storage;

        public CrawlerThread(TaskManager tasks, IStorage storage)
        {
            this.storage = storage;
            this.tasks = tasks;
        }

        protected override void Run()
        {
            var browser = new Browser();
            while (IsWorking)
            {
                if (tasks.TryGet(browser, out var task))
                {
                    try
                    {
                        task.Run(browser, storage, tasks);
                    }
                    catch (CrawlingException ex)
                    {
                        storage.StoreException(ex);
                        browser.Close();
                        tasks.Retry(ex.Task);
                    }
                }
                else
                {
                    browser.Close();
                }
                Thread.Sleep(1000);
            }
            browser.Close();
        }
    }
}
