using Core;
using Core.Crawling;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Facebook.Crawling
{
    public class FacebookAccount : Account
    {
        public override RequestLimits GetRequestLimits()
        {
            return new RequestLimits(100, TimeSpan.FromHours(1), url => url.Contains("facebook.com"));
        }

        public override void Login(ChromeDriver driver)
        {
            if (!driver.Url.StartsWith("https://www.facebook.com"))
            {
                driver.Url = "https://www.facebook.com/login";
                driver.WaitForMain();
                Crawler.Sleep(this, "open");
            }

            if (new Uri(driver.Url).LocalPath.StartsWith("/login"))
            {
                if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password))
                {
                    throw new AccountException("Email or Password are empty", this);
                }
                Crawler.Sleep(this, "before login");

                driver.TryUntilExec(() =>
                {
                    var user = driver.FindElement(By.CssSelector("input[name=email]"));
                    user.Click();
                    user.SendKeys(Email);
                });

                driver.TryUntilExec(() =>
                {
                    var pass = driver.FindElement(By.CssSelector("[type=password]"));
                    pass.Click();
                    pass.SendKeys(Password + "\n");
                });

                driver.WaitForUrlChange();
                driver.WaitForMain();

                Crawler.Sleep(this, "after login");
            }

            UserId = driver.Manage().Cookies.GetCookieNamed("c_user")?.Value ?? UserId;

            if (new Uri(driver.Url).LocalPath.StartsWith("/checkpoint/"))
            {
                throw new AccountException("Failed to login", this);
            }

            Crawler.Sleep(this, "open");
        }
    }
}
