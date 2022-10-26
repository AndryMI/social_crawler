using Newtonsoft.Json;
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
            return JsonConvert.DeserializeObject<Page<T>>(Request("GET", new Uri(page.next).PathAndQuery));
        }

        public Page<Status> ListStatuses(string query = null)
        {
            return JsonConvert.DeserializeObject<Page<Status>>(Request("GET", "/browser_profiles/statuses" + new UrlEncodedData
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
            var response = JsonConvert.DeserializeObject<Data<BrowserProfile>>(Request("GET", "/browser_profiles/" + id));
            return response.data;
        }

        public string GetNewUserAgent(Platform platform, int? version = null, string browserType = "anty")
        {
            var response = JsonConvert.DeserializeObject<Data<string>>(Request("GET", "/fingerprints/useragent" + new UrlEncodedData
            {
                { "browser_type", browserType },
                { "browser_version", version },
                { "platform", platform },
            }));
            return response.data;
        }

        public UserProfile GetUserProfile()
        {
            var response = JsonConvert.DeserializeObject<Data<UserProfile>>(Request("GET", "/profile"));
            return response.data;
        }

        public enum Platform { undefined, windows, linux, macos }

        public class Status
        {
            public int id;
            public int teamId;
            public string name;
            public string color;
            public bool deleted;
        }

        public class UserProfile
        {
            public string username;
        }

        public class BrowserProfile
        {
            public int id;
            public int userId;
            public int teamId;
            public string name;
            public Platform platform;
            public string browserType;
            public int proxyId;
            public string mainWebsite;

            public Mode<string> useragent;
            public IpMode webrtc;
            public NoiseMode canvas;
            public NoiseMode webgl;
            public WebGlMode webglInfo;
            public NoiseMode clientRect;
            public Notes notes;
            public Mode<string> timezone;
            public Mode<string> locale;
            public int totalSessionDuration;
            public string userFields;
            public GeoMode geolocation;
            public bool doNotTrack;
            public string[] args;
            public Mode<int> cpu;
            public Mode<int> memory;
            public Mode<string> screen;
            public BlacklistMode ports;
            public string[] tabs;
            public string created_at;
            public string updated_at;
            public string deleted_at;

            public string platformName;
            public string cpuArchitecture;
            public int osVersion;
            public string connectionDownlink;
            public string connectionEffectiveType;
            public int connectionRtt;
            public int connectionSaveData;
            public string vendorSub;
            public int productSub;
            public string vendor;
            public string product;
            public string appCodeName;
            public MediaMode mediaDevices;
            public string datadirHash;
            public string cookiesHash;
            public string storagePath;
            public string lastRunningTime;
            public string lastRunnedByUserId;
            public string lastRunUuid;
            public int running;
            public string platformVersion;
            public int archived;
            public string login;
            public string password;
            public Access access;
            public string name_digital;
            public Status status;
            public string[] tags;
            public string[] tags_with_separator;
            public bool pinned;
        }

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

        public class Mode
        {
            public string mode;
        }

        public class Mode<T> : Mode
        {
            public T value;
        }

        public class IpMode : Mode
        {
            public string ipAddress;
        }

        public class BlacklistMode : Mode
        {
            public string blacklist;
        }

        public class NoiseMode : Mode
        {
            public double[] noise;
        }

        public class GeoMode : Mode
        {
            public string latitude;
            public string longitude;
            public string accuracy;
        }

        public class WebGlMode : Mode
        {
            public string vendor;
            public string renderer;
            public string webgl2Maximum;
        }

        public class MediaMode : Mode
        {
            public int? audioInputs;
            public int? videoInputs;
            public int? audioOutputs;
        }

        public class Notes
        {
            public string content;
            public string color;
            public string style;
            public string icon;
        }

        public class Access
        {
            public int view;
            public int update;
            public int delete;
            public int share;
            public int usage;
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
