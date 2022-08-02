using Core.Storages;
using Serilog;
using System.Threading;

namespace Core.Crawling
{
    public class CrawlerThread : Threaded
    {
        private readonly TaskManager tasks;
        private readonly IDataStorage storage;

        public CrawlerThread(TaskManager tasks, IDataStorage storage)
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
                        Log.Information("Begin Task: {task}", task);
                        task.Run(browser, storage, tasks);
                        tasks.Complete(task);
                    }
                    catch (CrawlingException ex)
                    {
                        Log.Warning(ex, "Task failed");
                        storage.StoreException(ex);
                        browser.Close();
                        tasks.Complete(task);
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
