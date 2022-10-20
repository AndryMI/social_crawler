using Newtonsoft.Json;
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

        public virtual string Request(string method, string path, object data = null)
        {
            var multipart = data as MultipartData;
            var request = (HttpWebRequest)WebRequest.Create(host + path);
            request.Method = method;
            request.ContentType = multipart?.ContentType ?? "application/json";

            if (!string.IsNullOrEmpty(AuthHeader))
            {
                request.Headers.Add(HttpRequestHeader.Authorization, AuthHeader);
            }
            if (data != null)
            {
                using (var stream = request.GetRequestStream())
                using (var writer = new StreamWriter(stream))
                {
                    if (multipart == null)
                    {
                        var json = JsonConvert.SerializeObject(data);
                        log.Verbose("Send {method} {path} {json}", method, path, json);
                        writer.Write(json);
                    }
                    else
                    {
                        log.Verbose("Send {method} {path} {multipart}", method, path, multipart);
                        multipart.Serialize(writer);
                    }
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
