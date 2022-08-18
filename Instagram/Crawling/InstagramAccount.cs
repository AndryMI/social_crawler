using Core;
using Core.Crawling;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
                        Crawler.Sleep(this, "open");
                        return;
                    }
                }
                driver.DeleteCurrentCookies();
                driver.Url = "https://www.instagram.com/accounts/login/";
                driver.WaitForMain();
            }
            Crawler.Sleep(this, "open");

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
                throw new AccountException(this);
            }
            Crawler.Sleep(this, "after login");
        }
    }
}
