using Core.Crawling;
using Core.Data;
using System;

namespace Facebook.Data
{
    public class PostInfo : IPostInfo
    {
        public string Social => "facebook";
        public string ProfileLink { get; set; }
        public string Link { get; set; }

        public string Text;
        public ImageUrl[] Images;
        public string[] Videos;
        public string[] Links;

        public int Reactions;
        public int Comments;
        public int Shares;

        public long UnixTime;

        public string Time => DateTimeOffset.FromUnixTimeSeconds(UnixTime).ToString("yyyy-MM-ddTHH:mm:ss.000Z");

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static PostInfo[] Collect(Browser browser)
        {
            browser.InjectUtils("Scripts/JsUtils.js");
            browser.InjectUtils("Scripts/Facebook/Utils.js");
            return browser.RunCollector<PostInfo[]>("Scripts/Facebook/PostInfo.js");
        }
    }
}
