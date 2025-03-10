﻿using Core.Crawling;
using Core.Data;
using System;
using VK.Crawling;

namespace VK.Data
{
    public class PostInfo : IPostInfo
    {
        public string Social => "vkontakte";
        public string ProfileLink { get; set; }
        public string Link { get; set; }
        
        public string Text;
        public PostMediaInfo[] Media;
        public string QuoteLink;
        public string RawTime;

        public int Reactions;
        public int Shares;
        public int Views;

        public string Time => DateTimeParser.Convert(RawTime, CreatedAt);

        public DateTimeOffset CreatedAt = DateTimeOffset.UtcNow;

        public static PostInfo[] Collect(Browser browser)
        {
            return browser.RunCollector<PostInfo[]>("Scripts/VK/PostInfo.js");
        }
    }
}
