using Core;
using Core.Crawling;
using Facebook.Crawling;
using Instagram.Crawling;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using Twitter.Crawling;

namespace CrawlerApp
{
    public class RemoteManager : Threaded
    {
        private readonly TaskManager tasks;

        public RemoteManager(TaskManager tasks)
        {
            this.tasks = tasks;
        }

        protected override void Run()
        {
            var client = new Client();
            while (IsWorking)
            {
                try
                {
                    var data = client.Sync(tasks.Progress);
                    tasks.SetBlacklist(data.blacklist);
                    foreach (var command in data.commands)
                    {
                        tasks.Add(command);
                    }
                    tasks.Save();
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Failed to sync commands");
                }
                LongSleep(TimeSpan.FromMinutes(1));
            }
            tasks.Save();
        }

        private class Client : ApiServerClient
        {
            private readonly CommandsFactory factory = new CommandsFactory();

            public Client()
            {
                factory.Register<CancelCommand>("command:cancel");
                factory.Register<TwitterCommand>("twitter:by_profile_link");
                factory.Register<TwitterCommand>("twitter:by_keyword");
                factory.Register<InstagramCommand>("instagram:by_profile_link");
                factory.Register<InstagramCommand>("instagram:by_keyword");
                factory.Register<FacebookCommand>("facebook:by_profile_link");
                factory.Register<FacebookCommand>("facebook:by_keyword");
            }

            public SyncResponse Sync(List<TaskManager.Status> progress)
            {
                var response = Request("POST", "/crawler", new JsonData(new { guid = Config.Guid, progress, types = factory.Types, strategy = Config.Instance.Strategy }));
                return JsonConvert.DeserializeObject<SyncResponse>(response, factory);
            }
        }

        private class SyncResponse
        {
            public ICommand[] commands = null;
            public string[] blacklist = null;
        }
    }
}
