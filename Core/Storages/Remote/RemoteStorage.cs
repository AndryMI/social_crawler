using Core.Crawling;
using Core.Data;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Core.Storages
{
    public class RemoteStorage : IStorage
    {
        private readonly Thread thread;
        private volatile bool working = true;

        private readonly ConcurrentQueue<IProfileInfo> profiles = new ConcurrentQueue<IProfileInfo>();
        private readonly ConcurrentQueue<IPostInfo> posts = new ConcurrentQueue<IPostInfo>();
        private readonly ConcurrentQueue<ICommentInfo> comments = new ConcurrentQueue<ICommentInfo>();

        public RemoteStorage()
        {
            thread = new Thread(Run);
            thread.Start();
        }

        public void StoreProfile(CrawlerTask task, IProfileInfo data)
        {
            profiles.Enqueue(data);
        }

        public void StorePost(CrawlerTask task, IPostInfo data)
        {
            posts.Enqueue(data);
        }

        public void StoreComment(CrawlerTask task, ICommentInfo data)
        {
            comments.Enqueue(data);
        }

        public void StoreException(CrawlingException ex)
        {
            //TODO store exceptions
        }

        public void Stop(int timeout = -1)
        {
            working = false;
            thread.Join(timeout);
        }

        private void Run()
        {
            var client = new StorageApiClient();
            while (working)
            {
                client.StoreProfiles(Dequeue(profiles, 30));
                if (!working) break;
                Thread.Sleep(1000);

                client.StorePosts(Dequeue(posts, 30));
                if (!working) break;
                Thread.Sleep(1000);

                client.StoreComments(Dequeue(comments, 30));
                if (!working) break;
                Thread.Sleep(1000);
            }
        }

        private static List<T> Dequeue<T>(ConcurrentQueue<T> queue, int max)
        {
            var list = new List<T>();
            while (queue.TryDequeue(out var item) && list.Count < max)
            {
                list.Add(item);
            }
            return list;
        }
    }
}