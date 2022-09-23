using Core.Crawling;
using Core.Data;
using System;

namespace Facebook.Data
{
    public class ProfileInfo : IProfileInfo
    {
        public string Social => "facebook";
        public string Link { get; set; }

        public string Id;
        public string Name;
        public string Description;
        public string Url;

        public ImageUrl HeaderImg;
        public ImageUrl PhotoImg;

        public int Following;
        public int Followers;

        public string Location;
        public string JoinDate;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static ProfileInfo Collect(Browser browser)
        {
            browser.InjectUtils("Scripts/ReactUtils.js");
            return browser.RunCollector<ProfileInfo>("Scripts/Facebook/ProfileInfo.js");
        }
    }
}
