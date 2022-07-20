using Core.Data;
using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;

namespace Instagram.Data
{
    public class PostInfo : IPostInfo
    {
        public string Social => "instagram";
        public string ProfileLink { get; set; }
        public string Link { get; set; }
        
        public string ImageUrl;
        public string VideoUrl;
        public string Time { get; set; }

        public string Like;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static PostInfo Collect(ChromeDriver driver)
        {
            return driver.RunCollector<PostInfo>("Scripts/Instagram/PostInfo.js");
        }
    }
}
