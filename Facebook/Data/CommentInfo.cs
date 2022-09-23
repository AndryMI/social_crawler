using Core.Crawling;
using Core.Data;
using System;

namespace Facebook.Data
{
    public class CommentInfo : ICommentInfo
    {
        public string Social => "facebook";
        public string ProfileLink { get; set; }
        public string PostLink { get; set; }
        public string Link { get; set; }

        public string Time { get; set; }

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static CommentInfo[] Collect(Browser browser)
        {
            browser.InjectUtils("Scripts/ReactUtils.js");
            return browser.RunCollector<CommentInfo[]>("Scripts/Facebook/CommentInfo.js");
        }
    }
}
