using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;

namespace RD_Team_TweetMonitor
{
    public class CrawlingException : Exception
    {
        public string Url { get; private set; }
        public string[] Html { get; private set; }

        public CrawlingException(Exception inner, string url, ChromeDriver driver) : base(inner.Message, inner)
        {
            Url = url;
            Html = driver.FindElements(By.TagName("article")).Select(x => x.GetAttribute("outerHTML")).ToArray();
        }
    }
}
