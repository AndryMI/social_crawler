using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Core
{
    //TODO config hot reload
    public class Config
    {
        /// <summary> Unique ID of crawler instance running </summary>
        public static readonly string Guid = InitGuid();

        public static readonly Config Instance = Init();

        [JsonProperty]
        public int CheckUpdateInterval { get; private set; } = 300;

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
        public int RetryTimeout { get; private set; } = 150;

        [JsonProperty]
        public int RetryAttempts { get; private set; } = 10;

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

        [JsonProperty]
        public CrawlerStrategy Strategy { get; private set; } = CrawlerStrategy.Default;

        public enum CrawlerStrategy
        {
            /// <summary>Take commands sequentially and try to execute completely</summary>
            Default,
            /// <summary>Take the newest commands and leave the previous ones unfinished</summary>
            Urgent,
        }

        private static Config Init()
        {
            if (File.Exists("Configs/config.json"))
            {
                return JsonConvert.DeserializeObject<Config>(File.ReadAllText("Configs/config.json"));
            }
            return new Config();
        }

        private static string InitGuid()
        {
            using (var random = RandomNumberGenerator.Create())
            {
                var server = IpInfo.My();
                var bytes = new byte[4];
                random.GetNonZeroBytes(bytes);
                return $"{server.country} : {server.ip} : {BitConverter.ToInt32(bytes, 0):X8}";
            }
        }
    }
}
