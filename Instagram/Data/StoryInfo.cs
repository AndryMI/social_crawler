using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;

namespace Instagram.Data
{
    public class StoryInfo
    {
        public string Link;

        public string ImageUrl;
        public string VideoUrl;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static StoryInfo Collect(ChromeDriver driver)
        {
            return driver.RunCollector<StoryInfo>("Scripts/Instagram/StoryInfo.js");
        }
    }
}
