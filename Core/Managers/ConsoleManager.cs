using Core.Crawling;
using System;

namespace Core.Managers
{
    public class ConsoleManager
    {
        public static void Run(TaskManager tasks)
        {
            while (true)
            {
                Console.WriteLine("Type url, 'tasks' or 'exit'");
                Console.Write("> ");

                var command = Console.ReadLine();
                if (command == null || command.ToLower() == "exit")
                {
                    return;
                }

                if (command.ToLower() == "tasks")
                {
                    Console.WriteLine("There are tasks in queue: " + tasks.Count);
                }

                if (command.StartsWith("https://"))
                {
                    try
                    {
                        tasks.AddUrl(command);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}
