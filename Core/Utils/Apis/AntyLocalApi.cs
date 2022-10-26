using Newtonsoft.Json;
using Serilog;
using System;

namespace Core
{
    public class AntyLocalApi : ApiClient
    {
        public AntyLocalApi(string host = "http://localhost:3001/v1.0") : base(host)
        {
        }

        public string RemoteApiToken()
        {
            var info = JsonConvert.DeserializeObject<Token>(Request("GET", "/remote_api_token"));
            return info.token;
        }

        public Automation Start(int browserProfileId)
        {
            return Start(browserProfileId.ToString());
        }

        public Automation Start(string browserProfileId)
        {
            try
            {
                var info = JsonConvert.DeserializeObject<Info>(Request("GET", $"/browser_profiles/{browserProfileId}/start?automation=1"));
                return info.automation;
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to start Anty {ProfileId}", browserProfileId);
                return null;
            }
        }

        public bool Stop(int browserProfileId)
        {
            var info = JsonConvert.DeserializeObject<Info>(Request("GET", $"/browser_profiles/{browserProfileId}/stop"));
            return info.success;
        }

        public class Automation
        {
            [JsonProperty]
            public readonly int port;
            [JsonProperty("wsEndpoint")]
            public readonly string endpoint;
        }

        private class Info
        {
            public bool success = false;
            public Automation automation = null;
        }

        private class Token
        {
            public string token = null;
        }
    }
}
