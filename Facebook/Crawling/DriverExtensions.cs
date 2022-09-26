using Core;
using Core.Crawling;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace Facebook.Crawling
{
    public static class DriverExtensions
    {
        public static void WaitForMain(this ChromeDriver driver)
        {
            Thread.Sleep(100);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Config.Instance.WaitTimeout));
            wait.Until(x => x.FindElements(By.CssSelector("[role=main]")).Count > 0);
        }

        public static void DumpPrefetchedRequests(this ChromeDriver driver)
        {
            driver.InjectUtils("Scripts/JsUtils.js");
            driver.InjectUtils("Scripts/Facebook/Utils.js");
            driver.ExecuteScript("return __DumpPrefetchedFacebookRequests()");
        }

        public static void ScrollToPageBottom(this ChromeDriver driver)
        {
            driver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
        }
    }
}
