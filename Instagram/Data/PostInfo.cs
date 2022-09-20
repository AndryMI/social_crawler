using Core.Crawling;
using Core.Data;
using System;

namespace Instagram.Data
{
    public class PostInfo : IPostInfo
    {
        public string Social => "instagram";
        public string ProfileLink { get; set; }
        public string Link { get; set; }

        public string Text;
        public ImageUrl[] Images;
        public string[] Videos;

        public int Comments;
        public int Like;

        public long UnixTime;

        public string Time => DateTimeOffset.FromUnixTimeSeconds(UnixTime).ToString("yyyy-MM-ddTHH:mm:ss.000Z");

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static PostInfo[] Collect(Browser browser)
        {
            browser.InjectUtils("Scripts/ReactUtils.js");
            return browser.RunCollector<PostInfo[]>("Scripts/Instagram/PostInfo.js");
        }
    }
}
