using Core.Crawling;
using Core.Storages;

namespace Instagram.Crawling
{
    public class InstagramTask : CrawlerTask
    {
        public override string Type => "Instagram";

        public InstagramTask(string url, string priority) : base(url, priority)
        {
        }

        public override void Run(Browser browser, IStorage storage, TaskManager tasks)
        {
            new InstagramCrawler(browser, new InstagramStorage(storage, tasks), this).Run();
        }
    }
}
