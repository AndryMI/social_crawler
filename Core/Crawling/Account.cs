using Core.Browsers.Profiles;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;

namespace Core.Crawling
{
    public abstract class Account
    {
        [JsonProperty("browser_profile", Required = Required.Always)]
        public IBrowserProfile BrowserProfile;

        [JsonProperty("_id", Required = Required.Always)]
        public readonly ObjectId Id;

        [JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
        public string UserId;

        [JsonProperty("login", NullValueHandling = NullValueHandling.Ignore)]
        public string Email;

        [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
        public string Password;

        [JsonProperty("limits", NullValueHandling = NullValueHandling.Ignore)]
        public readonly RequestLimits Limits;

        public abstract void Login(ChromeDriver driver);
        public virtual string GetDefaultTab() => null;
        public virtual RequestLimits GetRequestLimits() => null;

        protected Account()
        {
            Limits = GetRequestLimits();
        }
    }

    public class RandomProxyAccount : Account
    {
        public override string GetDefaultTab()
        {
            return "about:blank";
        }

        public override void Login(ChromeDriver driver)
        {
        }
    }
}
