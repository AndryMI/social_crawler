using Newtonsoft.Json;
using Serilog;
using System.Net;

namespace Core
{
    public class ApiServerClient : ApiClient
    {
        private static string SharedAuthHeader;

        public ApiServerClient() : base(Config.Instance.ApiUrl)
        {
        }

        public override string AuthHeader
        {
            get => SharedAuthHeader;
            set => SharedAuthHeader = value;
        }

        public override string Request(string method, string path, object data = null)
        {
            try
            {
                return base.Request(method, path, data);
            }
            catch (WebException e)
            {
                if (e.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Login();
                    return base.Request(method, path, data);
                }
                Log.Warning(e, "{ErrorResponse}", e.Response != null ? ReadAllText(e.Response) : null);
                throw;
            }
        }

        public void Login()
        {
            var json = base.Request("POST", "/auth/login", new { email = Config.Instance.ApiMail, password = Config.Instance.ApiPass });
            var auth = JsonConvert.DeserializeObject<Auth>(json);
            AuthHeader = $"{auth.token_type} {auth.access_token}";
        }

        private class Auth
        {
            public string access_token = null;
            public string token_type = null;
        }
    }
}
