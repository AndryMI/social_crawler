using Core.Crawling;
using Core.Data;
using System;

namespace Instagram.Data
{
    public class StoryInfo : IPostInfo
    {
        public string Social => "instagram";
        public string ProfileLink { get; set; }
        public string Link { get; set; }

        public ImageUrl StoryImg;
        public string VideoUrl;

        public string Time { get; set; }

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static StoryInfo Collect(Browser browser)
        {
            return browser.RunCollector<StoryInfo>("Scripts/Instagram/StoryInfo.js");
        }
    }
}
