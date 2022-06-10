using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;

namespace RD_Team_TweetMonitor
{
    public class CrawlingException : Exception
    {
        public CrawlerTask Task { get; private set; }
        public string[] Html { get; private set; }

        public CrawlingException(Exception inner, CrawlerTask task, ChromeDriver driver) : base(inner.Message, inner)
        {
            Task = task;
            Html = driver.FindElements(By.TagName("article")).Select(x => x.GetAttribute("outerHTML")).ToArray();
        }
    }
}
