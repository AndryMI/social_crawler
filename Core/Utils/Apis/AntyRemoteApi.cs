using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Core
{
    public class AntyRemoteApi : ApiClient
    {
        public AntyRemoteApi(string user, string pass, string host = "https://anty-api.com") : base(host)
        {
            var auth = JsonConvert.DeserializeObject<Auth>(Request("POST", "/auth/login", new UrlEncodedData
            {
                { "username", user },
                { "password", pass },
            }));
            AuthHeader = "Bearer " + auth.token;
        }

        public AntyRemoteApi(string token, string host = "https://anty-api.com") : base(host)
        {
            AuthHeader = token;
        }

        public Page<T> Next<T>(Page<T> page)
        {
            if (page.next == null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<Page<T>>(Request("GET", new Uri(page.next).PathAndQuery));
        }

        public Page<Status> ListStatuses(string query = null)
        {
            return JsonConvert.DeserializeObject<Page<Status>>(Request("GET", "/browser_profiles/statuses" + new UrlEncodedData
            {
                { "query", query },
            }));
        }

        public Page<Proxy> ListProxies(string query = null)
        {
            return JsonConvert.DeserializeObject<Page<Proxy>>(Request("GET", "/proxy" + new UrlEncodedData
            {
                { "query", query },
            }));
        }

        public Page<BrowserProfile> ListBrowserProfiles(string query = null, IEnumerable<string> tags = null, IEnumerable<int> statuses = null)
        {
            return JsonConvert.DeserializeObject<Page<BrowserProfile>>(Request("GET", "/browser_profiles" + new UrlEncodedData
            {
                { "query", query },
                { "tags", tags },
                { "statuses", statuses },
            }));
        }

        public BrowserProfile GetBrowserProfile(int id)
        {
            return GetBrowserProfile(id.ToString());
        }

        public BrowserProfile GetBrowserProfile(string id)
        {
            var response = JsonConvert.DeserializeObject<Data<BrowserProfile>>(Request("GET", "/browser_profiles/" + id));
            return response.data;
        }

        public BrowserProfile CreateBrowserProfile(BrowserProfile reference)
        {
            var json = Request("POST", "/browser_profiles", new JsonData(reference));
            var response = JsonConvert.DeserializeObject<Data<BrowserProfile>>(json);
            return response.data;
        }

        public BrowserProfile CreateBrowserProfile(Platform platform, string name, Fingerprint fingerprint)
        {
            var json = Request("POST", "/browser_profiles", new JsonData(new
            {
                browserType = "anty",
                platform = platform.ToString(),
                name,
                useragent = new
                {
                    mode = "manual",
                    value = fingerprint["userAgent"].ToString().Replace("106.0.0.0", (string)fingerprint["uaFullVersion"]), //TODO hotfix Selenium with Dolphin Anty
                },
                webrtc = new { mode = "altered" },
                canvas = new { mode = "real" },
                webgl = new { mode = "real" },
                webglInfo = new
                {
                    mode = "manual",
                    vendor = fingerprint["webgl"]["unmaskedVendor"],
                    renderer = fingerprint["webgl"]["unmaskedRenderer"],
                    webgl2Maximum = fingerprint["webgl2Maximum"],
                },
                clientRect = new { mode = "real" },
                timezone = new { mode = "auto" },
                locale = new
                {
                    mode = "manual",
                    value = "en",
                },
                geolocation = new { mode = "auto" },
                cpu = new
                {
                    mode = "manual",
                    value = fingerprint["hardwareConcurrency"],
                },
                memory = new
                {
                    mode = "manual",
                    value = fingerprint["deviceMemory"],
                },
                screen = new { mode = "real" },
                audio = new { mode = "real" },
                mediaDevices = new { mode = "real" },
                ports = new
                {
                    mode = "protect",
                    blacklist = "3389,5900,5800,7070,6568,5938",
                },
                platformVersion = fingerprint["platformVersion"],
                uaFullVersion = fingerprint["uaFullVersion"],
                appCodeName = fingerprint["appCodeName"],
                platformName = fingerprint["platform"],
                connectionDownlink = fingerprint["connection"]["downlink"],
                connectionEffectiveType = fingerprint["connection"]["effectiveType"],
                connectionRtt = fingerprint["connection"]["rtt"],
                connectionSaveData = fingerprint["connection"]["saveData"],
                cpuArchitecture = fingerprint["cpu"]["architecture"],
                osVersion = fingerprint["os"]["version"],
                vendorSub = fingerprint["vendorSub"],
                vendor = fingerprint["vendor"],
                productSub = fingerprint["productSub"],
                product = fingerprint["product"],
            }));
            var response = JsonConvert.DeserializeObject<Data<BrowserProfile>>(json);
            return response.data;
        }

        //TODO Find version somewhere
        public Fingerprint NewFingerprint(Platform platform, int version = 106, string browserType = "anty")
        {
            var json = Request("GET", "/fingerprints/fingerprint" + new UrlEncodedData
            {
                { "type", "fingerprint" },
                { "screen", "1920x1080" },
                { "browser_type", browserType },
                { "browser_version", version },
                { "platform", platform.ToString() },
            });
            return JsonConvert.DeserializeObject<Fingerprint>(json);
        }

        public UserProfile GetUserProfile()
        {
            var response = JsonConvert.DeserializeObject<Data<UserProfile>>(Request("GET", "/profile"));
            return response.data;
        }

        public enum Platform { undefined, windows, linux, macos }

        public class Page<T>
        {
            [JsonProperty("next_page_url")]
            public string next;
            [JsonProperty("current_page")]
            public int current;
            [JsonProperty("last_page")]
            public int pages;
            [JsonProperty("total")]
            public int items;
            public T[] data;
        }

        public class Status : Json
        {
        }

        public class Fingerprint : Json
        {
        }

        public class Proxy : Json
        {
            public int Id => (int)this["id"];
            public string LastIp => (string)(this["lastCheck"]?["ip"]);
            public string LastCountry => (string)(this["lastCheck"]?["country"]);
        }

        public class BrowserProfile : Json
        {
            public string Id => (string)this["id"];
            public int ProxyId => (int)this["proxyId"];
            public string Name
            {
                get => (string)this["name"];
                set => this["name"] = value;
            }
        }

        public class UserProfile : Json
        {
            public string Username => (string)this["username"];
            public UserSubscription Subscription => Get<UserSubscription>(this["subscription"]);
        }

        public class UserSubscription : Json
        {
            public int BrowserProfilesCount => (int)this["browserProfiles"]["count"];
            public int BrowserProfilesLimit => (int)this["browserProfiles"]["limit"];
        }

        [JsonConverter(typeof(Converter))]
        public abstract class Json : JObject
        {
            protected static T Get<T>(JToken value) where T : Json, new()
            {
                var result = new T();
                foreach (var token in value)
                {
                    result.Add(token);
                }
                return result;
            }

            public class Converter : JsonConverter<Json>
            {
                public override Json ReadJson(JsonReader reader, Type objectType, Json existingValue, bool hasExistingValue, JsonSerializer serializer)
                {
                    var value = serializer.Deserialize<JToken>(reader);
                    var result = (Json)Activator.CreateInstance(objectType);
                    foreach (var token in value)
                    {
                        result.Add(token);
                    }
                    return result;
                }

                public override void WriteJson(JsonWriter writer, Json value, JsonSerializer serializer)
                {
                    serializer.Serialize(writer, new JObject(value));
                }
            }
        }

        private class Data<T>
        {
            public T data = default;
        }

        private class Auth
        {
            public string token = null;
        }
    }
}
