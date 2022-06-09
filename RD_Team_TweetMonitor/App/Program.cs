using System.Collections.Concurrent;

namespace RD_Team_TweetMonitor
{
    public static class Program
    {
        public static void Main()
        {
            var tasks = new ConcurrentStack<TwitterCrawler.Task>();
            var threads = new CrawlerThread[Config.Instance.Threads];
            var storage = new AutoTasks(tasks, new FileStorage());

            for (var i = 0; i < threads.Length; i++)
            {
                threads[i] = new CrawlerThread(tasks, storage);
            }

            ConsoleManager.Run(tasks);

            for (var i = 0; i < threads.Length; i++)
            {
                threads[i].Stop(0);
            }
            for (var i = 0; i < threads.Length; i++)
            {
                threads[i].Stop();
            }
        }
    }
}
