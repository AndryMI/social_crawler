using Core.Crawling;
using Core.Storages;
using Instagram.Data;

namespace Instagram.Crawling
{
    public class InstagramStorage
    {
        private readonly IDataStorage storage;
        private readonly TaskManager tasks;

        public InstagramStorage(IDataStorage storage, TaskManager tasks)
        {
            this.storage = storage;
            this.tasks = tasks;
        }

        public void StoreProfile(InstagramTask task, ProfileInfo profile)
        {
            storage.StoreProfile(task, profile);
        }

        public void StorePosts(InstagramTask task, PostInfo[] posts)
        {
            if (task is PostProfileTask || task is PostCommentsTask)
            {
                foreach (var post in posts)
                {
                    storage.StorePost(task, post);
                }
                return;
            }
            foreach (var post in posts)
            {
                storage.StorePost(task, post);

                if (post.Comments > 0)
                {
                    tasks.Add(new PostCommentsTask(post.Link, post.Time, task));
                }
                if (task.IsExplore)
                {
                    tasks.Add(new PostProfileTask(post.ProfileLink, post.Time, task));
                }
            }
        }

        public void StoreComments(InstagramTask task, CommentInfo[] comments)
        {
            foreach (var comment in comments)
            {
                storage.StoreComment(task, comment);
            }
        }

        public void StoreStory(InstagramTask task, StoryInfo story)
        {
            storage.StorePost(task, story);
        }
    }
}
