using Core.Crawling;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;

namespace VK.Crawling
{
    public class VkAccount : Account
    {
        public override void Login(ChromeDriver driver)
        {
            try { driver.Url = "https://vk.com/login"; }
            catch { Thread.Sleep(100); }
            driver.WaitForPageLayout();

            if (!driver.Url.Contains("/login"))
            {
                if (driver.Url.Contains(UserId))
                {
                    return;
                }
                foreach (var cookie in driver.Manage().Cookies.AllCookies)
                {
                    if (cookie.Domain == "vk.com" || cookie.Domain.EndsWith(".vk.com"))
                    {
                        driver.Manage().Cookies.DeleteCookie(cookie);
                    }
                }
                try { driver.Url = "https://vk.com/login"; }
                catch { Thread.Sleep(100); }
                driver.WaitForPageLayout();
            }

            // Sign In
            driver.TryUntilExec(() =>
            {
                driver.FindElement(By.CssSelector(".VkIdForm__signInButton")).Click();
            });

            driver.TryUntilExec(() =>
            {
                var user = driver.FindElement(By.CssSelector("input[name=login]"));
                user.Click();
                user.SendKeys(Email + "\n");
            });

            //TODO captcha

            driver.TryUntilExec(() =>
            {
                var pass = driver.FindElement(By.CssSelector("[type=password]"));
                pass.Click();
                pass.SendKeys(Password + "\n");
            });

            // Here might be 2FA

            driver.WaitForPageLayout();
        }
    }
}
