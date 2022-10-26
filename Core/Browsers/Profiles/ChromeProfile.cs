using Core.Browsers.DevTools;
using OpenQA.Selenium.Chrome;
using System.IO;

namespace Core.Browsers.Profiles
{
    public class ChromeProfile : IBrowserProfile
    {
        private static string LastVersion;

        public string Type => "Chrome";
        public string Id { get; private set; }

        public ChromeProfile(string id = null)
        {
            Id = id;
        }

        public ChromeDriver Start()
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
            if (!string.IsNullOrWhiteSpace(Id))
            {
                options.AddArgument("user-data-dir=" + Path.GetFullPath("Browsers/" + Id));
            }

            return DriverService.Run(ref LastVersion, options);
        }
    }
}
