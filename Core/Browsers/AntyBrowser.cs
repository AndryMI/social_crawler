using Core.Crawling;
using OpenQA.Selenium.Chrome;

namespace Core.Browsers
{
    public static class AntyBrowser
    {
        private static readonly AntyLocalApi api = new AntyLocalApi();
        private static string LastVersion;

        public static ChromeDriver Start(BrowserProfile profile)
        {
            var automation = api.Start(profile.Name);

            var options = new ChromeOptions
            {
                DebuggerAddress = "127.0.0.1:" + automation.port
            };

            return DriverService.Run(ref LastVersion, options);
        }
    }
}
