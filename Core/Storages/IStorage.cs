using Core.Crawling;
using System;

namespace Core.Storages
{
    public interface IStorage
    {
        void StoreData(CrawlerTask task, Uri url, object data);
        void StoreException(CrawlingException ex);
    }
}
