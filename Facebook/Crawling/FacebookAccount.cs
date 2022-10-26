﻿using Core;
using Core.Crawling;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Facebook.Crawling
{
    public class FacebookAccount : Account
    {
        public string Id;

        public override RequestLimits GetRequestLimits()
        {
            return new RequestLimits(200, TimeSpan.FromHours(1), url => url.Contains("/graphql/"));
        }

        private bool IsLoggedIn(ChromeDriver driver)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                var cookie = driver.Manage().Cookies.GetCookieNamed("c_user");
                return cookie != null && cookie.Value == Id;
            }
            driver.InjectUtils("Scripts/JsUtils.js");
            driver.InjectUtils("Scripts/Facebook/Utils.js");
            if (Name == driver.ExecuteScript("return __FindFacebookViewer()").ToString())
            {
                if (Id == null)
                {
                    Id = driver.Manage().Cookies.GetCookieNamed("c_user")?.Value;
                }
                return true;
            }
            return false;
        }

        public override void Login(ChromeDriver driver)
        {
            if (driver.Url.StartsWith("https://www.facebook.com") && IsLoggedIn(driver))
            {
                Crawler.Sleep(this, "open");
                return;
            }

            driver.Url = "https://www.facebook.com/login";
            driver.WaitForMain();

            if (!driver.Url.Contains("/login"))
            {
                if (IsLoggedIn(driver))
                {
                    Crawler.Sleep(this, "open");
                    return;
                }
                driver.DeleteCurrentCookies();
                driver.Url = "https://www.facebook.com/login";
                driver.WaitForMain();
            }
            Crawler.Sleep(this, "open");

            if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password))
            {
                throw new AccountException("Email or Password are empty", this);
            }
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
            Crawler.Sleep(this, "after credentials");

            driver.WaitForUrlChange();
            driver.WaitForMain();

            //TODO if banned or additional tests

            if (Id == null)
            {
                Id = driver.Manage().Cookies.GetCookieNamed("c_user")?.Value;
            }
            Crawler.Sleep(this, "after login");
        }
    }
}
