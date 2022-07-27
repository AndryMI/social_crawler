using Core.Crawling;
using Core.Data;
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

        public string PhotoImg;

        public string Following;
        public string Followers;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static ProfileInfo Collect(Browser browser)
        {
            return browser.RunCollector<ProfileInfo>("Scripts/Instagram/ProfileInfo.js");
        }
    }
}
