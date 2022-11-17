using Core;
using Core.Crawling;
using Facebook.Crawling;
using Instagram.Crawling;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using Twitter.Crawling;

namespace CrawlerApp
{
    public class RemoteManager : ApiServerClient
    {
        private readonly CommandsFactory factory = new CommandsFactory();

        private RemoteManager()
        {
            factory.Register<CancelCommand>("command:cancel");
            factory.Register<TwitterCommand>("twitter:by_profile_link");
            factory.Register<TwitterCommand>("twitter:by_keyword");
            factory.Register<InstagramCommand>("instagram:by_profile_link");
            factory.Register<InstagramCommand>("instagram:by_keyword");
            factory.Register<FacebookCommand>("facebook:by_profile_link");
            factory.Register<FacebookCommand>("facebook:by_keyword");
        }

        private ICommand[] Sync(List<TaskManager.Status> progress)
        {
            var response = Request("POST", "/crawler", new JsonData(new { guid = Config.Guid, progress, types = factory.Types }));
            var data = JsonConvert.DeserializeObject<SyncResponse>(response, factory);
            return data.commands;
        }

        public static void Run(TaskManager tasks)
        {
            var client = new RemoteManager();
            while (true)
            {
                try
                {
                    var commands = client.Sync(tasks.Progress);
                    foreach (var command in commands)
                    {
                        tasks.Add(command);
                    }
                    tasks.Save();
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Failed to sync commands");
                }
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }

        private class SyncResponse
        {
            public ICommand[] commands = null;
        }
    }
}
