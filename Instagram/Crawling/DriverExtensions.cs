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
        public static void WaitForLoading(this ChromeDriver driver)
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
    }
}
