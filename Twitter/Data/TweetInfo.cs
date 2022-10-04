using Core.Crawling;
using Core.Data;
using Newtonsoft.Json;
using System;

namespace Twitter.Data
{
    public class TweetInfo : IPostInfo, ICommentInfo
    {
        public string Social => "twitter";

        public string ProfileLink {
            get => profileLink ?? TwitterUtils.ExtractProfileLink(PostLink ?? Link);
            set => profileLink = value;
        }
        private string profileLink;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PostLink { get; set; }
        public string Link { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Author => PostLink != null ? TwitterUtils.ExtractProfileName(Link) : null;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AuthorLink => PostLink != null ? TwitterUtils.ExtractProfileLink(Link) : null;

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
    }
}
