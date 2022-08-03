using Core.Data;

namespace Core.Storages
{
    public interface IMediaStorage
    {
        /// <summary>Wait for the browser to complete image requests</summary>
        bool WaitForBrowserLoading { get; }

        void StoreImage(ImageUrl image);
    }
}
