using Core.Crawling;
using Core.Data;
using System;

namespace Twitter.Data
{
    public class FollowersInfo
    {
        public string Link;

        public string Name;
        public string Description;
        public ImageUrl PhotoImg;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static FollowersInfo[] Collect(Browser browser)
        {
            return browser.RunCollector<FollowersInfo[]>("Scripts/Twitter/FollowersInfo.js");
        }
    }
}
