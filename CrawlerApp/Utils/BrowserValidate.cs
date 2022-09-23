using Core;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Text.RegularExpressions;

namespace CrawlerApp
{
    public static class BrowserValidate
    {
        public static void Run()
        {
            var timeout = TimeSpan.FromSeconds(Config.Instance.WaitTimeout);
            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("--lang=en");
            options.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);

            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            var driver = new ChromeDriver(service, options, timeout);
            var capabilities = JsonConvert.DeserializeObject<Caps>(driver.Capabilities.ToString());
            driver.Quit();

            var ver1 = Regex.Match(capabilities.BrowserVersion, @"\d+.\d+.\d+").ToString();
            var ver2 = Regex.Match(capabilities.Driver.Version, @"\d+.\d+.\d+").ToString();

            if (ver1 != ver2)
            {
                throw new Exception($"Browser ({ver1}) and Driver ({ver2}) versions may be incompatible");
            }
        }

        private class Caps
        {
            [JsonProperty("browserVersion")]
            public readonly string BrowserVersion = null;
            [JsonProperty("chrome")]
            public readonly DriverCaps Driver = null;
        }

        private class DriverCaps
        {
            [JsonProperty("chromedriverVersion")]
            public readonly string Version = null;
        }
    }
}
