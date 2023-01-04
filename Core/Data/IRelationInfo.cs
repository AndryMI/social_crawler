using Newtonsoft.Json;

namespace Core.Data
{
    public interface IRelationInfo
    {
        string Social { get; }
        
        /// <summary>Profile link</summary>
        string Link { get; }

        /// <summary>Target profile link</summary>
        string TargetLink { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        string Type { get; }
    }
}
