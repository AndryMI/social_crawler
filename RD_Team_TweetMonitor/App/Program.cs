using System.Collections.Generic;
using System.Linq;

namespace RD_Team_TweetMonitor
{
    public static class Program
    {
        public static void Main()
        {
            var tasks = new TaskManager();
            var storage = new AutoTasks(tasks, new DebugStorage());
            var threads = InitThreads(tasks, storage).ToArray();

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

        private static IEnumerable<CrawlerThread> InitThreads(TaskManager tasks, IStorage storage)
        {
            for (var i = 0; i < Config.Instance.AnonymousThreads; i++)
            {
                yield return new CrawlerThread(new AnonymousDriver(), tasks, storage);
            }
            for (var i = 0; i < Config.Instance.AuthorizedThreads; i++)
            {
                yield return new CrawlerThread(new AuthorizedDriver(), tasks, storage);
            }
        }
    }
}
