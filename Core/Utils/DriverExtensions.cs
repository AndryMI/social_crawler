using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Threading;

namespace Core.Crawling
{
    public static class DriverExtensions
    {
        public static void WaitForReady(this ChromeDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Config.Instance.WaitTimeout));
            wait.Until(x => driver.ExecuteScript("return document.readyState").Equals("complete"));
            Thread.Sleep(100);
        }

        public static IWebElement TryFindElement(this ChromeDriver driver, By by)
        {
            try
            {
                return driver.FindElement(by);
            }
            catch
            {
                return null;
            }
        }

        public static void DeleteCurrentCookies(this ChromeDriver driver)
        {
            driver.ExecuteScript(File.ReadAllText("Scripts/DeleteAllCookies.js"));
        }
    }
}
