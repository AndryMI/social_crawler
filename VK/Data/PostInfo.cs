using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace VK.Data
{
    public class PostInfo
    {
        public string Link;
        
        public string Text;
        public PostMediaInfo[] Media;
        public string QuoteLink;
        public string Time;

        public int Reactions;
        public int Shares;
        public int Views;

        public string ParsedTime
        {
            get
            {
                if (DateTimeOffset.TryParse(Time.Replace("at", CreatedAt.Year.ToString()), out var result))
                {
                    return result.ToString("yyyy-MM-ddTHH:mm:ss.000Z");
                }
                return null;
            }
        }

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static PostInfo[] Collect(ChromeDriver driver)
        {
            var script = File.ReadAllText("Scripts/VK/PostInfo.js");
            var json = driver.ExecuteScript(script) as string;
            return JsonConvert.DeserializeObject<PostInfo[]>(json);
        }
    }
}
