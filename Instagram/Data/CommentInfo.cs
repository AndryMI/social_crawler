using Core.Data;
using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;

namespace Instagram.Data
{
    public class CommentInfo : ICommentInfo
    {
        public string Social => "instagram";
        public string ProfileLink { get; set; }
        public string PostLink { get; set; }
        public string Link { get; set; }

        public string Header;
        public string Body;
        public string Footer;
        public string Time { get; set; }

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static CommentInfo[] Collect(ChromeDriver driver)
        {
            return driver.RunCollector<CommentInfo[]>("Scripts/Instagram/CommentInfo.js");
        }
    }
}
