using Newtonsoft.Json;
using System.IO;

namespace Core
{
    public class Config
    {
        public static readonly Config Instance = Init();

        [JsonProperty]
        public string ApiUrl { get; private set; } = "https://sc-api-srv.profcatalyst.com/api";

        [JsonProperty]
        public string ApiMail { get; private set; } = "qa@roibees.com";

        [JsonProperty]
        public string ApiPass { get; private set; } = "test";

        [JsonProperty]
        public int Threads { get; private set; } = 1;

        [JsonProperty]
        public int WaitTimeout { get; private set; } = 300;

        [JsonProperty]
        public int RetryTimeout { get; private set; } = 60;

        [JsonProperty]
        public int StorageApiSizeThreshold { get; private set; } = 1000000;

        [JsonProperty]
        public int StorageApiFilesThreshold { get; private set; } = 20;

        [JsonProperty]
        public double StorageApiSendInterval { get; private set; } = 1;

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
