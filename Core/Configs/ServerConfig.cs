using Newtonsoft.Json;
using System.Collections.Generic;

namespace Core
{
    public class ServerConfig
    {
        public static ServerConfig Instance { get; private set; }

        [JsonProperty("server_info")]
        public readonly ServerInfo Server;

        [JsonProperty("blacklist")]
        public readonly string[] Blacklists;

        [JsonProperty("media_warmup")]
        public readonly Dictionary<string, string> MediaWarmup;

        public static void Load()
        {
            var json = new ApiServerClient().Request("GET", "/crawler");
            json = json.Replace("\"media_warmup\":[]", "\"media_warmup\":{}");
            Instance = JsonConvert.DeserializeObject<ServerConfig>(json);
        }

        public class ServerInfo
        {
            [JsonProperty("max_input_vars")]
            public readonly int MaxInputVars;
            [JsonProperty("max_file_uploads")]
            public readonly int MaxFileUploads;
            [JsonProperty("max_execution_time")]
            public readonly int MaxExecutionTime;
        }
    }
}
