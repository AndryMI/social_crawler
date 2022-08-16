using System.Collections.Concurrent;

namespace Core.Storages
{
    public class MemoryMultipartStorage : MultipartStorage
    {
        private readonly ConcurrentQueue<MultipartData> datas = new ConcurrentQueue<MultipartData>();

        protected override bool Dequeue(out MultipartData data)
        {
            return datas.TryDequeue(out data);
        }

        protected override void Enqueue(MultipartData data)
        {
            datas.Enqueue(data);
        }
    }
}
