using Core.Crawling;
using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;
using System.Threading;

namespace Core
{
    public static class ChromeBrowser
    {
        private const string ChromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
        private static ulong nextUid = 0;

        public static ChromeDriver Start(BrowserProfile profile)
        {
            var timeout = TimeSpan.FromSeconds(Config.Instance.WaitTimeout);
            var options = new ChromeOptions();
            options.AddArgument("--crawler-thread=" + Thread.CurrentThread.ManagedThreadId);
            options.AddArgument("--crawler-instance=" + nextUid++);
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

            var ver = FileVersionInfo.GetVersionInfo(ChromePath).FileVersion;

            return new ChromeDriver(DriverService.AutoRun(ver), options, timeout);
        }
    }
}
