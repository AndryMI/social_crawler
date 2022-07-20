
namespace Core.Data
{
    public interface IPostInfo
    {
        string Social { get; }
        string ProfileLink { get; }
        string Link { get; }

        /// <summary> Time in format: yyyy-MM-ddTHH:mm:ss.000Z</summary>
        string Time { get; }
    }
}
