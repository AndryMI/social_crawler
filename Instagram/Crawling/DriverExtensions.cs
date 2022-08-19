﻿using Core;
using Core.Crawling;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace Instagram.Crawling
{
    public static class DriverExtensions
    {
        public static void WaitForStoryLoading(this ChromeDriver driver)
        {
            Thread.Sleep(100);
            while (driver.FindElements(By.CssSelector("[data-visualcompletion=\"loading-state\"]:not([style])")).Count > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Config.Instance.WaitTimeout));
                wait.Until(x => x.FindElements(By.CssSelector("[data-visualcompletion=\"loading-state\"]:not([style])")).Count == 0);
                Thread.Sleep(100);
            }
        }

        public static void WaitForCommentsLoading(this ChromeDriver driver)
        {
            driver.WaitForLoading(
                "return !__WalkFiberRecursive(__GetFiber(document.querySelector('article')), cf => {" +
                "  if (cf.pendingProps.commentsIsFetching) return true" +
                "})"
            );
        }

        public static void WaitForPostsLoading(this ChromeDriver driver)
        {
            driver.WaitForLoading("return !__FindProps(document.querySelector('article'), p => 'isFetching' in p).isFetching");
        }

        private static void WaitForLoading(this ChromeDriver driver, string script)
        {
            driver.InjectUtils("Scripts/Instagram/Utils.js");
            Thread.Sleep(100);
            while (!(bool)driver.ExecuteScript(script))
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Config.Instance.WaitTimeout));
                wait.Until(x => (bool)driver.ExecuteScript(script));
                Thread.Sleep(100);
            }
        }

        public static void ScrollToPageBottom(this ChromeDriver driver)
        {
            driver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
        }

        public static void LoadMoreComments(this ChromeDriver driver)
        {
            driver.InjectUtils("Scripts/Instagram/Utils.js");
            driver.ExecuteScript(
                "var cf = __WalkFiberRecursive(__GetFiber(document.querySelector('article')), cf => {" +
                "  if (cf.pendingProps?.handleLoadMoreCommentsClick) return cf" +
                "});" +
                "if (cf) cf.pendingProps.handleLoadMoreCommentsClick()"
            );
        }

        public static void TryOpenStories(this ChromeDriver driver)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
                wait.Until(x =>
                {
                    driver.ExecuteScript("document.querySelector('header img')?.click()");
                    Thread.Sleep(100);
                    return driver.Url.Contains("/stories/");
                });
            }
            catch (WebDriverTimeoutException) { }
        }
    }
}
