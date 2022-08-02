using Core.Storages;
using VK.Data;
using Core.Crawling;

namespace VK.Crawling
{
    public class VkStorage
    {
        private readonly IDataStorage storage;
        private readonly TaskManager tasks;

        public VkStorage(IDataStorage storage, TaskManager tasks)
        {
            this.storage = storage;
            this.tasks = tasks;
        }

        public void StoreProfile(VkTask task, ProfileInfo profile)
        {
            storage.StoreProfile(task, profile);
        }

        public void StorePosts(VkTask task, PostInfo[] posts)
        {
            foreach (var post in posts)
            {
                storage.StorePost(task, post);
            }

            foreach (var post in posts)
            {
                tasks.Add(new VkTask(post.Link, post.Time, task));
            }
        }

        public void StoreComments(VkTask task, CommentInfo[] comments)
        {
            foreach (var comment in comments)
            {
                storage.StoreComment(task, comment);
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
