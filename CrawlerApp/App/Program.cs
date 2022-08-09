using Core;
using Core.Crawling;
using Core.Storages;
using Serilog;
using System.Diagnostics;

namespace CrawlerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoggerConfig.Init();

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
            var storage = new RemoteStorage();

            for (var i = 0; i < Config.Instance.Threads; i++)
            {
                new CrawlerThread(tasks, storage, storage);
            }

            RemoteManager.Run(tasks);

            Threaded.StopAll();
            Log.CloseAndFlush();
        }
    }
}
