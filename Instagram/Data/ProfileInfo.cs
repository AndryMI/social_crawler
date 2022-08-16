using Core.Crawling;
using Core.Data;
using Instagram.Crawling;
using System;

namespace Instagram.Data
{
    public class ProfileInfo : IProfileInfo
    {
        public string Social => "instagram";
        public string Link { get; set; }

        public string Id;
        public string Name;
        public string Description;
        public string Url;

        public ImageUrl PhotoImg;

        public string RawFollowing;
        public string RawFollowers;

        public int Following => InstagramUtils.ParseCount(RawFollowing);
        public int Followers => InstagramUtils.ParseCount(RawFollowers);

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static ProfileInfo Collect(Browser browser)
        {
            return browser.RunCollector<ProfileInfo>("Scripts/Instagram/ProfileInfo.js");
        }
    }
}
