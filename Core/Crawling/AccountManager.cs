using Core.Browsers.Profiles;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;

namespace Core.Crawling
{
    public class AccountManager
    {
        private readonly ApiServerClient client = new ApiServerClient();
        private readonly Antyinfo anty = new Antyinfo();

        public Account Take<T>() where T : Account
        {
            var json = client.Request("POST", "/accounts/take", new JsonData(new
            {
                guid = Config.Guid,
                type = typeof(T).Name,
                crawler = IpInfo.My(),
                antyuser = anty.username,
            }));
            var response = JsonConvert.DeserializeObject<Response<T>>(json, new BrowserProfileJson());
            if (response.account == null)
            {
                var time = DateTimeOffset.FromUnixTimeSeconds(response.time);
                throw new TryLaterException("No available accounts", time);
            }
            return response.account;
        }

        public void Release(Account account) => Release(account, "release");
        public void Blocked(Account account) => Release(account, "block");

        private void Release(Account account, string action)
        {
            if (account == null)
            {
                return;
            }
            try
            {
                if (account is RandomProxyAccount)
                {
                    anty.SetActiveTabs(account.BrowserProfile, "about:blank");
                }
            }
            catch { }
            try
            {
                client.Request("POST", "/accounts/" + action, new JsonData(new
                {
                    guid = Config.Guid,
                    crawler = IpInfo.My(),
                    browser = account.BrowserProfile is AntyProfile ? anty.GetIpInfo(account.BrowserProfile) : IpInfo.My(),
                    account,
                }));
            }
            catch (Exception e)
            {
                Log.Warning(e, $"Failed to {action} account");
            }
        }

        private class Antyinfo
        {
            private readonly AntyRemoteApi anty = new AntyRemoteApi(new AntyLocalApi().RemoteApiToken());
            private readonly Dictionary<string, IpInfo> browsers = new Dictionary<string, IpInfo>();
            private readonly Dictionary<int, IpInfo> proxies = new Dictionary<int, IpInfo>();
            private readonly object locker = new object();
            private DateTimeOffset update;

            public readonly string username;

            public Antyinfo()
            {
                username = anty.GetUserProfile().Username;
            }

            public void SetActiveTabs(IBrowserProfile profile, params string[] tabs)
            {
                anty.SetBrowserProfileTabs(profile.Id, tabs);
            }

            public IpInfo GetIpInfo(IBrowserProfile profile)
            {
                if (NeedUpdate())
                {
                    try
                    {
                        UpdateProxies();
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e, "Failed to update proxies info");
                    }
                }
                try
                {
                    lock (locker)
                    {
                        if (browsers.TryGetValue(profile.Id, out var info))
                        {
                            return info ?? IpInfo.My();
                        }
                    }
                    var browser = anty.GetBrowserProfile(profile.Id);
                    lock (locker)
                    {
                        if (proxies.TryGetValue(browser.ProxyId, out var info))
                        {
                            browsers[profile.Id] = info;
                            return info ?? IpInfo.My();
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Warning(e, "Failed to get profile IP info");
                }
                return IpInfo.My();
            }

            private bool NeedUpdate()
            {
                lock (locker)
                {
                    if (update < DateTimeOffset.Now)
                    {
                        update = DateTimeOffset.Now.AddMinutes(15);
                        return true;
                    }
                    return false;
                }
            }

            private void UpdateProxies()
            {
                var proxies = new Dictionary<int, IpInfo>();
                for (var page = anty.ListProxies(); page != null; page = anty.Next(page))
                {
                    foreach (var proxy in page.data)
                    {
                        proxies[proxy.Id] = new IpInfo(proxy.LastIp, proxy.LastCountry);
                    }
                }
                proxies[0] = null;

                var browsers = new Dictionary<string, int>();
                for (var page = anty.ListBrowserProfiles(); page != null; page = anty.Next(page))
                {
                    foreach (var browser in page.data)
                    {
                        browsers[browser.Id] = browser.ProxyId;
                    }
                }

                lock (locker)
                {
                    this.proxies.Clear();
                    this.browsers.Clear();

                    foreach (var p in proxies)
                    {
                        this.proxies.Add(p.Key, p.Value);
                    }
                    foreach (var p in browsers)
                    {
                        this.browsers.Add(p.Key, proxies[p.Value]);
                    }
                }
            }
        }

        private class Response<T> where T : Account
        {
            public T account = null;
            public long time = 0;
        }
    }
}
