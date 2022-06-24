using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace Core.Crawling
{
    public class Browser
    {
        private static readonly ChromeDriverService service = InitService();

        private ChromeDriver driver = null;
        private string profile = null;

        public ChromeDriver Driver(string profile = null)
        {
            if (this.profile != profile)
            {
                Close();
            }
            if (driver == null)
            {
                var timeout = TimeSpan.FromSeconds(Config.Instance.WaitTimeout);
                var options = new ChromeOptions();
                if (profile != null)
                {
                    options.AddArgument("user-data-dir=" + Path.GetFullPath("Browsers/" + profile));
                }
                this.driver = new ChromeDriver(service, options, timeout);
                this.profile = profile;
            }
            return driver;
        }

        public void Close()
        {
            if (driver != null)
            {
                driver.Quit();
                driver = null;
                profile = null;
            }
        }

        private static ChromeDriverService InitService()
        {
            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            return service;
        }
    }
}
