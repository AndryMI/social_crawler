using Core.Crawling;
using Core.Data;
using System;

namespace Core.Storages
{
    public class NullStorage : IDataStorage, IMediaStorage, IErrorStorage
    {
        public bool WaitForBrowserLoading { get; private set; }

        public NullStorage(bool wait = false)
        {
            WaitForBrowserLoading = wait;
        }

        public void StoreProfile(CrawlerTask task, IProfileInfo data)
        {
        }

        public void StorePost(CrawlerTask task, IPostInfo data)
        {
        }

        public void StoreComment(CrawlerTask task, ICommentInfo data)
        {
        }

        public void StoreRelation(CrawlerTask task, IRelationInfo data)
        {
        }

        public void StoreFriends(CrawlerTask task, IFriendListInfo data)
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
