using Core.Storages;
using Instagram.Data;
using System;

namespace Instagram.Crawling
{
    public class InstagramStorage
    {
        private readonly IStorage storage;

        public InstagramStorage(IStorage storage)
        {
            this.storage = storage;
        }

        public void StoreProfile(ProfileInfo profile)
        {
            var uri = new Uri(profile.Link);
            storage.StoreData(uri, profile);
        }

        public void StorePost(PostInfo post)
        {
            var uri = new Uri(post.ProfileUrl);
            storage.StoreData(uri, post);
        }

        public void StoreComments(CommentInfo[] comments)
        {
            foreach (var comment in comments)
            {
                storage.StoreData(new Uri(comment.PostUrl), comment);
            }
        }

        public void StoreStory(StoryInfo story)
        {
            storage.StoreData(new Uri(story.Link), story);
        }
    }
}
