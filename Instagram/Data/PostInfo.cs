using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;

namespace Instagram.Data
{
    public class PostInfo
    {
        public string Link;
        
        public string ProfileUrl;
        public string ImageUrl;
        public string VideoUrl;
        public string Time;

        public string Like;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static PostInfo Collect(ChromeDriver driver)
        {
            return driver.RunCollector<PostInfo>("Scripts/Instagram/PostInfo.js");
        }
    }
}
