
namespace Core.Data
{
    /// <summary>Full list of friends/followers to track additions/removals</summary>
    public interface IFriendListInfo
    {
        string Social { get; }

        /// <summary>Profile link</summary>
        string Link { get; }

        /// <summary>Friends/followers profile links</summary>
        string[] TargetLinks { get; }
    }
}
