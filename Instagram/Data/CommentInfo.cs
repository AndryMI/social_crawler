using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace Instagram.Data
{
    public class CommentInfo
    {
        public string Link;
        public string PostUrl;

        public string Header;
        public string Body;
        public string Footer;
        public string Time;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static CommentInfo[] Collect(ChromeDriver driver)
        {
            var script = File.ReadAllText("Scripts/Instagram/CommentInfo.js");
            var json = driver.ExecuteScript(script) as string;
            return JsonConvert.DeserializeObject<CommentInfo[]>(json);
        }
    }
}
