using Core.Crawling;
using Core.Data;
using System;

namespace Facebook.Data
{
    public class PostInfo : IPostInfo
    {
        public string Social => "facebook";
        public string ProfileLink { get; set; }
        public string Link { get; set; }

        public string Time { get; set; }

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static PostInfo[] Collect(Browser browser)
        {
            browser.InjectUtils("Scripts/ReactUtils.js");
            return browser.RunCollector<PostInfo[]>("Scripts/Facebook/PostInfo.js");
        }
    }
}
