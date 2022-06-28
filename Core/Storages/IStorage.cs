using System;

namespace Core.Storages
{
    public interface IStorage
    {
        void StoreData(Uri url, object data);
        void StoreException(CrawlingException ex);
    }
}
