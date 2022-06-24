using Core.Crawling;
using OpenQA.Selenium.Chrome;
using System;

namespace Core
{
    public class CrawlingException : Exception
    {
        public CrawlerTask Task { get; private set; }
        public string Html { get; private set; }

        public CrawlingException(Exception inner, CrawlerTask task, ChromeDriver driver) : base(inner.Message, inner)
        {
            Task = task;
            Html = (string)(driver?.ExecuteScript("return document.body.outerHTML") ?? "");
        }
    }
}
