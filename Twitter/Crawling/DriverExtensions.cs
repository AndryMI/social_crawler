using Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace Twitter.Crawling
{
    public static class DriverExtensions
    {
        public static void WaitForLoading(this ChromeDriver driver)
        {
            Thread.Sleep(100);
            while (driver.FindElements(By.CssSelector("[role=\"progressbar\"]:not([style])")).Count > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Config.Instance.WaitTimeout));
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
