using Core.Storages;
using VK.Data;
using System;
using Core.Crawling;

namespace VK.Crawling
{
    public class VkStorage
    {
        private readonly IStorage storage;
        private readonly TaskManager tasks;

        public VkStorage(IStorage storage, TaskManager tasks)
        {
            this.storage = storage;
            this.tasks = tasks;
        }

        public void StoreProfile(VkTask task, ProfileInfo profile)
        {
            var uri = new Uri(profile.Link);
            storage.StoreData(task, uri, profile);
        }

        public void StorePosts(VkTask task, PostInfo[] posts)
        {
            var uri = new Uri(task.Url);
            foreach (var post in posts)
            {
                storage.StoreData(task, uri, post);
            }

            foreach (var post in posts)
            {
                tasks.AddUrl(post.Link, post.ParsedTime);
            }
        }

        public void StoreComments(VkTask task, CommentInfo[] comments)
        {
            var uri = new Uri(task.Url);
            foreach (var comment in comments)
            {
                storage.StoreData(task, uri, comment);
            }
        }

        public void OnPrivatePage(VkTask task)
        {
            if (task.NeedAuthorization == false)
            {
                task.NeedAuthorization = true;
                tasks.Add(task);
            }
        }
    }
}
