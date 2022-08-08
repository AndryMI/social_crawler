using Core.Data;

namespace Core.Storages
{
    public interface IMediaStorage
    {
        /// <summary>Wait for the browser to complete image requests</summary>
        bool WaitForBrowserLoading { get; }

        /// <summary>Stores or schedules to store image and updates Stored property</summary>
        void StoreImage(ImageUrl image);
    }
}
