﻿using Newtonsoft.Json;
using System.IO;

namespace RD_Team_TweetMonitor
{
    public class FileStorage
    {
        public void StoreProfile(ProfileInfo profile)
        {
            var path = profile.Link.Substring("https://twitter.com".Length);
            Directory.CreateDirectory("Data" + path);
            File.WriteAllText("Data" + path + ".json", JsonConvert.SerializeObject(profile));
        }

        public void StoreTweets(string url, TweetInfo[] tweets)
        {
            foreach (var tweet in tweets)
            {
                var path = tweet.Link.Substring("https://twitter.com".Length);
                Directory.CreateDirectory("Data" + path);
                File.WriteAllText("Data" + path + ".json", JsonConvert.SerializeObject(tweet));
            }
        }

        public void StoreReplies(string url, TweetInfo[] tweets)
        {
            foreach (var tweet in tweets)
            {
                var path = tweet.Link.Substring("https://twitter.com".Length);
                Directory.CreateDirectory("Data" + path);
                File.WriteAllText("Data" + path + ".json", JsonConvert.SerializeObject(tweet));
            }
        }
    }
}
