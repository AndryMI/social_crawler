using Core.Crawling;
using Core.Data;
using Core.Storages.Remote;
using MimeTypes;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Core.Storages
{
    public class RemoteStorage : Threaded, IDataStorage, IMediaStorage
    {
        private readonly ConcurrentQueue<RequestChunk> chunks = new ConcurrentQueue<RequestChunk>();

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
            image.Stored = DateTimeOffset.UtcNow.ToString(@"\/yyyy\/MM\/dd\/") + ObjectId.New() + MimeTypeMap.GetExtension(image.MimeType);
            chunks.Enqueue(new RequestChunk(image));
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
                while (!client.IsReady && chunks.TryDequeue(out var chunk))
                {
                    client.Add(chunk);
                }
                client.Send();
                Thread.Sleep(1000);
            }
        }
    }
}