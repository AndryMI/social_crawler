using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace RD_Team_TweetMonitor
{
    public class TweetInfo
    {
        public string Link;

        public string Text;
        public string Time;

        public int Reply;
        public int Retweet;
        public int Like;

        public TweetAttachInfo Attach;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static TweetInfo[] Collect(ChromeDriver driver)
        {
            var script = File.ReadAllText("Scripts/TweetInfo.js");
            var json = driver.ExecuteScript(script) as string;
            return JsonConvert.DeserializeObject<TweetInfo[]>(json);
        }
    }
}
