using Core.Crawling;
using Core.Data;
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

        public static CommentInfo[] Collect(Browser browser)
        {
            return browser.RunCollector<CommentInfo[]>("Scripts/Instagram/CommentInfo.js");
        }
    }
}
