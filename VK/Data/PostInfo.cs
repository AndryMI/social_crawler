using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;

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
            return driver.RunCollector<PostInfo[]>("Scripts/VK/PostInfo.js");
        }
    }
}
