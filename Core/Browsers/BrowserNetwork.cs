using Core.Browsers.DevTools;
using OpenQA.Selenium.Chrome;
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
        private readonly Network network;

        public IRequestCounter RequestCounter;

        public BrowserNetwork(ChromeDriver driver)
        {
            log = Log.Logger.ForContext<BrowserNetwork>().ForContext("SessionId", driver.SessionId);
            network = Network.Create(driver);
            network.OnRequestWillBeSent = OnRequestWillBeSent;
            network.OnResponseReceived = OnResponseReceived;
            network.OnLoadingFailed = OnLoadingFailed;
            network.OnLoadingFinished = OnLoadingFinished;
            log.Verbose("Net Init {Type}", network.GetType());
        }

        public void Dispose()
        {
            network.Dispose();
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
                var data = network.GetResponseBody(item.id);
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

        private void OnRequestWillBeSent(string requestId, string type, string requestUrl)
        {
            log.Verbose("Net Send {RequestId} {Type} {Url}", requestId, type, requestUrl);
            AddItem(new Item { id = requestId, url = requestUrl, type = type });
            RequestCounter?.OnRequest(requestUrl);
        }

        private void OnResponseReceived(string requestId, string type, Network.Response response)
        {
            log.Verbose("Net Response {RequestId} {Type} {@Response}", requestId, type, response);
            var item = GetItem(requestId);
            if (item != null)
            {
                item.type = type;
                item.mime = response.MimeType;
            }
        }

        private void OnLoadingFailed(string requestId, string errorText)
        {
            log.Verbose("Net Failed {RequestId} {Error}", requestId, errorText);
            var item = GetItem(requestId);
            if (item != null)
            {
                item.status = Status.Fail;
                item.error = errorText;
            }
        }

        private void OnLoadingFinished(string requestId)
        {
            log.Verbose("Net Complete {RequestId}", requestId);
            var item = GetItem(requestId);
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
            public string type;
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
