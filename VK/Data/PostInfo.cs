using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;
using VK.Crawling;

namespace VK.Data
{
    public class PostInfo
    {
        public string Link;
        
        public string Text;
        public PostMediaInfo[] Media;
        public string QuoteLink;
        public string Time;

        public int Reactions;
        public int Shares;
        public int Views;

        public string ParsedTime => DateTimeParser.Convert(Time, CreatedAt);

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static PostInfo[] Collect(ChromeDriver driver)
        {
            return driver.RunCollector<PostInfo[]>("Scripts/VK/PostInfo.js");
        }
    }
}
