using Core;
using Core.Crawling;
using Core.Storages;
using System.Diagnostics;

namespace CrawlerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg == "--kill-drivers")
                {
                    foreach (var process in Process.GetProcessesByName("chromedriver"))
                    {
                        process.Kill();
                    }
                }
            }

            var tasks = new TaskManager();
            var storage = new DebugStorage();

            for (var i = 0; i < Config.Instance.Threads; i++)
            {
                new CrawlerThread(tasks, storage);
            }

            ConsoleManager.Run(tasks);

            Threaded.StopAll();
        }
    }
}
