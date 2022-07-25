using Core.Crawling;
using Core.Data;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace Twitter.Data
{
    public class TweetInfo : IPostInfo, ICommentInfo
    {
        public string Social => "twitter";

        public string ProfileLink {
            get => profileLink ?? ExtractProfileLink(PostLink ?? Link);
            set => profileLink = value;
        }
        private string profileLink;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PostLink { get; set; }
        public string Link { get; set; }

        public string Text;
        public string Time { get; set; }

        public int Reply;
        public int Retweet;
        public int Like;

        public TweetAttachInfo Attach;

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static TweetInfo[] Collect(Browser browser)
        {
            return browser.RunCollector<TweetInfo[]>("Scripts/Twitter/TweetInfo.js");
        }

        private static string ExtractProfileLink(string link)
        {
            return Regex.Replace(link, "/status/.*", "");
        }
    }
}
