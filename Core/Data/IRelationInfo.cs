
namespace Core.Data
{
    public interface IRelationInfo
    {
        string Social { get; }
        
        /// <summary>Profile link</summary>
        string Link { get; }

        /// <summary>Target profile link</summary>
        string TargetLink { get; }

        string Type { get; }
    }
}
