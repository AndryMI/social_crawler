using Serilog;
using System.IO;
using System.Net;
using System.Text;

namespace Core
{
    public class ApiClient
    {
        private readonly ILogger log;
        private readonly string host;

        public virtual string AuthHeader { get; set; }

        public ApiClient(string host)
        {
            this.log = Log.Logger.ForContext(GetType());
            this.host = host;
        }

        public virtual string Request(string method, string path, IRequestData data = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(host + path);
            request.Method = method;
            request.ContentType = data?.ContentType;

            if (!string.IsNullOrEmpty(AuthHeader))
            {
                request.Headers.Add(HttpRequestHeader.Authorization, AuthHeader);
            }

            log.Verbose("Send {method} {path} {data}", method, path, data);

            if (data != null)
            {
                using (var stream = request.GetRequestStream())
                using (var writer = new StreamWriter(stream))
                {
                    data.Serialize(writer);
                }
            }

            var response = request.GetResponse();
            var result = ReadAllText(response);
            log.Verbose("Receive {json}", result);

            return result;
        }

        protected static string ReadAllText(WebResponse response)
        {
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}