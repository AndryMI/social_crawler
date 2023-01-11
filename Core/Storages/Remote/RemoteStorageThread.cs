using Serilog;
using System;
using System.Threading;

namespace Core.Storages
{
    public class RemoteStorageThread : Threaded
    {
        private readonly MultipartStorage storage;
        private readonly IErrorStorage errors;

        public RemoteStorageThread(MultipartStorage storage, IErrorStorage errors)
        {
            this.storage = storage;
            this.errors = errors;
        }

        protected override void Run()
        {
            Thread.Sleep(0);
            var data = default(MultipartData);
            var client = new ApiServerClient();
            while (IsWorking)
            {
                try
                {
                    if (storage.TryDequeue(out data))
                    {
                        client.Request("POST", "/crawler/data", data);
                    }
                    LongSleep(TimeSpan.FromSeconds(Config.Instance.StorageApiSendInterval));
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Failed to store data");
                    errors.StoreMultipart(ex, data);
                    LongSleep(TimeSpan.FromSeconds(Config.Instance.RetryTimeout));
                }
            }
            storage.Flush();
        }
    }
}