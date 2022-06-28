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

        public void StoreProfile(InstagramTask task, ProfileInfo profile)
        {
            var uri = new Uri(profile.Link);
            storage.StoreData(task, uri, profile);
        }

        public void StorePost(InstagramTask task, PostInfo post)
        {
            var uri = new Uri(post.ProfileUrl);
            storage.StoreData(task, uri, post);
        }

        public void StoreComments(InstagramTask task, CommentInfo[] comments)
        {
            foreach (var comment in comments)
            {
                storage.StoreData(task, new Uri(comment.PostUrl), comment);
            }
        }

        public void StoreStory(InstagramTask task, StoryInfo story)
        {
            storage.StoreData(task, new Uri(story.Link), story);
        }
    }
}
