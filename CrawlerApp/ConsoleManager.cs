using Core.Crawling;
using Instagram.Crawling;
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
                    //TODO write tasks execution stats
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
            private readonly string url;

            public Command(string url)
            {
                this.url = url;
            }

            public IEnumerable<CrawlerTask> CreateTasks()
            {
                var priority = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.000Z");

                if (url.StartsWith("https://vk.com/"))
                {
                    yield return new VkTask(url, priority, this);
                }
                if (url.StartsWith("https://twitter.com/"))
                {
                    yield return new TwitterTask(url, priority, this);
                }
                if (url.StartsWith("https://www.instagram.com/"))
                {
                    yield return new InstagramTask(url, priority, this);
                }
            }
        }
    }
}
