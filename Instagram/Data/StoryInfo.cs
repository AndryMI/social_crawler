using Core.Data;
using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;

namespace Instagram.Data
{
    public class StoryInfo : IPostInfo
    {
        public string Social => "instagram";
        public string ProfileLink { get; set; }
        public string Link { get; set; }

        public string ImageUrl;
        public string VideoUrl;

        public string Time { get; set; }

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static StoryInfo Collect(ChromeDriver driver)
        {
            return driver.RunCollector<StoryInfo>("Scripts/Instagram/StoryInfo.js");
        }
    }
}
