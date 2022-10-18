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
            public override string BrowserProfile => profile;

            private readonly string profile;

            public Wrapper(string profile)
            {
                this.profile = profile;
            }

            public override void Login(ChromeDriver driver)
            {
            }
        }
    }
}
