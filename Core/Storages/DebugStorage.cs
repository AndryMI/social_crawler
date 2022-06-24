
namespace Core.Storages
{
    public class DebugStorage : IStorage
    {
        public void StoreException(CrawlingException ex)
        {
            throw ex;
        }
    }
}
