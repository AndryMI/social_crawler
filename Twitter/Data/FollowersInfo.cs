using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;

namespace Twitter.Data
{
    public class FollowersInfo
    {
        public string Link;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static FollowersInfo[] Collect(ChromeDriver driver)
        {
            return driver.RunCollector<FollowersInfo[]>("Scripts/Twitter/FollowersInfo.js");
        }
    }
}
