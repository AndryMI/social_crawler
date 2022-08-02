using Core.Crawling;
using Core.Data;

namespace Core.Storages
{
    public class NullStorage : IDataStorage
    {
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
    }
}
