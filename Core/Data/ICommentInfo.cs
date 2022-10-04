
namespace Core.Data
{
    public interface ICommentInfo
    {
        string Social { get; }
        string ProfileLink { get; }
        string PostLink { get; }
        string Link { get; }

        string Author { get; }
        string AuthorLink { get; }

        /// <summary> Time in format: yyyy-MM-ddTHH:mm:ss.000Z</summary>
        string Time { get; }
    }
}
