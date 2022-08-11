using Core.Crawling;
using Core.Data;
using System;

namespace Core.Storages
{
    public class NullStorage : IDataStorage, IMediaStorage, IErrorStorage
    {
        public bool WaitForBrowserLoading => false;

        public void StoreProfile(CrawlerTask task, IProfileInfo data)
        {
        }

        public void StorePost(CrawlerTask task, IPostInfo data)
        {
        }

        public void StoreComment(CrawlerTask task, ICommentInfo data)
        {
        }

        public void StoreException(CrawlingException ex)
        {
        }

        public void StoreMultipart(Exception ex, MultipartData data)
        {
        }

        public void StoreImage(ImageUrl image)
        {
        }
    }
}
