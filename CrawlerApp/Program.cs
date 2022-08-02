using Core;
using Core.Crawling;
using Core.Storages;
using Serilog;
using Serilog.Filters;
using Serilog.Formatting.Compact;
using System.Diagnostics;

namespace CrawlerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(new CompactJsonFormatter(), "Logs/.log", rollingInterval: RollingInterval.Day)
                .WriteTo.Logger(log => log
                    .Filter.ByExcluding(Matching.FromSource<BrowserNetwork>())
                    .WriteTo.Console()
                    .WriteTo.Debug()
                )
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .CreateLogger();

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
            Log.CloseAndFlush();
        }
    }
}
