using Core.Crawling;
using Core.Data;
using Core.Storages.Remote;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Core.Storages
{
    public class RemoteStorage : Threaded, IDataStorage, IMediaStorage
    {
        private readonly ConcurrentQueue<RequestChunk> chunks = new ConcurrentQueue<RequestChunk>();
        private readonly UniqueMediaFilter unique = new UniqueMediaFilter();

        public bool WaitForBrowserLoading => true;

        public void StoreProfile(CrawlerTask task, IProfileInfo data)
        {
            chunks.Enqueue(new RequestChunk(task, data));
        }

        public void StorePost(CrawlerTask task, IPostInfo data)
        {
            chunks.Enqueue(new RequestChunk(task, data));
        }

        public void StoreComment(CrawlerTask task, ICommentInfo data)
        {
            chunks.Enqueue(new RequestChunk(task, data));
        }

        public void StoreImage(ImageUrl image)
        {
            if (unique.NeedStore(image))
            {
                chunks.Enqueue(new RequestChunk(image));
            }
        }

        protected override void Run()
        {
            var client = new StorageApiClient();
            while (IsWorking)
            {
                try
                {
                    Log.Debug("Storage Size: {ChunksCount}", chunks.Count);
                    while (!client.IsReady && chunks.TryDequeue(out var chunk))
                    {
                        client.Add(chunk);
                    }
                    client.Send();
                    Thread.Sleep(TimeSpan.FromSeconds(Config.Instance.StorageApiSendInterval));
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Failed to store data");
                    Thread.Sleep(TimeSpan.FromSeconds(Config.Instance.RetryTimeout));
                }
            }
        }
    }
}