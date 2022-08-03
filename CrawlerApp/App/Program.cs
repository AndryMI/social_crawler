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
            var storage = new DebugStorage();
            var media = new LocalMediaStorage("Files");

            for (var i = 0; i < Config.Instance.Threads; i++)
            {
                new CrawlerThread(tasks, storage, media);
            }

            ConsoleManager.Run(tasks);

            Threaded.StopAll();
            Log.CloseAndFlush();
        }
    }
}
