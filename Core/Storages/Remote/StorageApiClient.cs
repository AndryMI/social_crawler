using Core.Storages.Remote;

namespace Core.Storages
{
    public class StorageApiClient : ApiServerClient
    {
        private MultipartData data = new MultipartData();

        public bool IsReady => data.Size > Config.Instance.StorageApiSizeThreshold;

        public void Add(RequestChunk chunk)
        {
            data.Add(chunk.name, chunk.data, chunk.file, chunk.type);
        }

        public void Send()
        {
            if (data.Count > 0)
            {
                Request("POST", "/crawler/data", data);
                data = new MultipartData();
            }
        }
    }
}
