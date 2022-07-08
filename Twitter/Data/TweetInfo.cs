using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;

namespace Twitter.Data
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
            return driver.RunCollector<TweetInfo[]>("Scripts/Twitter/TweetInfo.js");
        }
    }
}
