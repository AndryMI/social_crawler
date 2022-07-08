using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;

namespace Instagram.Data
{
    public class ProfileInfo
    {
        public string Link;

        public string Id;
        public string Name;
        public string Description;
        public string Url;

        public string PhotoImg;

        public string Following;
        public string Followers;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static ProfileInfo Collect(ChromeDriver driver)
        {
            return driver.RunCollector<ProfileInfo>("Scripts/Instagram/ProfileInfo.js");
        }
    }
}
