using Core;
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
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Log.Fatal(e.ExceptionObject as Exception, "Terminate");
            LoggerConfig.Init();
            ProgramArgs.Handle(args);

            ServerConfig.Load();

            var tasks = new TaskManager();
            var storage = new RemoteStorage();
            var errors = new LocalErrorStorage("Errors");

            for (var i = 0; i < Config.Instance.Threads; i++)
            {
                new CrawlerThread(tasks, storage, storage, errors);
            }

            RemoteManager.Run(tasks);

            Threaded.StopAll();
            Log.CloseAndFlush();
        }

        public static void KillDrivers()
        {
            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                process.Kill();
            }
        }
    }
}
