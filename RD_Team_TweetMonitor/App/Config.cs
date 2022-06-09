using Newtonsoft.Json;
using System.IO;

namespace RD_Team_TweetMonitor
{
    public class Config
    {
        public static readonly Config Instance = Init();

        public int Threads { get; private set; } = 4;
        public int RetryTimeout { get; private set; } = 30;

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
