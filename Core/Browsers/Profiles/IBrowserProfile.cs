using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;

namespace Core.Browsers.Profiles
{
    public interface IBrowserProfile
    {
        [JsonProperty]
        string Type { get; }

        [JsonProperty]
        string Id { get; }

        ChromeDriver Start();
    }
}
