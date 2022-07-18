using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;
using VK.Crawling;

namespace VK.Data
{
    public class CommentInfo
    {
        public string Link;

        public string Author;
        public string AuthorUrl;
        public string MentionUrl;
        public PostMediaInfo[] Media;
        public string Text;
        public string Time;

        public int Likes;

        public string ParsedTime => DateTimeParser.Convert(Time, CreatedAt);

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static CommentInfo[] Collect(ChromeDriver driver)
        {
            return driver.RunCollector<CommentInfo[]>("Scripts/VK/CommentInfo.js");
        }
    }
}
