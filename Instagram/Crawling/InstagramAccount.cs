using Core;
using Core.Crawling;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace Instagram.Crawling
{
    public class InstagramAccount : Account
    {
        public override void Login(ChromeDriver driver)
        {
            driver.Url = "https://www.instagram.com/accounts/login/";
            driver.WaitForMain();

            if (!driver.Url.Contains("/login/"))
            {
                foreach (var img in driver.FindElements(By.CssSelector("nav img")))
                {
                    if (img.GetAttribute("alt").Contains(Name))
                    {
                        return;
                    }
                }
                driver.DeleteCurrentCookies();
                driver.Url = "https://www.instagram.com/accounts/login/";
                driver.WaitForMain();
            }

            // Accept Cookies
            driver.FindElements(By.TagName("button")).LastOrDefault(button => button.Text.ToLower().Contains("cookie"))?.Click();

            new WebDriverWait(driver, TimeSpan.FromMinutes(Config.Instance.WaitTimeout)).Until(x =>
            {
                try
                {
                    var user = driver.FindElement(By.CssSelector("input[name=username]"));
                    user.Click();
                    user.SendKeys(Email);
                    return true;
                }
                catch { return false; }
            });

            new WebDriverWait(driver, TimeSpan.FromMinutes(Config.Instance.WaitTimeout)).Until(x =>
            {
                try
                {
                    var pass = driver.FindElement(By.CssSelector("[type=password]"));
                    pass.Click();
                    pass.SendKeys(Password + "\n");
                    return true;
                }
                catch { return false; }
            });

            driver.WaitForMain();
        }
    }
}
