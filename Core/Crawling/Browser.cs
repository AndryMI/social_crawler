﻿using Core.Browsers.Profiles;
using Core.Storages;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Serilog;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Core.Crawling
{
    public class Browser
    {
        private static readonly IBrowserProfile Anonymous = new AnonymousProfile();

        private readonly IMediaStorage media;
        private BrowserRequestsDump dumper = null;
        private BrowserNetwork network = null;
        private BrowserConsole console = null;
        private IBrowserProfile profile = Anonymous;
        private ChromeDriver driver = null;

        public Browser(IMediaStorage media)
        {
            this.media = media;
        }

        public ChromeDriver Driver<T>(string url) where T : Account, new()
        {
            var account = Accounts<T>.Instance.Get(url);
            var driver = Driver(account);
            account.Login(driver);
            return driver;
        }

        public ChromeDriver Driver(Account account = null)
        {
            if (!Equals(profile, account?.BrowserProfile ?? Anonymous))
            {
                Close();
                profile = account?.BrowserProfile ?? Anonymous;
            }
            if (driver == null)
            {
                try
                {
                    driver = profile.Start();
                    network = new BrowserNetwork(driver);
                    console = new BrowserConsole(driver);

                    //TODO tempfix https://github.com/SeleniumHQ/selenium/issues/10799
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    throw new TryLaterException($"Failed to start browser: {profile.Type} {profile.Id}", e);
                }
            }
            dumper?.Dispose();
            dumper = null;
            network.Clear();
            network.RequestCounter = account?.Limits;
            return driver;
        }

        public void InjectUtils(string path)
        {
            driver.InjectUtils(path);
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

        public BrowserRequestsDump DumpRequests(Predicate<string> predicate)
        {
            if (dumper != null)
            {
                Log.Error(new Exception(), "Overriding requests dumper");
                dumper.Dispose();
            }
            return dumper = new BrowserRequestsDump(driver, predicate);
        }

        public void Close()
        {
            driver?.Close();
            driver?.Dispose();
            driver = null;
            dumper?.Dispose();
            dumper = null;
            network?.Dispose();
            network = null;
            console?.Dispose();
            console = null;
            profile = Anonymous;
        }

        private static bool Equals(IBrowserProfile a, IBrowserProfile b)
        {
            return a.Id == b.Id && a.Type == b.Type;
        }
    }
}
