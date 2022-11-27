using System;
using System.Collections.Concurrent;

namespace Core
{
    public class NetworkResponse
    {
        public readonly string MimeType;
        public readonly byte[] Data;

        public NetworkResponse(byte[] data, string mime)
        {
            MimeType = mime;
            Data = data;
        }
    }

    public class NetworkCache
    {
        private readonly ConcurrentDictionary<string, NetworkResponse> cache = new ConcurrentDictionary<string, NetworkResponse>();

        public NetworkResponse Get(string url, Func<string, NetworkResponse> request)
        {
            if (!cache.TryGetValue(url, out var response))
            {
                cache.TryAdd(url, response = request(url));
            }
            return response;
        }

        public void Clear()
        {
            cache.Clear();
        }
    }
}
