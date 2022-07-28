using Core;
using Core.Crawling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using Twitter.Crawling;

namespace CrawlerApp
{
    public class RemoteManager : ApiServerClient
    {
        private readonly string guid = Guid.NewGuid().ToString();
        private readonly CommandsFactory factory = new CommandsFactory();

        private RemoteManager()
        {
            factory.Register<TwitterCommand>("twitter:by_profile_link");
            factory.Register<TwitterCommand>("twitter:by_keyword");
        }

        private ICommand[] Sync(List<TaskProgress.Item> progress)
        {
            var response = Request("POST", "/crawler", new { guid, progress, types = factory.Types });
            var data = JsonConvert.DeserializeObject<SyncResponse>(response, factory);
            return data.commands;
        }

        public static void Run(TaskManager tasks)
        {
            var client = new RemoteManager();
            while (true)
            {
                //TODO exception handling

                var commands = client.Sync(tasks.Progress);
                foreach (var command in commands)
                {
                    tasks.Add(command);
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
