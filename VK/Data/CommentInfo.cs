using Core.Data;
using Core.Utils;
using OpenQA.Selenium.Chrome;
using System;
using VK.Crawling;

namespace VK.Data
{
    public class CommentInfo : ICommentInfo
    {
        public string Social => "vkontakte";
        public string ProfileLink { get; set; }
        public string PostLink { get; set; }
        public string Link { get; set; }

        public string Author;
        public string AuthorUrl;
        public string MentionUrl;
        public PostMediaInfo[] Media;
        public string Text;
        public string RawTime;

        public int Likes;

        public string Time => DateTimeParser.Convert(RawTime, CreatedAt);

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static CommentInfo[] Collect(ChromeDriver driver)
        {
            return driver.RunCollector<CommentInfo[]>("Scripts/VK/CommentInfo.js");
        }
    }
}
