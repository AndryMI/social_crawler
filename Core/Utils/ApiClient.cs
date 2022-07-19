using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Core
{
    public class ApiClient
    {
        private readonly string host;

        public ApiClient(string host)
        {
            this.host = host;
        }

        public string AuthHeader { get; set; }

        public T Get<T>(string path)
        {
            return Request<T>("GET", path, null);
        }

        public T Put<T>(string path, object data = null)
        {
            return Request<T>("PUT", path, data);
        }

        public T Post<T>(string path, object data = null)
        {
            return Request<T>("POST", path, data);
        }

        public T Request<T>(string method, string path, object data = null)
        {
            return JsonConvert.DeserializeObject<T>(Request(method, path, data));
        }

        public string Request(string method, string path, object data = null)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(host + path);
                request.Method = method;
                request.ContentType = "application/json";

                if (!string.IsNullOrEmpty(AuthHeader))
                {
                    request.Headers.Add(HttpRequestHeader.Authorization, AuthHeader);
                }
                if (data != null)
                {
                    using (var stream = request.GetRequestStream())
                    using (var writer = new StreamWriter(stream))
                    {
                        var json = JsonConvert.SerializeObject(data);
                        writer.Write(json);
                    }
                }

                var response = request.GetResponse();
                var result = ReadAllText(response);

                return result;
            }
            catch (WebException e)
            {
                var details = ReadAllText(e.Response);
                Debug.WriteLine(details);
                throw;
            }
        }

        private static string ReadAllText(WebResponse response)
        {
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
