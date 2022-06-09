using System;
using System.Collections.Concurrent;

namespace RD_Team_TweetMonitor
{
    public class ConsoleManager
    {
        public static void Run(ConcurrentStack<TwitterCrawler.Task> tasks)
        {
            while (true)
            {
                Console.WriteLine("Type twitter profile url or 'exit'");
                Console.Write("> ");

                var command = Console.ReadLine();
                if (command.ToLower() == "exit")
                {
                    return;
                }

                if (command.ToLower() == "tasks")
                {
                    Console.WriteLine("There are tasks in queue: " + tasks.Count);
                }

                if (command.StartsWith("https://twitter.com/"))
                {
                    tasks.Push(new TwitterCrawler.Task
                    {
                        Url = command,
                        Profile = true,
                        Tweets = true,
                    });
                }
            }
        }
    }
}
