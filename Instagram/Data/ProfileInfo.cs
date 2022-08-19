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

        public bool IsBusinessAccount;
        public bool IsPrivate;
        public bool IsProfessionalAccount;
        public bool IsVerified;

        public ImageUrl PhotoImg;

        public int Following;
        public int Followers;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static ProfileInfo Collect(Browser browser)
        {
            browser.InjectUtils("Scripts/Instagram/Utils.js");
            return browser.RunCollector<ProfileInfo>("Scripts/Instagram/ProfileInfo.js");
        }
    }
}
