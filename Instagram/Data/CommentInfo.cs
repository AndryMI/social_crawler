using Core.Crawling;
using Core.Data;
using System;

namespace Instagram.Data
{
    public class CommentInfo : ICommentInfo
    {
        public string Social => "instagram";
        public string ProfileLink { get; set; }
        public string PostLink { get; set; }
        public string Link { get; set; }

        public string Author;
        public string Text;

        public int Like;

        public long UnixTime;

        public string Time => DateTimeOffset.FromUnixTimeSeconds(UnixTime).ToString("yyyy-MM-ddTHH:mm:ss.000Z");

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static CommentInfo[] Collect(Browser browser)
        {
            browser.InjectUtils("Scripts/Instagram/Utils.js");
            return browser.RunCollector<CommentInfo[]>("Scripts/Instagram/CommentInfo.js");
        }
    }
}
