using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Api
{
    public static class API
    {
        private static readonly ApiClient client = new ApiClient(Config.Instance.ApiUrl);

        public static void Login()
        {
            var auth = client.Post<Auth>("/auth/login", new { email = Config.Instance.ApiMail, password = Config.Instance.ApiPass });
            client.AuthHeader = $"{auth.token_type} {auth.access_token}";
        }

        public static void Logout()
        {
            client.Request("POST", "/auth/logout");
            client.AuthHeader = null;
        }

        private class Auth
        {
            public string access_token;
            public string token_type;
        }
    }
}
