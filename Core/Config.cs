using Newtonsoft.Json;
using System.IO;

namespace Core
{
    public class Config
    {
        public static readonly Config Instance = Init();

        public string ApiUrl { get; private set; } = "https://sc-api-srv.albertroi.com/api";
        public string ApiMail { get; private set; } = "qa@roibees.com";
        public string ApiPass { get; private set; } = "test";

        public int Threads { get; private set; } = 1;
        public int WaitTimeout { get; private set; } = 90;
        public int RetryTimeout { get; private set; } = 60;

        private static Config Init()
        {
            if (File.Exists("Configs/config.json"))
            {
                return JsonConvert.DeserializeObject<Config>(File.ReadAllText("Configs/config.json"));
            }
            return new Config();
        }
    }
}
