using Core;
using Core.Crawling;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Twitter.Crawling
{
    public class TwitterAccount : Account
    {
        public override void Login(ChromeDriver driver)
        {
            if (!driver.Url.StartsWith("https://twitter.com"))
            {
                driver.Url = "https://twitter.com/login";
                driver.WaitForMain();
                driver.WaitForLoading();
            }

            if (driver.Url.Contains("/login"))
            {
                if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password))
                {
                    throw new AccountException("Email or Password are empty", this);
                }
                var user = driver.FindElement(By.CssSelector("[autocomplete=username]"));
                user.Click();
                user.SendKeys(Email + "\n");
                driver.WaitForLoading();

                var name = driver.TryFindElement(By.CssSelector("[data-testid=ocfEnterTextTextInput]"));
                if (name != null)
                {
                    name.Click();
                    name.SendKeys(UserId + "\n");
                    driver.WaitForLoading();
                }

                var pass = driver.FindElement(By.CssSelector("[type=password]"));
                pass.Click();
                pass.SendKeys(Password + "\n");
                driver.WaitForLoading();

                //TODO captcha
            }

            UserId = GetUserId(driver) ?? UserId;
        }

        private static string GetUserId(ChromeDriver driver)
        {
            var profile = driver.TryFindElement(By.CssSelector("[data-testid=AppTabBar_Profile_Link]"));
            if (profile != null)
            {
                var href = profile.GetAttribute("href");
                var id = new Uri(href).LocalPath.TrimStart('/');
                return string.IsNullOrEmpty(id) ? null : id;
            }
            return null;
        }
    }
}
