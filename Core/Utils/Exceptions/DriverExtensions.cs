﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Core.Crawling
{
    public static class DriverExtensions
    {
        public static void InjectUtils(this ChromeDriver driver, string path)
        {
            var script = new StringBuilder()
                .Append("try {")
                .Append(" return (function __FN__() {")
                .Append(File.ReadAllText(path))
                .Append(" })()")
                .Append("}")
                .Append("catch (error) {")
                .Append(" return '!' + error.stack")
                .Append("}");

            var json = driver.ExecuteScript(script.ToString()) as string;
            if (json != null)
            {
                throw new JavaScriptException(json.Replace("__FN__", path));
            }
        }

        public static void WaitForReady(this ChromeDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Config.Instance.WaitTimeout));
            wait.Until(x => driver.ExecuteScript("return document.readyState").Equals("complete"));
            Thread.Sleep(100);
        }

        public static void WaitForUrlChange(this ChromeDriver driver, string url = null)
        {
            if (url == null)
            {
                url = driver.Url;
            }
            Thread.Sleep(100);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Config.Instance.WaitTimeout));
            wait.Until(x => x.Url != url);
        }

        public static void WaitForMain(this ChromeDriver driver)
        {
            Thread.Sleep(100);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Config.Instance.WaitTimeout));
            wait.Until(x => x.FindElements(By.TagName("main")).Count > 0);
        }

        public static bool WaitForMainShortly(this ChromeDriver driver, TimeSpan time)
        {
            try
            {
                Thread.Sleep(100);
                var wait = new WebDriverWait(driver, time);
                wait.Until(x => x.FindElements(By.TagName("main")).Count > 0);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void FocusToWindow(this ChromeDriver driver)
        {
            driver.SwitchTo().Window(driver.CurrentWindowHandle);
        }

        public static void TryUntilExec(this ChromeDriver driver, Action action)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromMinutes(Config.Instance.WaitTimeout));
            wait.Until(x =>
            {
                try
                {
                    action();
                    return true;
                }
                catch { return false; }
            });
        }

        public static IWebElement TryFindElement(this ChromeDriver driver, By by)
        {
            try
            {
                return driver.FindElement(by);
            }
            catch
            {
                return null;
            }
        }

        public static void DeleteCurrentCookies(this ChromeDriver driver)
        {
            driver.ExecuteScript(File.ReadAllText("Scripts/DeleteAllCookies.js"));
        }
    }
}
