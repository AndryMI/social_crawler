using Newtonsoft.Json;

namespace Core.Data
{
    public class ImageUrl
    {
        public readonly string Original;
        public string Stored;

        [JsonIgnore]
        public string MimeType;

        [JsonIgnore]
        public byte[] Data;

        public ImageUrl(string url)
        {
            Original = url;
        }

        public override string ToString()
        {
            return Stored ?? Original;
        }
    }
}
