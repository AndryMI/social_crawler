using Core.Storages;
using Instagram.Data;

namespace Instagram.Crawling
{
    public class InstagramStorage
    {
        private readonly IDataStorage storage;

        public InstagramStorage(IDataStorage storage)
        {
            this.storage = storage;
        }

        public void StoreProfile(InstagramTask task, ProfileInfo profile)
        {
            storage.StoreProfile(task, profile);
        }

        public void StorePost(InstagramTask task, PostInfo post)
        {
            storage.StorePost(task, post);
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
