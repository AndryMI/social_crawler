using Core;
using Core.Crawling;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;

namespace Instagram.Crawling
{
    public class InstagramAccount : Account
    {
        public override RequestLimits GetRequestLimits()
        {
            return new RequestLimits(100, TimeSpan.FromHours(1), url => url.Contains("/api/"));
        }

        private bool IsLoggedIn(ChromeDriver driver)
        {
            var script =
                "return __WalkFiberRecursive(__GetFiber(document.querySelector('body>div>div')), (fb) => {" +
                "  return fb.pendingProps?.viewer?.username" +
                "})";
            driver.InjectUtils("Scripts/ReactUtils.js");
            return driver.ExecuteScript(script)?.ToString() == Name;
        }

        public override void Login(ChromeDriver driver)
        {
            if (driver.Url.StartsWith("https://www.instagram.com") && IsLoggedIn(driver))
            {
                Crawler.Sleep(this, "open");
                return;
            }

            driver.Url = "https://www.instagram.com/accounts/login/";
            driver.WaitForMain();

            if (!driver.Url.Contains("/login/"))
            {
                if (IsLoggedIn(driver))
                {
                    Crawler.Sleep(this, "open");
                    return;
                }
                driver.DeleteCurrentCookies();
                driver.Url = "https://www.instagram.com/accounts/login/";
                driver.WaitForMain();
            }
            Crawler.Sleep(this, "open");

            if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password))
            {
                throw new AccountException("Email or Password are empty", this);
            }
            // Accept Cookies
            driver.FindElements(By.TagName("button")).LastOrDefault(button => button.Text.ToLower().Contains("cookie"))?.Click();
            Crawler.Sleep(this, "accept cookies");

            driver.TryUntilExec(() =>
            {
                var user = driver.FindElement(By.CssSelector("input[name=username]"));
                user.Click();
                user.SendKeys(Email);
            });

            driver.TryUntilExec(() =>
            {
                var pass = driver.FindElement(By.CssSelector("[type=password]"));
                pass.Click();
                pass.SendKeys(Password + "\n");
            });
            Crawler.Sleep(this, "after credentials");

            driver.WaitForUrlChange();
            driver.WaitForMain();

            var message = driver.TryFindElement(By.CssSelector("main h3"))?.Text ?? "";
            if (message.ToLower().Contains("add phone number"))
            {
                throw new AccountException("Account is blocked", this);
            }
            Crawler.Sleep(this, "after login");
        }
    }
}
