﻿using Core.Storages;
using Serilog;
using System.Threading;

namespace Core.Crawling
{
    public class CrawlerThread : Threaded
    {
        private readonly TaskManager tasks;
        private readonly IDataStorage storage;
        private readonly IMediaStorage media;

        public CrawlerThread(TaskManager tasks, IDataStorage storage, IMediaStorage media)
        {
            this.storage = storage;
            this.tasks = tasks;
            this.media = media;
        }

        protected override void Run()
        {
            var browser = new Browser(media);
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
