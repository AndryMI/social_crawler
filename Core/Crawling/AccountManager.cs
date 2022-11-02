using Newtonsoft.Json;

namespace Core.Crawling
{
    public class AccountManager
    {
        private readonly ApiServerClient client = new ApiServerClient();

        public Account Take<T>() where T : Account
        {
            var json = client.Request("POST", "/crawler/account/take", new JsonData(new
            {
                guid = Config.Guid,
                type = typeof(T).Name,
                crawler = IpInfo.My()
            }));
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Release(Account account)
        {
            if (account != null)
            {
                client.Request("POST", "/crawler/account/release", new JsonData(new
                {
                    guid = Config.Guid,
                    crawler = IpInfo.My(),
                    account,
                }));
            }
        }
    }
}
