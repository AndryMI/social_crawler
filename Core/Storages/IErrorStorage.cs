using System;

namespace Core.Storages
{
    public interface IErrorStorage
    {
        void StoreException(CrawlingException ex);
        void StoreMultipart(Exception ex, MultipartData data);
    }
}
