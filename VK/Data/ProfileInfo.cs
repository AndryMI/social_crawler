using Core.Data;
using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;

namespace VK.Data
{
    public enum ProfileType
    {
        Undefined,
        Profile,
        Public,
        Group,
    }

    public class ProfileInfo : IProfileInfo
    {
        public string Social => "vkontakte";
        public string Link { get; set; }

        public ProfileType Type;
        public string Name;
        public string Status;
        public List<KeyValuePair<string, string>> Description;
        public string Url;

        public string PhotoImg;
        public string HeaderImg;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static ProfileInfo Collect(ChromeDriver driver)
        {
            return driver.RunCollector<ProfileInfo>("Scripts/VK/ProfileInfo.js");
        }
    }
}
