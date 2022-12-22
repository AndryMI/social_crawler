using Core.Crawling;
using Core.Storages;
using Facebook.Data;

namespace Facebook.Crawling
{
    public class FacebookStorage
    {
        private readonly IDataStorage storage;
        private readonly TaskManager tasks;

        public FacebookStorage(IDataStorage storage, TaskManager tasks)
        {
            this.storage = storage;
            this.tasks = tasks;
        }

        public void StoreProfile(FacebookTask task, ProfileInfo profile)
        {
            storage.StoreProfile(task, profile);
            tasks.Add(new RelationsTask(RelationsTask.ToUrl(task.Url), task.Priority, task));
        }

        public void StorePosts(FacebookTask task, PostInfo[] posts)
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
                if (task.IsSearch)
                {
                    tasks.Add(new PostProfileTask(post.ProfileLink, post.Time, task));
                }
            }
        }

        public void StoreComments(FacebookTask task, CommentInfo[] comments)
        {
            foreach (var comment in comments)
            {
                storage.StoreComment(task, comment);
            }
        }

        public void StoreRelations(FacebookTask task, RelationInfo[] relations)
        {
            foreach (var relation in relations)
            {
                storage.StoreRelation(task, relation);
            }
        }
    }
}
