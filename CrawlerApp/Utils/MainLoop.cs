using Serilog;
using System;
using System.Threading;

namespace CrawlerApp
{
    public static class MainLoop
    {
        private static volatile bool working = true;

        public static void Run()
        {
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                working = false;
                Log.Information("Ctrl+C signal detected, terminating...");
            };
            while (working)
            {
                Thread.Sleep(1000);
            }
        }

        public static void Terminate()
        {
            working = false;
            Log.Information("Terminate requested, terminating...");
        }
    }
}
