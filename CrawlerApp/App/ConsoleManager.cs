using Core;
using Core.Crawling;
using Facebook.Crawling;
using Instagram.Crawling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Twitter.Crawling;
using VK.Crawling;

namespace CrawlerApp
{
    public class ConsoleManager
    {
        public static void Run(TaskManager tasks)
        {
            while (true)
            {
                Console.WriteLine("Type url, 'stats' or 'exit'");
                Console.Write("> ");

                var command = Console.ReadLine();
                if (command == null || command.ToLower() == "exit")
                {
                    return;
                }

                if (command.ToLower() == "stats")
                {
                    Console.WriteLine(JsonConvert.SerializeObject(tasks.Progress, Formatting.Indented));
                }

                if (command.StartsWith("https://"))
                {
                    try
                    {
                        tasks.Add(new Command(command));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        [Serializable]
        private class Command : ICommand
        {
            public string Id { get; private set; }
            public string Type => "console";
            public readonly string Url;

            public Command(string url)
            {
                Id = new ObjectId();
                Url = url;
            }

            public IEnumerable<CrawlerTask> CreateTasks()
            {
                if (Url.StartsWith("https://vk.com/"))
                {
                    yield return new VkTask(Url, CrawlerTask.DefaultPriority, this);
                }
                if (Url.StartsWith("https://twitter.com/"))
                {
                    yield return new TwitterTask(Url, CrawlerTask.DefaultPriority, this);
                }
                if (Url.StartsWith("https://www.instagram.com/"))
                {
                    yield return new InstagramTask(Url, CrawlerTask.DefaultPriority, this);
                }
                if (Url.StartsWith("https://www.facebook.com/"))
                {
                    yield return new FacebookTask(Url, CrawlerTask.DefaultPriority, this);
                }
            }
        }
    }
}
