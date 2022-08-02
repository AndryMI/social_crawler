using Core.Crawling;
using Core.Data;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Core.Storages
{
    public class RemoteStorage : Threaded, IStorage
    {
        private readonly Queue<IProfileInfo> profiles = new Queue<IProfileInfo>();
        private readonly Queue<IPostInfo> posts = new Queue<IPostInfo>();
        private readonly Queue<ICommentInfo> comments = new Queue<ICommentInfo>();

        public void StoreProfile(CrawlerTask task, IProfileInfo data)
        {
            profiles.Enqueue(task, data);
        }

        public void StorePost(CrawlerTask task, IPostInfo data)
        {
            posts.Enqueue(task, data);
        }

        public void StoreComment(CrawlerTask task, ICommentInfo data)
        {
            comments.Enqueue(task, data);
        }

        public void StoreException(CrawlingException ex)
        {
            //TODO store exceptions
        }

        protected override void Run()
        {
            var client = new StorageApiClient();
            while (IsWorking)
            {
                client.StoreProfiles(profiles.Dequeue(30));
                if (!IsWorking) break;
                Thread.Sleep(1000);

                client.StorePosts(posts.Dequeue(30));
                if (!IsWorking) break;
                Thread.Sleep(1000);

                client.StoreComments(comments.Dequeue(30));
                if (!IsWorking) break;
                Thread.Sleep(1000);
            }
        }

        private class Queue<T> : ConcurrentQueue<StorageApiClient.Args<T>>
        {
            public void Enqueue(CrawlerTask task, T data)
            {
                Enqueue(new StorageApiClient.Args<T> { task = task, data = data });
            }

            public List<StorageApiClient.Args<T>> Dequeue(int max)
            {
                var list = new List<StorageApiClient.Args<T>>();
                while (TryDequeue(out var item) && list.Count < max)
                {
                    list.Add(item);
                }
                return list;
            }
        }
    }
}