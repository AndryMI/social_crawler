using Newtonsoft.Json;
using System.Collections.Generic;

namespace Core
{
    public class ServerConfig
    {
        public static ServerConfig Instance { get; private set; }

        [JsonProperty("server_info")]
        public readonly ServerInfo Server;

        [JsonProperty("media_warmup")]
        public readonly Dictionary<string, string> MediaWarmup;

        public static void Load()
        {
            var json = new ApiServerClient().Request("GET", "/crawler");
            Instance = JsonConvert.DeserializeObject<ServerConfig>(json);
        }

        public class ServerInfo
        {
            [JsonProperty("max_file_uploads")]
            public readonly int MaxFileUploads;
            [JsonProperty("max_execution_time")]
            public readonly int MaxExecutionTime;
        }
    }
}
