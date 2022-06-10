using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace RD_Team_TweetMonitor
{
    public static class DriverExtensions
    {
        private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(120);

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

        public static void InitTimeouts(this ChromeDriver driver)
        {
            driver.Manage().Timeouts().AsynchronousJavaScript = WaitTimeout;
        }

        public static void WaitForPassword(this ChromeDriver driver)
        {
            Thread.Sleep(100);
            var wait = new WebDriverWait(driver, WaitTimeout);
            wait.Until(x => x.FindElements(By.TagName("[type=\"password\"]")).Count > 0);
        }

        public static void WaitForLoading(this ChromeDriver driver)
        {
            Thread.Sleep(100);
            while (driver.FindElements(By.CssSelector("[role=\"progressbar\"]:not([style])")).Count > 0)
            {
                var wait = new WebDriverWait(driver, WaitTimeout);
                wait.Until(x => x.FindElements(By.CssSelector("[role=\"progressbar\"]:not([style])")).Count == 0);
                Thread.Sleep(100);
            }
        }

        public static void ScrollToLastArticle(this ChromeDriver driver)
        {
            driver.ExecuteScript("Array.from(document.querySelectorAll('article')).pop()?.scrollIntoView()");
        }

        public static void ScrollToLastFollower(this ChromeDriver driver)
        {
            driver.ExecuteScript("Array.from(document.querySelectorAll('[data-testid=UserCell]')).pop()?.scrollIntoView()");
        }
    }
}
