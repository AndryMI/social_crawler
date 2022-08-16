using Newtonsoft.Json;
using System.IO;

namespace Core
{
    //TODO config hot reload
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
        public int RetryAttempts { get; private set; } = 15;

        [JsonProperty]
        public int MultipartSizeThreshold { get; private set; } = 1000000;

        [JsonProperty]
        public int MultipartFilesThreshold { get; private set; } = 20;

        [JsonProperty]
        public int MultipartVarsThreshold { get; private set; } = 1000;

        [JsonProperty]
        public int StorageApiThreads { get; private set; } = 1;

        [JsonProperty]
        public double StorageApiSendInterval { get; private set; } = 1;

        [JsonProperty]
        public bool BrowserHeadless { get; private set; } = false;

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
