using Core.Crawling;
using Core.Data;
using System;

namespace Facebook.Data
{
    public class CommentInfo : ICommentInfo
    {
        public string Social => "facebook";
        public string ProfileLink { get; set; }
        public string PostLink { get; set; }
        public string Link { get; set; }

        public string AuthorLink;
        public string Text;

        public int Reactions;

        public ImageUrl[] Images;
        public string[] Videos;

        public long UnixTime;

        public string Time => DateTimeOffset.FromUnixTimeSeconds(UnixTime).ToString("yyyy-MM-ddTHH:mm:ss.000Z");

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static CommentInfo[] Collect(Browser browser)
        {
            browser.InjectUtils("Scripts/JsUtils.js");
            browser.InjectUtils("Scripts/Facebook/Utils.js");
            return browser.RunCollector<CommentInfo[]>("Scripts/Facebook/CommentInfo.js");
        }
    }
}
