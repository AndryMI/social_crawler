using Core.Crawling;
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

        private class Command : ICommand
        {
            public string Type => "Console";
            public readonly string Url;

            public Command(string url)
            {
                Url = url;
            }

            public IEnumerable<CrawlerTask> CreateTasks()
            {
                var priority = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.000Z");

                if (Url.StartsWith("https://vk.com/"))
                {
                    yield return new VkTask(Url, priority, this);
                }
                if (Url.StartsWith("https://twitter.com/"))
                {
                    yield return new TwitterTask(Url, priority, this);
                }
                if (Url.StartsWith("https://www.instagram.com/"))
                {
                    yield return new InstagramTask(Url, priority, this);
                }
            }
        }
    }
}
