using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace Core
{
    public class ApiServerClient : ApiClient
    {
        public ApiServerClient() : base(Config.Instance.ApiUrl)
        {
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
                Debug.WriteLine(ReadAllText(e.Response));
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
