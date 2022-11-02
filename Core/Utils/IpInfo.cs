using Newtonsoft.Json;
using Serilog;
using System;
using System.Net;

namespace Core
{
    public class IpInfo
    {
        private static readonly WebClient client = new WebClient();
        private static DateTimeOffset update;
        private static IpInfo last;

        [JsonProperty]
        public readonly string ip;

        [JsonProperty]
        public readonly string country;

        public override string ToString()
        {
            return country + ":" + ip;
        }

        public static IpInfo My()
        {
            if (update < DateTimeOffset.Now)
            {
                try
                {
                    last = JsonConvert.DeserializeObject<IpInfo>(client.DownloadString("https://ipinfo.io/json"));
                    update = DateTimeOffset.Now.AddHours(4);
                }
                catch (Exception e)
                {
                    Log.Warning(e, "Failed to get self IP");
                    update = DateTimeOffset.Now.AddMinutes(10);
                }
            }
            return last;
        }
    }
}
