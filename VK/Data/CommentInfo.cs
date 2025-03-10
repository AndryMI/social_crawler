﻿using Core.Crawling;
using Core.Data;
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

        public string Author { get; set; }
        public string AuthorLink { get; set; }
        public string MentionUrl;
        public PostMediaInfo[] Media;
        public string Text;
        public string RawTime;

        public int Likes;

        public string Time => DateTimeParser.Convert(RawTime, CreatedAt);

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static CommentInfo[] Collect(Browser browser)
        {
            return browser.RunCollector<CommentInfo[]>("Scripts/VK/CommentInfo.js");
        }
    }
}
