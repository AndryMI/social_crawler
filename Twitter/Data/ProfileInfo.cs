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
        public string JoinDate => DateUtils.TryParseDate(RawJoinDate);

        public ImageUrl HeaderImg;
        public ImageUrl PhotoImg;

        public string RawFollowing;
        public string RawFollowers;
        public string RawJoinDate;

        public int Following => NumberUtils.ParseCount(RawFollowing);
        public int Followers => NumberUtils.ParseCount(RawFollowers);

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static ProfileInfo Collect(Browser browser)
        {
            return browser.RunCollector<ProfileInfo>("Scripts/Twitter/ProfileInfo.js");
        }
    }
}
