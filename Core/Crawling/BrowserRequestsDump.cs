using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V104;
using OpenQA.Selenium.DevTools.V104.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Core.Crawling
{
    public class BrowserRequestsDump : IDisposable
    {
        private readonly Dictionary<string, bool> requests = new Dictionary<string, bool>();
        private readonly DevToolsSessionDomains domains;
        private readonly Predicate<string> predicate;
        private readonly ChromeDriver driver;

        public BrowserRequestsDump(ChromeDriver driver, Predicate<string> predicate)
        {
            this.predicate = predicate;
            this.driver = driver;

            domains = driver.GetDevToolsSession().GetVersionSpecificDomains<DevToolsSessionDomains>();
            domains.Network.RequestWillBeSent += OnRequestWillBeSent;
            domains.Network.LoadingFailed += OnLoadingFailed;
            domains.Network.LoadingFinished += OnLoadingFinished;
        }

        public void Dispose()
        {
            domains.Network.RequestWillBeSent -= OnRequestWillBeSent;
            domains.Network.LoadingFailed -= OnLoadingFailed;
            domains.Network.LoadingFinished -= OnLoadingFinished;
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
            for (var i = 0; !IsComplete() && i < Config.Instance.WaitTimeout; i++)
            {
                Thread.Sleep(1000);
            }
            lock (requests)
            {
                foreach (var p in requests)
                {
                    if (p.Value)
                    {
                        var data = domains.Network.GetResponseBody(new GetResponseBodyCommandSettings { RequestId = p.Key }).Result;
                        var sb = new StringBuilder()
                            .Append("const scr = document.createElement('script');")
                            .Append("scr.setAttribute('data-dump', '")
                            .Append(p.Key)
                            .Append("');")
                            .Append("scr.type = 'text/plain';")
                            .Append("scr.text = '")
                            .Append(data.Base64Encoded ? data.Body : Convert.ToBase64String(Encoding.UTF8.GetBytes(data.Body)))
                            .Append("';")
                            .Append("document.body.appendChild(scr);");
                        driver.ExecuteScript(sb.ToString());
                    }
                }
                requests.Clear();
            }
        }

        private void OnRequestWillBeSent(object sender, RequestWillBeSentEventArgs e)
        {
            if (predicate(e.Request.Url))
            {
                lock (requests)
                {
                    requests[e.RequestId] = false;
                }
            }
        }

        private void OnLoadingFailed(object sender, LoadingFailedEventArgs e)
        {
            lock (requests)
            {
                requests.Remove(e.RequestId);
            }
        }

        private void OnLoadingFinished(object sender, LoadingFinishedEventArgs e)
        {
            lock (requests)
            {
                if (requests.ContainsKey(e.RequestId))
                {
                    requests[e.RequestId] = true;
                }
            }
        }
    }
}
