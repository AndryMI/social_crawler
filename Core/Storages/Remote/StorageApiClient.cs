using Core.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Core.Storages
{
    public class StorageApiClient : ApiClient
    {
        public StorageApiClient() : base(Config.Instance.ApiUrl)
        {
        }

        public void StoreProfiles(List<IProfileInfo> data)
        {
            if (data.Count > 0)
            {
                Request("POST", "/crawler/profiles", data);
            }
        }

        public void StorePosts(List<IPostInfo> data)
        {
            if (data.Count > 0)
            {
                Request("POST", "/crawler/posts", data);
            }
        }

        public void StoreComments(List<ICommentInfo> data)
        {
            if (data.Count > 0)
            {
                Request("POST", "/crawler/comments", data);
            }
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
