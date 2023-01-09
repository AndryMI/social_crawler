using Serilog;
using System.IO;
using System.IO.Compression;
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
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(host + path);
                request.Method = method;
                request.ContentType = data?.ContentType;
                request.Headers.Add("Accept-Encoding", "gzip");
                request.ServerCertificateValidationCallback = (a, b, c, d) => true;

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
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    Log.Verbose("Failed {Responce}", ReadAllText(e.Response));
                }
                throw;
            }
        }

        protected static string ReadAllText(WebResponse response)
        {
            var encoding = response.Headers[HttpResponseHeader.ContentEncoding];

            using (var stream = response.GetResponseStream())
            using (var decoded = encoding == "gzip" ? new GZipStream(stream, CompressionMode.Decompress) : stream)
            using (var reader = new StreamReader(decoded, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}