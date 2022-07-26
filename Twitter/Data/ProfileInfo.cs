using Core.Crawling;
using Core.Data;
using System;

namespace Twitter.Data
{
    public class ProfileInfo : IProfileInfo
    {
        public string Social => "twitter";
        public string Link { get; set; }

        public string Id;
        public string Name;
        public string Description;
        public string Location;
        public string Url;
        public string JoinDate;

        public ImageUrl HeaderImg;
        public ImageUrl PhotoImg;

        public string RawFollowing;
        public string RawFollowers;

        public int Following => TwitterUtils.ParseFollow(RawFollowing);
        public int Followers => TwitterUtils.ParseFollow(RawFollowers);

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static ProfileInfo Collect(Browser browser)
        {
            return browser.RunCollector<ProfileInfo>("Scripts/Twitter/ProfileInfo.js");
        }
    }
}
