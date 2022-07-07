using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;

namespace VK.Data
{
    public class ProfileInfo
    {
        public string Link;

        public string Name;
        public string Status;
        public List<KeyValuePair<string, string>> Description;
        public string Url;

        public string PhotoImg;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static ProfileInfo Collect(ChromeDriver driver)
        {
            var script = File.ReadAllText("Scripts/VK/ProfileInfo.js");
            var json = driver.ExecuteScript(script) as string;
            return JsonConvert.DeserializeObject<ProfileInfo>(json);
        }
    }
}
