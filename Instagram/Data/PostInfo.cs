using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

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
            var script = File.ReadAllText("Scripts/Instagram/PostInfo.js");
            var json = driver.ExecuteScript(script) as string;
            return JsonConvert.DeserializeObject<PostInfo>(json);
        }
    }
}
