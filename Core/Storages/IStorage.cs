
namespace Core.Storages
{
    public interface IStorage
    {
        void StoreException(CrawlingException ex);
    }
}
