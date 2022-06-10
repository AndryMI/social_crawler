using Newtonsoft.Json;
using System;
using System.IO;

namespace RD_Team_TweetMonitor
{
    public class TwitterAccount
    {
        public string Mail;
        public string Name;
        public string Password;
    }

    public class Config
    {
        private static readonly Random random = new Random();
        public static readonly Config Instance = Init();

        public int AnonymousThreads { get; private set; } = 4;
        public int AuthorizedThreads { get; private set; } = 1;
        public int RetryTimeout { get; private set; } = 30;

        public TwitterAccount[] Accounts { get; private set; } = new TwitterAccount[]
        {
            new TwitterAccount
            {
                Mail = "rowas40579@iconzap.com",
                Name = "RowasZap",
                Password = "hello+world",
            }
        };

        public TwitterAccount RandomAccount()
        {
            return Accounts[random.Next(Accounts.Length)];
        }

        private static Config Init()
        {
            if (File.Exists("config.json"))
            {
                return JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            }
            return new Config();
        }
    }
}
