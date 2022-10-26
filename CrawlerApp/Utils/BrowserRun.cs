using Core.Browsers.Profiles;
using Core.Crawling;
using Core.Storages;
using OpenQA.Selenium.Chrome;

namespace CrawlerApp
{
    public static class BrowserRun
    {
        public static void Profile(string profile)
        {
            var browser = new Browser(new NullStorage());
            browser.Driver(new Wrapper(profile));
        }

        private class Wrapper : Account
        {
            public Wrapper(string profile)
            {
                BrowserProfile = new ChromeProfile(profile);
            }

            public override void Login(ChromeDriver driver)
            {
            }
        }
    }
}
