using Core;
using Core.Crawling;
using Core.Managers;
using Core.Storages;
using Instagram.Crawling;
using System.Diagnostics;
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

            var factory = new TaskFactory()
            {
                { "https://vk.com/", (url, priority) => new VkTask(url, priority) },
                { "https://twitter.com/", (url, priority) => new TwitterTask(url, priority) },
                { "https://www.instagram.com/", (url, priority) => new InstagramTask(url, priority) },
            };

            var tasks = new TaskManager(factory);
            var storage = new RemoteStorage();

            for (var i = 0; i < Config.Instance.Threads; i++)
            {
                new CrawlerThread(tasks, storage);
            }

            ConsoleManager.Run(tasks);

            Threaded.StopAll();
        }
    }
}
