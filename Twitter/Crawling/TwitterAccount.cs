using Core.Crawling;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Twitter.Crawling
{
    public class TwitterAccount : Account
    {
        public override string ToUid(string url)
        {
            var uri = new Uri(url);
            if (uri.LocalPath.StartsWith("/search"))
            {
                var query = HttpUtility.ParseQueryString(uri.Query)["q"];
                return uri.Host + "?" + query;
            }
            return uri.Host + Regex.Match(uri.LocalPath, "/[^/]*");
        }

        public override void Login(ChromeDriver driver)
        {
            driver.Url = "https://twitter.com/login";
            driver.WaitForMain();
            driver.WaitForLoading();

            var profile = driver.TryFindElement(By.CssSelector("[data-testid=AppTabBar_Profile_Link]"));
            if (profile != null)
            {
                var href = profile.GetAttribute("href");
                if (href == "https://twitter.com/" + Name)
                {
                    return;
                }
                driver.DeleteCurrentCookies();
                driver.Url = "https://twitter.com/login";
                driver.WaitForMain();
                driver.WaitForLoading();
            }

            var user = driver.FindElement(By.CssSelector("[autocomplete=username]"));
            user.Click();
            user.SendKeys(Email + "\n");
            driver.WaitForLoading();

            var name = driver.TryFindElement(By.CssSelector("[data-testid=ocfEnterTextTextInput]"));
            if (name != null)
            {
                name.Click();
                name.SendKeys(Name + "\n");
                driver.WaitForLoading();
            }

            var pass = driver.FindElement(By.CssSelector("[type=password]"));
            pass.Click();
            pass.SendKeys(Password + "\n");
            driver.WaitForLoading();

            //TODO captcha
        }
    }
}
