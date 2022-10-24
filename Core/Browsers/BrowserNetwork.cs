using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V106;
using OpenQA.Selenium.DevTools.V106.Network;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Crawling
{
    public interface IRequestCounter
    {
        void OnRequest(string url);
    }

    public class BrowserNetwork : IDisposable
    {
        private readonly ILogger log;
        private readonly Dictionary<string, Item> requests = new Dictionary<string, Item>();
        private readonly DevToolsSessionDomains domains;

        public IRequestCounter RequestCounter;

        public BrowserNetwork(ChromeDriver driver)
        {
            log = Log.Logger.ForContext<BrowserNetwork>().ForContext("SessionId", driver.SessionId);
            log.Verbose("Net Init");
            domains = driver.GetDevToolsSession().GetVersionSpecificDomains<DevToolsSessionDomains>();
            domains.Network.Enable(new EnableCommandSettings()).Wait();
            domains.Network.RequestWillBeSent += OnRequestWillBeSent;
            domains.Network.ResponseReceived += OnResponseReceived;
            domains.Network.LoadingFailed += OnLoadingFailed;
            domains.Network.LoadingFinished += OnLoadingFinished;
        }

        public void Dispose()
        {
            domains.Network.RequestWillBeSent -= OnRequestWillBeSent;
            domains.Network.ResponseReceived -= OnResponseReceived;
            domains.Network.LoadingFailed -= OnLoadingFailed;
            domains.Network.LoadingFinished -= OnLoadingFinished;
        }

        public void Clear()
        {
            lock (requests)
            {
                requests.Clear();
            }
        }

        public bool IsComplete(List<string> urls)
        {
            lock (requests)
            {
                foreach (var url in urls)
                {
                    if (requests.TryGetValue(url, out Item item) && !item.IsComplete)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public byte[] GetResponseBody(string url)
        {
            var item = GetItem(url);
            if (item != null && item.status == Status.Success)
            {
                var data = domains.Network.GetResponseBody(new GetResponseBodyCommandSettings { RequestId = item.id }).Result;
                return data.Base64Encoded ? Convert.FromBase64String(data.Body) : Encoding.UTF8.GetBytes(data.Body);
            }
            log.Warning("Response body not found: {url}", url);
            return null;
        }

        public string GetMimeType(string url)
        {
            var item = GetItem(url);
            if (item != null)
            {
                return item.mime;
            }
            log.Warning("Response mime-type not found: {url}", url);
            return null;
        }

        private void OnRequestWillBeSent(object sender, RequestWillBeSentEventArgs e)
        {
            log.Verbose("Net Send {RequestId} {Type} {Url}", e.RequestId, e.Type, e.Request.Url);
            AddItem(new Item { id = e.RequestId, url = e.Request.Url, type = e.Type });
            RequestCounter?.OnRequest(e.Request.Url);
        }

        private void OnResponseReceived(object sender, ResponseReceivedEventArgs e)
        {
            log.Verbose("Net Response {RequestId} {Type} {@Response}", e.RequestId, e.Type, new { e.Response.Status, e.Response.FromDiskCache, e.Response.FromPrefetchCache, e.Response.FromServiceWorker });
            var item = GetItem(e.RequestId);
            if (item != null)
            {
                item.type = e.Type;
                item.mime = e.Response.MimeType;
            }
        }

        private void OnLoadingFailed(object sender, LoadingFailedEventArgs e)
        {
            log.Verbose("Net Failed {RequestId} {Error}", e.RequestId, e.ErrorText);
            var item = GetItem(e.RequestId);
            if (item != null)
            {
                item.status = Status.Fail;
                item.error = e.ErrorText;
            }
        }

        private void OnLoadingFinished(object sender, LoadingFinishedEventArgs e)
        {
            log.Verbose("Net Complete {RequestId}", e.RequestId);
            var item = GetItem(e.RequestId);
            if (item != null)
            {
                item.status = Status.Success;
            }
        }

        private void AddItem(Item item)
        {
            lock (requests)
            {
                requests[item.id] = item;
                requests[item.url] = item;
            }
        }

        private Item GetItem(string requestId)
        {
            lock (requests)
            {
                return requests.TryGetValue(requestId, out var item) ? item : null;
            }
        }

        private enum Status
        {
            Loading,
            Success,
            Fail,
        }

        private class Item
        {
            public Status status = Status.Loading;
            public string id;
            public string url;
            public ResourceType? type;
            public string mime;
            public string error;

            public bool IsComplete => status != Status.Loading;

            public override string ToString()
            {
                return $"{status} {type} {url}";
            }
        }
    }
}
