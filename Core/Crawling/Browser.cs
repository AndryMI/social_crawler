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
        private static readonly ChromeDriverService service = InitService();

        private BrowserNetwork network = null;
        private ChromeDriver driver = null;
        private string profile = null;

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
                options.AddArgument("--lang=en");
                if (profile != null)
                {
                    options.AddArgument("user-data-dir=" + Path.GetFullPath("Browsers/" + profile));
                }
                this.driver = new ChromeDriver(service, options, timeout);
                this.network = new BrowserNetwork(driver);
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
            images.WaitForLoading();
            return result;
        }

        public void Close()
        {
            driver?.Quit();
            driver = null;
            network?.Dispose();
            network = null;
            profile = null;
        }

        private static ChromeDriverService InitService()
        {
            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            return service;
        }
    }
}
