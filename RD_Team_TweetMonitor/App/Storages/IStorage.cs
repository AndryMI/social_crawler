﻿
namespace RD_Team_TweetMonitor
{
    public interface IStorage
    {
        void StoreProfile(ProfileInfo profile);
        void StoreTweets(string url, TweetInfo[] tweets);
        void StoreFollowers(string url, FollowersInfo[] followers);
        void StoreException(CrawlingException exception);
    }
}
