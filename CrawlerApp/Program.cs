﻿using Core;
using Core.Crawling;
using Core.Managers;
using Core.Storages;
using Instagram.Crawling;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Twitter.Crawling;
using VK.Crawling;

namespace CrawlerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg == "--kill-drivers")
                {
                    foreach (var process in Process.GetProcessesByName("chromedriver"))
                    {
                        process.Kill();
                    }
                }
            }

            var storage = new DebugStorage();
            var factory = new TaskFactory()
            {
                { "https://vk.com/", (url, priority) => new VkTask(url, priority) },
                { "https://twitter.com/", (url, priority) => new TwitterTask(url, priority) },
                { "https://www.instagram.com/", (url, priority) => new InstagramTask(url, priority) },
            };

            var tasks = new TaskManager(factory);
            var threads = InitThreads(tasks, storage).ToArray();

            ConsoleManager.Run(tasks);

            for (var i = 0; i < threads.Length; i++)
            {
                threads[i].Stop(0);
            }
            for (var i = 0; i < threads.Length; i++)
            {
                threads[i].Stop();
            }
        }

        private static IEnumerable<CrawlerThread> InitThreads(TaskManager tasks, IStorage storage)
        {
            for (var i = 0; i < Config.Instance.Threads; i++)
            {
                yield return new CrawlerThread(tasks, storage);
            }
        }
    }
}
