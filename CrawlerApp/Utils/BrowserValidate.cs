using Core;
using Core.Crawling;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CrawlerApp
{
    public static class BrowserValidate
    {
        public static void Run()
        {
            SeleniumDevToolsVersion();
            BrowserAndDriverVersions();
        }

        public static void SeleniumDevToolsVersion()
        {
            var regex = new Regex(@"Selenium.DevTools.V(\d+)", RegexOptions.Compiled);

            var latest = typeof(DevToolsSession).Assembly.GetExportedTypes()
                .Select(x => regex.Match(x.Namespace))
                .Max(x => x.Success && int.TryParse(x.Groups[1].Value, out var ver) ? ver : int.MinValue);

            var actual = typeof(Browser).IterateDependentTypes()
                .Select(x => regex.Match(x.Namespace ?? ""))
                .Min(x => x.Success && int.TryParse(x.Groups[1].Value, out var ver) ? ver : int.MaxValue);

            if (actual != latest)
            {
                throw new Exception($"Used (V{actual}) and Latest (V{latest}) DevTools versions may be incompatible");
            }
        }

        public static void BrowserAndDriverVersions()
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

        private static IEnumerable<Type> IterateDependentTypes(this Type root)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            var examined = new HashSet<Type>();
            var queue = new Queue<Type>();
            examined.Add(root);
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var type = queue.Dequeue();

                yield return type;

                foreach (var field in type.GetFields(flags))
                {
                    if (examined.Add(field.FieldType))
                    {
                        queue.Enqueue(field.FieldType);
                    }
                }
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
