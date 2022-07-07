using Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace VK.Crawling
{
    public static class DriverExtensions
    {
        public static void WaitForPageLayout(this ChromeDriver driver)
        {
            Thread.Sleep(100);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Config.Instance.WaitTimeout));
            wait.Until(x => x.FindElements(By.Id("page_layout")).Count > 0);
        }

        public static void WaitForPostsLoading(this ChromeDriver driver)
        {
            var script =
                "var more = document.getElementById('wall_more_link');" +
                "return more ? more.innerText.trim() : 'stop'";

            Thread.Sleep(100);
            while (driver.ExecuteScript(script).Equals(""))
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Config.Instance.WaitTimeout));
                wait.Until(x => !driver.ExecuteScript(script).Equals(""));
                Thread.Sleep(100);
            }
        }

        public static void ScrollToLoadMore(this ChromeDriver driver)
        {
            driver.ExecuteScript("document.getElementById('wall_more_link')?.click(); document.getElementById('wall_more_link')?.scrollIntoView()");
        }
    }
}
