using Core.Crawling;
using Core.Data;

namespace Core.Storages
{
    public interface IStorage
    {
        void StoreProfile(CrawlerTask task, IProfileInfo data);
        void StorePost(CrawlerTask task, IPostInfo data);
        void StoreComment(CrawlerTask task, ICommentInfo data);
        void StoreException(CrawlingException ex);
    }
}
