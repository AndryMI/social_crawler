using Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace CrawlerApp
{
    public class UpdateManager : Threaded
    {
        private static long CurrentVersion = InitVersion();
        private readonly string file;
        private readonly string updater;

        public UpdateManager(string file, string updater)
        {
            this.file = file;
            this.updater = updater;
        }

        protected override void Run()
        {
            var client = new ApiServerClient();
            while (IsWorking)
            {
                try
                {
                    var version = long.Parse(client.Request("GET", "/crawler/version"));
                    if (version > CurrentVersion)
                    {
                        var update = client.Request("GET", "/crawler/update", base64: true);
                        File.WriteAllBytes(file, Convert.FromBase64String(update));
                        RunUpdater();
                        CurrentVersion = version;
                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Failed to check/receive update");
                }
                LongSleep(TimeSpan.FromSeconds(Config.Instance.CheckUpdateInterval));
            }
        }

        private void RunUpdater()
        {
            var args = new List<string>
            {
                Process.GetCurrentProcess().Id.ToString(),
                Path.GetFullPath(file),
            };
            args.AddRange(Environment.GetCommandLineArgs());

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = updater,
                Arguments = ProgramArgs.Escape(args.ToArray()),
                UseShellExecute = true,
            });
            if (process.WaitForExit(10000))
            {
                Log.Fatal("Failed to start Update: {ExitCode}", process.ExitCode);
            }
            else
            {
                MainLoop.Terminate();
            }
        }

        private static long InitVersion()
        {
            DateTimeOffset time = File.GetLastWriteTimeUtc(Assembly.GetExecutingAssembly().Location);
            return time.ToUnixTimeSeconds();
        }
    }
}
