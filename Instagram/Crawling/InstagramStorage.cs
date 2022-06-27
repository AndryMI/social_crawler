using Core.Crawling;
using Core.Storages;
using Instagram.Data;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Instagram.Crawling
{
    public class InstagramStorage
    {
        private readonly IStorage storage;
        private readonly TaskManager tasks;

        public InstagramStorage(IStorage storage, TaskManager tasks)
        {
            this.storage = storage;
            this.tasks = tasks;
        }

        public void StoreProfile(ProfileInfo profile)
        {
        }

        public void StorePost(PostInfo post)
        {
            Debug.WriteLine(JsonConvert.SerializeObject(post));
        }

        public void StoreComments(CommentInfo[] comments)
        {
            Debug.WriteLine("--- --- ---");
            foreach (var comment in comments)
            {
                Debug.WriteLine(JsonConvert.SerializeObject(comment));
            }
        }
    }
}
