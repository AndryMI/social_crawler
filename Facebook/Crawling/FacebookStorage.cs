﻿using Core.Crawling;
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
                tasks.Add(new PostCommentsTask(post.Link, post.Time, task));
            }
            if (task.IsSearch)
            {
                foreach (var post in posts)
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
    }
}
