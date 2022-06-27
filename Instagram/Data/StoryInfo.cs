using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

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
            var script = File.ReadAllText("Scripts/Instagram/StoryInfo.js");
            var json = driver.ExecuteScript(script) as string;
            return JsonConvert.DeserializeObject<StoryInfo>(json);
        }
    }
}
