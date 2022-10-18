using Core.Storages;
using Serilog;
using System.Threading;

namespace Core.Crawling
{
    public class CrawlerThread : Threaded
    {
        private readonly TaskManager tasks;
        private readonly IDataStorage storage;
        private readonly IMediaStorage media;
        private readonly IErrorStorage errors;

        public CrawlerThread(TaskManager tasks, IDataStorage storage, IMediaStorage media, IErrorStorage errors)
        {
            this.storage = storage;
            this.tasks = tasks;
            this.media = media;
            this.errors = errors;
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
                        Log.Information("Task complete");
                    }
                    catch (CrawlingException ex)
                    {
                        if (ex.InnerException is TryLaterException later)
                        {
                            Log.Warning(ex, "Task delayed: {Time}", later.Time);
                            tasks.Complete(task);
                            tasks.Delay(ex.Task, later.Time);
                        }
                        else
                        {
                            Log.Warning(ex, "Task failed");
                            errors.StoreException(ex);
                            browser.Close();
                            tasks.Complete(task);
                            tasks.Retry(ex.Task);
                        }
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
