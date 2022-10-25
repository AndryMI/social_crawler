using System;

namespace Core.Browsers.Specific
{
    public abstract class Network : DevTools<Network>, IDisposable
    {
        public delegate void RequestWillBeSent(string requestId, string type, string requestUrl);
        public delegate void ResponseReceived(string requestId, string type, Response response);
        public delegate void LoadingFailed(string requestId, string errorText);
        public delegate void LoadingFinished(string requestId);

        public RequestWillBeSent OnRequestWillBeSent;
        public ResponseReceived OnResponseReceived;
        public LoadingFailed OnLoadingFailed;
        public LoadingFinished OnLoadingFinished;

        public abstract ResponseBody GetResponseBody(string requestId);
        public abstract void Dispose();

        public struct ResponseBody
        {
            public string Body;
            public bool Base64Encoded;
        }

        public class Response
        {
            public long Status { get; set; }
            public string MimeType { get; set; }
            public bool? FromDiskCache { get; set; }
            public bool? FromPrefetchCache { get; set; }
            public bool? FromServiceWorker { get; set; }
        }
    }
}
