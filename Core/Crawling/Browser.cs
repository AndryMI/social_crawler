using Core.Storages;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Text;

namespace Core.Crawling
{
    public class Browser
    {
        private readonly IMediaStorage media;
        private BrowserNetwork network = null;
        private BrowserConsole console = null;
        private ChromeDriver driver = null;
        private string profile = null;

        public Browser(IMediaStorage media)
        {
            this.media = media;
        }

        public ChromeDriver Driver<T>(string url) where T : Account, new()
        {
            var account = Accounts<T>.Instance.Get(url);
            var driver = Driver(account.BrowserProfile);
            account.Login(driver);
            return driver;
        }

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

                //TODO options.AddArgument("--proxy-server=88.119.175.141:3128");

                options.AddArgument("--lang=en");
                if (Config.Instance.BrowserHeadless)
                {
                    options.AddArgument("headless");
                }
                if (profile != null)
                {
                    options.AddArgument("user-data-dir=" + Path.GetFullPath("Browsers/" + profile));
                }
                var service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;

                this.driver = new ChromeDriver(service, options, timeout);
                this.network = new BrowserNetwork(driver);
                this.console = new BrowserConsole(driver);
                this.profile = profile;

                //TODO tempfix https://github.com/SeleniumHQ/selenium/issues/10799
                if (profile != null)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
            network.Clear();
            return driver;
        }

        public T RunCollector<T>(string path)
        {
            var script = new StringBuilder()
                .Append("try {")
                .Append(" return (function __FN__() {")
                .Append(File.ReadAllText(path))
                .Append(" })()")
                .Append("}")
                .Append("catch (error) {")
                .Append(" return '!' + error.stack")
                .Append("}");

            var json = driver.ExecuteScript(script.ToString()) as string;
            if (json.StartsWith("!"))
            {
                throw new JavaScriptException(json.Replace("__FN__", path));
            }
            var images = new ImageUrlCollector(network);
            var result = JsonConvert.DeserializeObject<T>(json, images);
            if (media.WaitForBrowserLoading)
            {
                images.WaitForLoading();
            }
            foreach (var image in images)
            {
                media.StoreImage(image);
            }
            return result;
        }

        public void Close()
        {
            driver?.Quit();
            driver = null;
            network?.Dispose();
            network = null;
            console?.Dispose();
            console = null;
            profile = null;
        }
    }
}
