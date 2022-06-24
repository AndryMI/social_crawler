using Core.Storages;

namespace Core.Crawling
{
    public abstract class CrawlerTask
    {
        public readonly string Priority;
        public readonly string Url;

        public abstract string Type { get; }
        public abstract void Run(Browser browser, IStorage storage, TaskManager tasks);

        public CrawlerTask(string url, string priority)
        {
            Priority = priority;
            Url = url;
        }
    }
}
