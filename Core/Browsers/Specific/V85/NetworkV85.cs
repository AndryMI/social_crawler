using OpenQA.Selenium.DevTools.V85;
using OpenQA.Selenium.DevTools.V85.Network;

namespace Core.Browsers.Specific.V85
{
    public class NetworkV85 : Network
    {
        private readonly DevToolsSessionDomains domains;

        public NetworkV85(V85Domains domain)
        {
            domains = (DevToolsSessionDomains)domain.VersionSpecificDomains;
            domains.Network.Enable(new EnableCommandSettings()).Wait();
            domains.Network.RequestWillBeSent += OnRequestWillBeSentImpl;
            domains.Network.ResponseReceived += OnResponseReceivedImpl;
            domains.Network.LoadingFailed += OnLoadingFailedImpl;
            domains.Network.LoadingFinished += OnLoadingFinishedImpl;
        }

        public override void Dispose()
        {
            domains.Network.RequestWillBeSent -= OnRequestWillBeSentImpl;
            domains.Network.ResponseReceived -= OnResponseReceivedImpl;
            domains.Network.LoadingFailed -= OnLoadingFailedImpl;
            domains.Network.LoadingFinished -= OnLoadingFinishedImpl;
        }

        public override ResponseBody GetResponseBody(string requestId)
        {
            var response = domains.Network.GetResponseBody(new GetResponseBodyCommandSettings { RequestId = requestId }).Result;
            return new ResponseBody
            {
                Body = response.Body,
                Base64Encoded = response.Base64Encoded,
            };
        }

        private void OnRequestWillBeSentImpl(object sender, RequestWillBeSentEventArgs e)
        {
            OnRequestWillBeSent?.Invoke(e.RequestId, e.Type.ToString(), e.Request.Url);
        }

        private void OnResponseReceivedImpl(object sender, ResponseReceivedEventArgs e)
        {
            OnResponseReceived?.Invoke(e.RequestId, e.Type.ToString(), new Response
            {
                Status = e.Response.Status,
                MimeType = e.Response.MimeType,
                FromDiskCache = e.Response.FromDiskCache,
                FromPrefetchCache = e.Response.FromPrefetchCache,
                FromServiceWorker = e.Response.FromServiceWorker,
            });
        }

        private void OnLoadingFailedImpl(object sender, LoadingFailedEventArgs e)
        {
            OnLoadingFailed?.Invoke(e.RequestId, e.ErrorText);
        }

        private void OnLoadingFinishedImpl(object sender, LoadingFinishedEventArgs e)
        {
            OnLoadingFinished?.Invoke(e.RequestId);
        }
    }
}
