using Core.Crawling;
using Core.Browsers.DevTools;
using OpenQA.Selenium.Chrome;

namespace Core.Browsers
{
    public static class ChromeBrowser
    {
        private static string LastVersion;

        public static ChromeDriver Start(BrowserProfile profile)
        {
            var options = new ChromeOptions();
            options.AddArgument("--lang=en");
            options.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);

            //TODO options.AddArgument("--host-rules=MAP www.instagram.com example.com");
            //TODO options.AddArgument("--proxy-server=88.119.175.141:3128");

            if (Config.Instance.BrowserHeadless)
            {
                options.AddArgument("headless");
            }
            if (!profile.IsAnonymous)
            {
                options.AddArgument("user-data-dir=" + profile.FullPath);
            }

            return DriverService.Run(ref LastVersion, options);
        }
    }
}
