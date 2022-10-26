using Core.Browsers.DevTools;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Core.Crawling
{
    public class BrowserRequestsDump : IDisposable
    {
        private readonly Dictionary<string, bool> requests = new Dictionary<string, bool>();
        private readonly Predicate<string> predicate;
        private readonly ChromeDriver driver;
        private readonly Network network;

        public BrowserRequestsDump(ChromeDriver driver, Predicate<string> predicate)
        {
            this.predicate = predicate;
            this.driver = driver;

            network = Network.Create(driver);
            network.OnRequestWillBeSent = OnRequestWillBeSent;
            network.OnLoadingFailed = OnLoadingFailed;
            network.OnLoadingFinished = OnLoadingFinished;
        }

        public void Dispose()
        {
            network.Dispose();
        }

        public bool IsComplete()
        {
            lock (requests)
            {
                foreach (var complete in requests.Values)
                {
                    if (!complete)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void WaitForComplete()
        {
            Thread.Sleep(1000);
            for (var i = 0; !IsComplete() && i < Config.Instance.WaitTimeout; i++)
            {
                Thread.Sleep(1000);
            }
            var requestIds = new List<string>(requests.Count);
            lock (requests)
            {
                foreach (var p in requests)
                {
                    if (p.Value)
                    {
                        requestIds.Add(p.Key);
                    }
                }
                requests.Clear();
            }
            foreach (var requestId in requestIds)
            {
                var data = network.GetResponseBody(requestId);
                var sb = new StringBuilder()
                    .Append("const scr = document.createElement('script');")
                    .Append("scr.setAttribute('data-dump', '")
                    .Append(requestId)
                    .Append("');")
                    .Append("scr.type = 'text/plain';")
                    .Append("scr.text = decodeURIComponent('")
                    .Append(MinimalUrlEncode(data.Body))
                    .Append("');")
                    .Append("document.body.appendChild(scr);");
                driver.ExecuteScript(sb.ToString());
            }
        }

        public void ClearDump()
        {
            driver.ExecuteScript("document.querySelectorAll('[data-dump]').forEach(dump => dump.remove())");
        }

        private void OnRequestWillBeSent(string requestId, string type, string requestUrl)
        {
            if (predicate(requestUrl))
            {
                lock (requests)
                {
                    requests[requestId] = false;
                }
            }
        }

        private void OnLoadingFailed(string requestId, string errorText)
        {
            lock (requests)
            {
                requests.Remove(requestId);
            }
        }

        private void OnLoadingFinished(string requestId)
        {
            lock (requests)
            {
                if (requests.ContainsKey(requestId))
                {
                    requests[requestId] = true;
                }
            }
        }

        public static string MinimalUrlEncode(string data)
        {
            var sb = new StringBuilder(data.Length + data.Length / 4);
            foreach (var ch in data)
            {
                switch (ch)
                {
                    case '%':
                        sb.Append("%25");
                        break;
                    case '\n':
                        sb.Append("%0A");
                        break;
                    case '\r':
                        sb.Append("%0D");
                        break;
                    case '\'':
                        sb.Append("%27");
                        break;
                    case '\\':
                        sb.Append("%5C");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
