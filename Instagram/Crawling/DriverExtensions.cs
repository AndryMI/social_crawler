using Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace Instagram.Crawling
{
    public static class DriverExtensions
    {
        public static void WaitForStoryLoading(this ChromeDriver driver)
        {
            Thread.Sleep(100);
            while (driver.FindElements(By.CssSelector("[data-visualcompletion=\"loading-state\"]:not([style])")).Count > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Config.Instance.WaitTimeout));
                wait.Until(x => x.FindElements(By.CssSelector("[data-visualcompletion=\"loading-state\"]:not([style])")).Count == 0);
                Thread.Sleep(100);
            }
        }

        public static void WaitForCommentsLoading(this ChromeDriver driver)
        {
            Thread.Sleep(100);
            while (driver.FindElements(By.CssSelector("[role=dialog] [data-visualcompletion=\"loading-state\"]")).Count > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Config.Instance.WaitTimeout));
                wait.Until(x => x.FindElements(By.CssSelector("[role=dialog] [data-visualcompletion=\"loading-state\"]")).Count == 0);
                Thread.Sleep(100);
            }
        }

        public static void WaitForDialogLoading(this ChromeDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Config.Instance.WaitTimeout));
            wait.Until(x => x.FindElements(By.CssSelector("[role=dialog] header")).Count > 0);
            Thread.Sleep(100);
        }

        public static void ScrollToLastComment(this ChromeDriver driver)
        {
            driver.ExecuteScript("document.querySelector('[role=dialog] article ul > li')?.scrollIntoView()");
        }

        public static void LoadMoreComments(this ChromeDriver driver)
        {
            driver.ExecuteScript("document.querySelector('[role=dialog] article ul > li')?.click()");
        }

        public static void TryOpenStories(this ChromeDriver driver)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
                wait.Until(x =>
                {
                    driver.ExecuteScript("document.querySelector('header img')?.click()");
                    Thread.Sleep(100);
                    return driver.Url.Contains("/stories/");
                });
            }
            catch (WebDriverTimeoutException) { }
        }
    }
}
