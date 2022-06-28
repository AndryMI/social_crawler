using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Core.Storages
{
    public class DebugStorage : IStorage
    {
        public void StoreData(Uri url, object data)
        {
            Debug.WriteLine($"--- {url.Host}{url.LocalPath} ---");
            Debug.WriteLine(JsonConvert.SerializeObject(data));
        }

        public void StoreException(CrawlingException ex)
        {
            throw ex;
        }
    }
}
