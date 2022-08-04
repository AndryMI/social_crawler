using Core.Crawling;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Core
{
    public class CrawlingException : Exception
    {
        public CrawlerTask Task { get; private set; }
        public string Html { get; private set; }
        public Screenshot Screenshot { get; private set; }

        public CrawlingException(Exception inner, CrawlerTask task, ChromeDriver driver) : base(inner.Message, inner)
        {
            Task = task;
            try { Html = (string)(driver?.ExecuteScript("return document.body.outerHTML") ?? ""); } catch { }
            try { Screenshot = driver?.GetScreenshot(); } catch { }
        }
    }
}
