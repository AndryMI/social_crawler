using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;

namespace Instagram.Data
{
    public class CommentInfo
    {
        public string Link;
        public string PostUrl;

        public string Header;
        public string Body;
        public string Footer;
        public string Time;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static CommentInfo[] Collect(ChromeDriver driver)
        {
            return driver.RunCollector<CommentInfo[]>("Scripts/Instagram/CommentInfo.js");
        }
    }
}
