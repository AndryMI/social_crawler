using Core.Crawling;
using Core.Data;
using Facebook.Crawling;
using System;
using System.Collections.Generic;

namespace Facebook.Data
{
    public class ProfileInfo : IProfileInfo
    {
        public string Social => "facebook";
        public string Link { get; set; }

        public string Type;
        public string FacebookId;
        public string Id;
        public string Name;
        public string Gender;
        public string Description;
        public string Url => Info.TryGetInfo("INTRO_CARD_WEBSITE", "website");
        public string JoinDate => Info.TryGetInfo("INTRO_CARD_MEMBER_SINCE");
        public string Location => Info.TryGetInfo("INTRO_CARD_CURRENT_CITY", "INTRO_CARD_HOMETOWN");

        public ImageUrl HeaderImg;
        public ImageUrl PhotoImg;

        public int Like => FacebookUtils.ParseCount(RawLike); //TODO try find in Info
        public int Followers => FacebookUtils.ParseCount(RawFollowers); //TODO try find in Info eg INTRO_CARD_FOLLOWERS

        public string RawLike;
        public string RawFollowers;

        public bool IsVerified;
        public bool IsVisiblyMemorialized;
        public bool IsAdditionalProfilePlus;

        public List<KeyValuePair<string, string>> Info;
        
        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static ProfileInfo Collect(Browser browser)
        {
            browser.InjectUtils("Scripts/JsUtils.js");
            return browser.RunCollector<ProfileInfo>("Scripts/Facebook/ProfileInfo.js");
        }
    }
}
