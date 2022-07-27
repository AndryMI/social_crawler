using Core.Crawling;
using Core.Data;
using System;

namespace Instagram.Data
{
    public class PostInfo : IPostInfo
    {
        public string Social => "instagram";
        public string ProfileLink { get; set; }
        public string Link { get; set; }
        
        public string ImageUrl;
        public string VideoUrl;
        public string Time { get; set; }

        public string Like;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static PostInfo Collect(Browser browser)
        {
            return browser.RunCollector<PostInfo>("Scripts/Instagram/PostInfo.js");
        }
    }
}
