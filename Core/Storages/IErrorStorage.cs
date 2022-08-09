
namespace Core.Storages
{
    public interface IErrorStorage
    {
        void StoreException(CrawlingException ex);
    }
}
