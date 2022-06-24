using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

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
            var script = File.ReadAllText("Scripts/Instagram/ProfileInfo.js");
            var json = driver.ExecuteScript(script) as string;
            return JsonConvert.DeserializeObject<ProfileInfo>(json);
        }
    }
}
