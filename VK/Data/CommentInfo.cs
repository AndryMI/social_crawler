using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace VK.Data
{
    public class CommentInfo
    {
        public string Link;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static CommentInfo[] Collect(ChromeDriver driver)
        {
            var script = File.ReadAllText("Scripts/VK/CommentInfo.js");
            var json = driver.ExecuteScript(script) as string;
            return JsonConvert.DeserializeObject<CommentInfo[]>(json);
        }
    }
}
