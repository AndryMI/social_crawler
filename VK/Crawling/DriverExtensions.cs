using Core;
using Core.Crawling;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace VK.Crawling
{
    public static class DriverExtensions
    {
        public static void SwitchToEnglish(this ChromeDriver driver)
        {
            var cookie = driver.Manage().Cookies.GetCookieNamed("remixlang");
            if (cookie == null || cookie.Value != "3")
            {
                driver.Manage().Cookies.AddCookie(new Cookie("remixlang", "3", ".vk.com", "/", DateTime.Now.AddYears(1), true, false, "None"));
                driver.Url = driver.Url;
            }
        }

        public static bool IsPrivatePage(this ChromeDriver driver)
        {
            return driver.TryFindElement(By.CssSelector(".profile_deleted_text")) != null;
        }

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

        public static void WaitForRepliesLoading(this ChromeDriver driver)
        {
            Thread.Sleep(100);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Config.Instance.WaitTimeout));
            wait.Until(x => x.FindElements(By.CssSelector(".progress_inline.replies_next_loader")).Count == 0);
        }

        public static void ScrollToNextReplies(this ChromeDriver driver)
        {
            driver.ExecuteScript("var next = document.querySelector('.replies_next'); next?.scrollIntoView(); next?.click();");
        }
    }
}
