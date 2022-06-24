using Core.Storages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Crawling
{
    public class CrawlerThread
    {
        private readonly TaskManager tasks;
        private readonly IStorage storage;
        private readonly Thread thread;
        private volatile bool working = true;

        public CrawlerThread(TaskManager tasks, IStorage storage)
        {
            this.storage = storage;
            this.tasks = tasks;
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
            var browser = new Browser();
            while (working)
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

                        Task.Run(async () =>
                        {
                            await Task.Delay(TimeSpan.FromSeconds(Config.Instance.RetryTimeout));
                            tasks.Add(ex.Task);
                        });
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
