using Core;
using Core.Browsers.DevTools;
using Core.Crawling;
using Core.Storages;
using Serilog;
using System;
using System.Diagnostics;

namespace CrawlerApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Log.Fatal(e.ExceptionObject as Exception, "Terminate");
                Log.CloseAndFlush();
            };
            LoggerConfig.Init();
            ProgramArgs.Handle(args);

            DevTools.ValidateAll();
            ServerConfig.Load();

            var tasks = new TaskManager("Data/tasks.bin");
            var errors = new LocalErrorStorage("Errors");
            var storage = new LocalMultipartStorage("Data");
            var accounts = new AccountManager();

            for (var i = 0; i < Config.Instance.Threads; i++)
            {
                new CrawlerThread(tasks, accounts, storage, storage, errors);
            }
            for (var i = 0; i < Config.Instance.StorageApiThreads; i++)
            {
                new RemoteStorageThread(storage, errors);
            }
            new RemoteManager(tasks);
            new UpdateManager("Data/update.zip", "Updater.exe");

            MainLoop.Run();

            Threaded.StopAll<CrawlerThread>();
            Threaded.StopAll<RemoteStorageThread>();
            Threaded.StopAll();
            Log.CloseAndFlush();
        }

        public static void RunBrowser(string profile)
        {
            BrowserRun.Profile(profile);
            Environment.Exit(0);
        }

        public static void KillDrivers()
        {
            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                process.Kill();
            }
        }

        public static void KillChrome()
        {
            foreach (var process in Process.GetProcessesByName("chrome"))
            {
                process.Kill();
            }
        }
    }
}
