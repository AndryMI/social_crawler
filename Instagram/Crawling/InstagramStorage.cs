using Core.Crawling;
using Core.Storages;
using Instagram.Data;

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
    }
}
