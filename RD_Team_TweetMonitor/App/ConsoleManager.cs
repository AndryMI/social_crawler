﻿using System;

namespace RD_Team_TweetMonitor
{
    public class ConsoleManager
    {
        public static void Run(TaskManager tasks)
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
                    tasks.Add(CrawlerTask.FromUrl(command));
                }
            }
        }
    }
}
